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
        var conditions = new List<SonarQualityCondition>();
        if (projectStatus.TryGetProperty("conditions", out var conditionsElement) &&
            conditionsElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var condition in conditionsElement.EnumerateArray())
            {
                var metric = condition.TryGetProperty("metricKey", out var metricElement)
                    ? metricElement.GetString() ?? string.Empty
                    : string.Empty;

                var comparator = condition.TryGetProperty("comparator", out var comparatorElement)
                    ? comparatorElement.GetString() ?? string.Empty
                    : string.Empty;

                var errorThreshold = condition.TryGetProperty("errorThreshold", out var errorThresholdElement)
                    ? errorThresholdElement.GetString()
                    : null;

                var actualValue = condition.TryGetProperty("actualValue", out var actualValueElement)
                    ? actualValueElement.GetString()
                    : null;

                var conditionStatus = condition.TryGetProperty("status", out var conditionStatusElement)
                    ? conditionStatusElement.GetString() ?? "NONE"
                    : "NONE";

                conditions.Add(new SonarQualityCondition(
                    metric,
                    comparator,
                    errorThreshold,
                    actualValue,
                    conditionStatus));
            }
        }

        return (qualityGateStatus, conditions);
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

        var issues = new List<SonarIssue>();
        if (issuesElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var issue in issuesElement.EnumerateArray())
            {
                var key = issue.TryGetProperty("key", out var keyElement)
                    ? keyElement.GetString() ?? string.Empty
                    : string.Empty;

                var rule = issue.TryGetProperty("rule", out var ruleElement)
                    ? ruleElement.GetString() ?? string.Empty
                    : string.Empty;

                var severity = issue.TryGetProperty("severity", out var severityElement)
                    ? severityElement.GetString() ?? string.Empty
                    : string.Empty;

                var component = issue.TryGetProperty("component", out var componentElement)
                    ? componentElement.GetString() ?? string.Empty
                    : string.Empty;

                int? line = null;
                if (issue.TryGetProperty("line", out var lineElement) && lineElement.ValueKind == JsonValueKind.Number)
                {
                    line = lineElement.GetInt32();
                }

                var message = issue.TryGetProperty("message", out var messageElement)
                    ? messageElement.GetString() ?? string.Empty
                    : string.Empty;

                var type = issue.TryGetProperty("type", out var typeElement)
                    ? typeElement.GetString() ?? string.Empty
                    : string.Empty;

                issues.Add(new SonarIssue(key, rule, severity, component, line, message, type));
            }
        }

        return issues;
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

        var hotSpots = new List<SonarHotSpot>();
        if (hotSpotsElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var hotSpot in hotSpotsElement.EnumerateArray())
            {
                var key = hotSpot.TryGetProperty("key", out var keyElement)
                    ? keyElement.GetString() ?? string.Empty
                    : string.Empty;

                var component = hotSpot.TryGetProperty("component", out var componentElement)
                    ? componentElement.GetString() ?? string.Empty
                    : string.Empty;

                int? line = null;
                if (hotSpot.TryGetProperty("line", out var lineElement) &&
                    lineElement.ValueKind == JsonValueKind.Number)
                {
                    line = lineElement.GetInt32();
                }

                var message = hotSpot.TryGetProperty("message", out var messageElement)
                    ? messageElement.GetString() ?? string.Empty
                    : string.Empty;

                var securityCategory = hotSpot.TryGetProperty("securityCategory", out var securityCategoryElement)
                    ? securityCategoryElement.GetString() ?? string.Empty
                    : string.Empty;

                var vulnerabilityProbability =
                    hotSpot.TryGetProperty("vulnerabilityProbability", out var vulnerabilityProbabilityElement)
                        ? vulnerabilityProbabilityElement.GetString() ?? string.Empty
                        : string.Empty;

                hotSpots.Add(new SonarHotSpot(key, component, line, message, securityCategory,
                    vulnerabilityProbability));
            }
        }

        return hotSpots;
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
