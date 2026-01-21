// Copyright (c) DEMA Consulting
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace DemaConsulting.SonarMark;

/// <summary>
///     Client for fetching quality information from SonarQube/SonarCloud analysis
/// </summary>
/// <remarks>
///     This client's primary responsibility is to retrieve analysis quality results
///     including quality gate status and conditions. It handles waiting for tasks
///     to complete and fetching associated quality data.
/// </remarks>
internal sealed class SonarQubeClient : IDisposable
{
    /// <summary>
    ///     HTTP client for making API requests
    /// </summary>
    private readonly HttpClient _httpClient;

    /// <summary>
    ///     Indicates whether this instance owns the HttpClient
    /// </summary>
    private readonly bool _ownsHttpClient;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SonarQubeClient"/> class
    /// </summary>
    /// <param name="authToken">Optional authentication token (PAT)</param>
    public SonarQubeClient(string? authToken = null)
        : this(CreateHttpClient(authToken), true)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="SonarQubeClient"/> class with a custom HttpClient
    /// </summary>
    /// <param name="httpClient">HTTP client to use for requests</param>
    /// <param name="ownsHttpClient">Whether this instance should dispose the HttpClient</param>
    internal SonarQubeClient(HttpClient httpClient, bool ownsHttpClient = false)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _ownsHttpClient = ownsHttpClient;
    }

    /// <summary>
    ///     Gets the quality analysis results from SonarQube/SonarCloud using project key and branch
    /// </summary>
    /// <param name="serverUrl">Server URL</param>
    /// <param name="projectKey">Project key</param>
    /// <param name="branch">Branch name (optional)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Quality analysis results including quality gate status, conditions, issues, and hot-spots</returns>
    /// <exception cref="InvalidOperationException">Thrown when request fails</exception>
    public async Task<SonarQualityResult> GetQualityResultByBranchAsync(
        string serverUrl,
        string projectKey,
        string? branch = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(serverUrl);
        ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);

        // Fetch the project name
        var projectName = await GetProjectNameByKeyAsync(serverUrl, projectKey, cancellationToken)
            .ConfigureAwait(false);

        // Fetch the quality gate status for the branch
        var (qualityGateStatus, conditions) = await GetQualityGateStatusByBranchAsync(
            serverUrl,
            projectKey,
            branch,
            cancellationToken).ConfigureAwait(false);

        // Fetch metric names to provide friendly names in the report
        var metricNames = await GetMetricNamesByServerAsync(serverUrl, cancellationToken).ConfigureAwait(false);

        // Fetch issues
        var issues = await GetIssuesAsync(serverUrl, projectKey, branch, cancellationToken).ConfigureAwait(false);

        // Fetch hot-spots
        var hotSpots = await GetHotSpotsAsync(serverUrl, projectKey, branch, cancellationToken)
            .ConfigureAwait(false);

        return new SonarQualityResult(
            serverUrl,
            projectKey,
            projectName,
            qualityGateStatus,
            conditions,
            metricNames,
            issues,
            hotSpots);
    }

    /// <summary>
    ///     Gets the project name from SonarQube/SonarCloud
    /// </summary>
    /// <param name="serverUrl">Server URL</param>
    /// <param name="projectKey">Project key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Project name</returns>
    private async Task<string> GetProjectNameByKeyAsync(
        string serverUrl,
        string projectKey,
        CancellationToken cancellationToken)
    {
        var url = $"{serverUrl.TrimEnd('/')}/api/components/show?component={projectKey}";

        var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        var jsonDoc = JsonDocument.Parse(content);

        var root = jsonDoc.RootElement;
        if (!root.TryGetProperty("component", out var component))
        {
            throw new InvalidOperationException("Invalid component response: missing 'component' property");
        }

        if (!component.TryGetProperty("name", out var nameElement))
        {
            throw new InvalidOperationException("Invalid component response: missing 'name' property");
        }

        // Return project name, or fallback to project key if name is null/empty
        return nameElement.GetString() ?? projectKey;
    }

    /// <summary>
    ///     Gets the quality gate status for a branch
    /// </summary>
    /// <param name="serverUrl">Server URL</param>
    /// <param name="projectKey">Project key</param>
    /// <param name="branch">Branch name (optional)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Quality gate status and conditions</returns>
    private async Task<(string QualityGateStatus, List<SonarQualityCondition> Conditions)>
        GetQualityGateStatusByBranchAsync(
            string serverUrl,
            string projectKey,
            string? branch,
            CancellationToken cancellationToken)
    {
        var url = $"{serverUrl.TrimEnd('/')}/api/qualitygates/project_status?projectKey={projectKey}";
        if (!string.IsNullOrWhiteSpace(branch))
        {
            url += $"&branch={Uri.EscapeDataString(branch)}";
        }

        var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        var jsonDoc = JsonDocument.Parse(content);

        var root = jsonDoc.RootElement;
        if (!root.TryGetProperty("projectStatus", out var projectStatus))
        {
            throw new InvalidOperationException("Invalid quality gate response: missing 'projectStatus' property");
        }

        // Parse quality gate status
        if (!projectStatus.TryGetProperty("status", out var statusElement))
        {
            throw new InvalidOperationException("Invalid quality gate response: missing 'status' property");
        }

        var qualityGateStatus = statusElement.GetString() ?? "NONE";

        // Parse conditions
        var conditions = ParseQualityGateConditions(projectStatus);

        return (qualityGateStatus, conditions);
    }

    /// <summary>
    ///     Parses quality gate conditions from a JSON element
    /// </summary>
    /// <param name="projectStatus">Project status JSON element</param>
    /// <returns>List of quality gate conditions</returns>
    private static List<SonarQualityCondition> ParseQualityGateConditions(JsonElement projectStatus)
    {
        var conditions = new List<SonarQualityCondition>();

        if (!projectStatus.TryGetProperty("conditions", out var conditionsElement) ||
            conditionsElement.ValueKind != JsonValueKind.Array)
        {
            return conditions;
        }

        foreach (var condition in conditionsElement.EnumerateArray())
        {
            var parsedCondition = ParseQualityGateCondition(condition);
            conditions.Add(parsedCondition);
        }

        return conditions;
    }

    /// <summary>
    ///     Parses a single quality gate condition from a JSON element
    /// </summary>
    /// <param name="condition">Condition JSON element</param>
    /// <returns>Parsed quality gate condition</returns>
    private static SonarQualityCondition ParseQualityGateCondition(JsonElement condition)
    {
        var metric = GetStringProperty(condition, "metricKey", string.Empty);
        var comparator = GetStringProperty(condition, "comparator", string.Empty);
        var errorThreshold = GetNullableStringProperty(condition, "errorThreshold");
        var actualValue = GetNullableStringProperty(condition, "actualValue");
        var conditionStatus = GetStringProperty(condition, "status", "NONE");

        return new SonarQualityCondition(metric, comparator, errorThreshold, actualValue, conditionStatus);
    }

    /// <summary>
    ///     Gets issues from SonarQube/SonarCloud
    /// </summary>
    /// <param name="serverUrl">Server URL</param>
    /// <param name="projectKey">Project key</param>
    /// <param name="branch">Branch name (optional)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of issues</returns>
    /// <remarks>
    ///     Page size is limited to 500 as per requirements. For projects with more than 500 issues,
    ///     only the first 500 will be returned. Future enhancements could implement pagination.
    /// </remarks>
    private async Task<List<SonarIssue>> GetIssuesAsync(
        string serverUrl,
        string projectKey,
        string? branch,
        CancellationToken cancellationToken)
    {
        // Note: Page size is limited to 500 as per requirements
        var url =
            $"{serverUrl.TrimEnd('/')}/api/issues/search?componentKeys={projectKey}&issueStatuses=OPEN,CONFIRMED&ps=500";
        if (!string.IsNullOrWhiteSpace(branch))
        {
            url += $"&branch={Uri.EscapeDataString(branch)}";
        }

        var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        var jsonDoc = JsonDocument.Parse(content);

        var root = jsonDoc.RootElement;
        if (!root.TryGetProperty("issues", out var issuesElement))
        {
            throw new InvalidOperationException("Invalid issues response: missing 'issues' property");
        }

        return ParseIssues(issuesElement);
    }

    /// <summary>
    ///     Parses issues from a JSON array element
    /// </summary>
    /// <param name="issuesElement">Issues JSON array element</param>
    /// <returns>List of parsed issues</returns>
    private static List<SonarIssue> ParseIssues(JsonElement issuesElement)
    {
        var issues = new List<SonarIssue>();

        if (issuesElement.ValueKind != JsonValueKind.Array)
        {
            return issues;
        }

        foreach (var issue in issuesElement.EnumerateArray())
        {
            var parsedIssue = ParseIssue(issue);
            issues.Add(parsedIssue);
        }

        return issues;
    }

    /// <summary>
    ///     Parses a single issue from a JSON element
    /// </summary>
    /// <param name="issue">Issue JSON element</param>
    /// <returns>Parsed issue</returns>
    private static SonarIssue ParseIssue(JsonElement issue)
    {
        var key = GetStringProperty(issue, "key", string.Empty);
        var rule = GetStringProperty(issue, "rule", string.Empty);
        var severity = GetStringProperty(issue, "severity", string.Empty);
        var component = GetStringProperty(issue, "component", string.Empty);
        var line = GetIntProperty(issue, "line");
        var message = GetStringProperty(issue, "message", string.Empty);
        var type = GetStringProperty(issue, "type", string.Empty);

        return new SonarIssue(key, rule, severity, component, line, message, type);
    }

    /// <summary>
    ///     Gets security hot-spots from SonarQube/SonarCloud
    /// </summary>
    /// <param name="serverUrl">Server URL</param>
    /// <param name="projectKey">Project key</param>
    /// <param name="branch">Branch name (optional)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of hot-spots</returns>
    /// <remarks>
    ///     Page size is limited to 500 as per requirements. For projects with more than 500 hot-spots,
    ///     only the first 500 will be returned. Future enhancements could implement pagination.
    /// </remarks>
    private async Task<List<SonarHotSpot>> GetHotSpotsAsync(
        string serverUrl,
        string projectKey,
        string? branch,
        CancellationToken cancellationToken)
    {
        // Note: Page size is limited to 500 as per requirements
        var url = $"{serverUrl.TrimEnd('/')}/api/hotspots/search?projectKey={projectKey}&ps=500";
        if (!string.IsNullOrWhiteSpace(branch))
        {
            url += $"&branch={Uri.EscapeDataString(branch)}";
        }

        var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        var jsonDoc = JsonDocument.Parse(content);

        var root = jsonDoc.RootElement;
        if (!root.TryGetProperty("hotspots", out var hotSpotsElement))
        {
            throw new InvalidOperationException("Invalid hot-spots response: missing 'hotspots' property");
        }

        return ParseHotSpots(hotSpotsElement);
    }

    /// <summary>
    ///     Parses hot-spots from a JSON array element
    /// </summary>
    /// <param name="hotSpotsElement">Hot-spots JSON array element</param>
    /// <returns>List of parsed hot-spots</returns>
    private static List<SonarHotSpot> ParseHotSpots(JsonElement hotSpotsElement)
    {
        var hotSpots = new List<SonarHotSpot>();

        if (hotSpotsElement.ValueKind != JsonValueKind.Array)
        {
            return hotSpots;
        }

        foreach (var hotSpot in hotSpotsElement.EnumerateArray())
        {
            var parsedHotSpot = ParseHotSpot(hotSpot);
            hotSpots.Add(parsedHotSpot);
        }

        return hotSpots;
    }

    /// <summary>
    ///     Parses a single hot-spot from a JSON element
    /// </summary>
    /// <param name="hotSpot">Hot-spot JSON element</param>
    /// <returns>Parsed hot-spot</returns>
    private static SonarHotSpot ParseHotSpot(JsonElement hotSpot)
    {
        var key = GetStringProperty(hotSpot, "key", string.Empty);
        var component = GetStringProperty(hotSpot, "component", string.Empty);
        var line = GetIntProperty(hotSpot, "line");
        var message = GetStringProperty(hotSpot, "message", string.Empty);
        var securityCategory = GetStringProperty(hotSpot, "securityCategory", string.Empty);
        var vulnerabilityProbability = GetStringProperty(hotSpot, "vulnerabilityProbability", string.Empty);

        return new SonarHotSpot(key, component, line, message, securityCategory, vulnerabilityProbability);
    }

    /// <summary>
    ///     Gets metric names from the SonarQube/SonarCloud API
    /// </summary>
    /// <param name="serverUrl">Server URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dictionary mapping metric keys to friendly names</returns>
    private async Task<IReadOnlyDictionary<string, string>> GetMetricNamesByServerAsync(
        string serverUrl,
        CancellationToken cancellationToken)
    {
        var url = $"{serverUrl.TrimEnd('/')}/api/metrics/search";

        var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        var jsonDoc = JsonDocument.Parse(content);

        var root = jsonDoc.RootElement;
        if (!root.TryGetProperty("metrics", out var metricsElement))
        {
            throw new InvalidOperationException("Invalid metrics response: missing 'metrics' property");
        }

        var metricNames = new Dictionary<string, string>();
        if (metricsElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var metric in metricsElement.EnumerateArray())
            {
                var key = metric.TryGetProperty("key", out var keyElement)
                    ? keyElement.GetString()
                    : null;

                var name = metric.TryGetProperty("name", out var nameElement)
                    ? nameElement.GetString()
                    : null;

                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(name))
                {
                    metricNames[key] = name;
                }
            }
        }

        return metricNames;
    }

    /// <summary>
    ///     Gets a string property from a JSON element with a default value
    /// </summary>
    /// <param name="element">JSON element</param>
    /// <param name="propertyName">Property name</param>
    /// <param name="defaultValue">Default value if property is missing</param>
    /// <returns>Property value or default</returns>
    private static string GetStringProperty(JsonElement element, string propertyName, string defaultValue)
    {
        return element.TryGetProperty(propertyName, out var property)
            ? property.GetString() ?? defaultValue
            : defaultValue;
    }

    /// <summary>
    ///     Gets a nullable string property from a JSON element
    /// </summary>
    /// <param name="element">JSON element</param>
    /// <param name="propertyName">Property name</param>
    /// <returns>Property value or null</returns>
    private static string? GetNullableStringProperty(JsonElement element, string propertyName)
    {
        return element.TryGetProperty(propertyName, out var property) ? property.GetString() : null;
    }

    /// <summary>
    ///     Gets an integer property from a JSON element
    /// </summary>
    /// <param name="element">JSON element</param>
    /// <param name="propertyName">Property name</param>
    /// <returns>Property value or null if missing or not a number</returns>
    private static int? GetIntProperty(JsonElement element, string propertyName)
    {
        if (element.TryGetProperty(propertyName, out var property) &&
            property.ValueKind == JsonValueKind.Number)
        {
            return property.GetInt32();
        }

        return null;
    }

    /// <summary>
    ///     Disposes the HTTP client if owned by this instance
    /// </summary>
    public void Dispose()
    {
        if (_ownsHttpClient)
        {
            _httpClient.Dispose();
        }
    }

    /// <summary>
    ///     Creates an HTTP client with optional authentication
    /// </summary>
    /// <param name="authToken">Optional authentication token</param>
    /// <returns>Configured HTTP client</returns>
    private static HttpClient CreateHttpClient(string? authToken)
    {
        var client = new HttpClient();

        if (!string.IsNullOrWhiteSpace(authToken))
        {
            // SonarQube uses Basic authentication with token as username and empty password
            var authValue = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{authToken}:"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authValue);
        }

        return client;
    }
}
