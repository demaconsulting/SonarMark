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
///     Client for interacting with SonarQube/SonarCloud APIs
/// </summary>
internal sealed class SonarQubeClient : IDisposable
{
    /// <summary>
    ///     Default polling timeout
    /// </summary>
    private static readonly TimeSpan DefaultPollingTimeout = TimeSpan.FromMinutes(5);

    /// <summary>
    ///     Default polling interval in milliseconds
    /// </summary>
    private const int DefaultPollingIntervalMs = 1000; // 1 second

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
    ///     Gets the results of a SonarQube analysis from a report task
    /// </summary>
    /// <param name="reportTask">Report task containing server and task information</param>
    /// <param name="pollingTimeout">Maximum time to wait for task completion (default: 5 minutes)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task result with analysis information</returns>
    /// <exception cref="InvalidOperationException">Thrown when task fails or times out</exception>
    public async Task<CeTaskResult> GetResultsAsync(
        ReportTask reportTask,
        TimeSpan? pollingTimeout = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(reportTask);

        var timeout = pollingTimeout ?? DefaultPollingTimeout;

        if (timeout <= TimeSpan.Zero)
        {
            throw new ArgumentException("Polling timeout must be positive", nameof(pollingTimeout));
        }

        var startTime = DateTime.UtcNow;

        while (true)
        {
            // Check for timeout
            if (DateTime.UtcNow - startTime > timeout)
            {
                throw new InvalidOperationException(
                    $"Timed out waiting for task {reportTask.CeTaskId} to complete after {timeout.TotalSeconds} seconds");
            }

            // Query the CE task status
            var result = await GetCeTaskStatusAsync(reportTask, cancellationToken).ConfigureAwait(false);

            // Return if task is in a terminal state
            if (result.Status is CeTaskStatus.Success or CeTaskStatus.Failed or CeTaskStatus.Canceled)
            {
                if (result.Status == CeTaskStatus.Failed)
                {
                    throw new InvalidOperationException($"Task {reportTask.CeTaskId} failed");
                }

                if (result.Status == CeTaskStatus.Canceled)
                {
                    throw new InvalidOperationException($"Task {reportTask.CeTaskId} was canceled");
                }

                return result;
            }

            // Wait before polling again
            await Task.Delay(DefaultPollingIntervalMs, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    ///     Gets the current status of a Compute Engine task
    /// </summary>
    /// <param name="reportTask">Report task containing server and task information</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task result with current status</returns>
    private async Task<CeTaskResult> GetCeTaskStatusAsync(
        ReportTask reportTask,
        CancellationToken cancellationToken)
    {
        var url = $"{reportTask.ServerUrl.TrimEnd('/')}/api/ce/task?id={reportTask.CeTaskId}";

        var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        var jsonDoc = JsonDocument.Parse(content);

        var root = jsonDoc.RootElement;
        if (!root.TryGetProperty("task", out var taskElement))
        {
            throw new InvalidOperationException("Invalid CE task response: missing 'task' property");
        }

        // Parse the status
        if (!taskElement.TryGetProperty("status", out var statusElement))
        {
            throw new InvalidOperationException("Invalid CE task response: missing 'status' property");
        }

        var statusString = statusElement.GetString();
        var status = statusString?.ToUpperInvariant() switch
        {
            "PENDING" => CeTaskStatus.Pending,
            "IN_PROGRESS" => CeTaskStatus.InProgress,
            "SUCCESS" => CeTaskStatus.Success,
            "FAILED" => CeTaskStatus.Failed,
            "CANCELED" => CeTaskStatus.Canceled,
            _ => throw new InvalidOperationException($"Unknown task status: {statusString}")
        };

        // Extract analysis ID if available
        string? analysisId = null;
        if (taskElement.TryGetProperty("analysisId", out var analysisIdElement))
        {
            analysisId = analysisIdElement.GetString();
        }

        return new CeTaskResult(status, analysisId);
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
