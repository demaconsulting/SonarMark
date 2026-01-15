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

using System.Net;
using System.Text.Json;

namespace DemaConsulting.SonarMark.Tests;

/// <summary>
///     Tests for SonarQubeClient class
/// </summary>
[TestClass]
public class SonarQubeClientTests
{
    /// <summary>
    ///     Test that constructor with auth token creates instance
    /// </summary>
    [TestMethod]
    public void SonarQubeClient_Constructor_WithAuthToken_CreatesInstance()
    {
        // Act - create client with authentication token
        using var client = new SonarQubeClient("test-token");

        // Assert - verify client was created
        Assert.IsNotNull(client);
    }

    /// <summary>
    ///     Test that constructor without auth token creates instance
    /// </summary>
    [TestMethod]
    public void SonarQubeClient_Constructor_WithoutAuthToken_CreatesInstance()
    {
        // Act - create client without authentication token
        using var client = new SonarQubeClient();

        // Assert - verify client was created
        Assert.IsNotNull(client);
    }

    /// <summary>
    ///     Test that GetResultsAsync throws for null report task
    /// </summary>
    [TestMethod]
    public async Task SonarQubeClient_GetResultsAsync_NullReportTask_ThrowsArgumentNullException()
    {
        // Arrange - create client
        using var client = new SonarQubeClient();

        // Act & Assert - verify exception is thrown for null report task
        await Assert.ThrowsAsync<ArgumentNullException>(
            async () => await client.GetResultsAsync(null!));
    }

    /// <summary>
    ///     Test that GetResultsAsync throws for zero timeout
    /// </summary>
    [TestMethod]
    public async Task SonarQubeClient_GetResultsAsync_ZeroTimeout_ThrowsArgumentException()
    {
        // Arrange - create client and report task
        using var client = new SonarQubeClient();
        var reportTask = new ReportTask("project", "https://sonarcloud.io/", "task123");

        // Act & Assert - verify exception is thrown for zero timeout
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            async () => await client.GetResultsAsync(reportTask, pollingTimeout: TimeSpan.Zero));
        Assert.Contains("timeout", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    ///     Test that GetResultsAsync throws for negative timeout
    /// </summary>
    [TestMethod]
    public async Task SonarQubeClient_GetResultsAsync_NegativeTimeout_ThrowsArgumentException()
    {
        // Arrange - create client and report task
        using var client = new SonarQubeClient();
        var reportTask = new ReportTask("project", "https://sonarcloud.io/", "task123");

        // Act & Assert - verify exception is thrown for negative timeout
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            async () => await client.GetResultsAsync(reportTask, pollingTimeout: TimeSpan.FromSeconds(-1)));
        Assert.Contains("timeout", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    ///     Test that GetResultsAsync returns success for completed task
    /// </summary>
    [TestMethod]
    public async Task SonarQubeClient_GetResultsAsync_CompletedTask_ReturnsSuccess()
    {
        // Arrange - create mock HTTP handler that returns success response
        var handler = new MockHttpMessageHandler(request =>
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(@"{
                    ""task"": {
                        ""id"": ""task123"",
                        ""status"": ""SUCCESS"",
                        ""analysisId"": ""analysis456""
                    }
                }")
            };
            return Task.FromResult(response);
        });

        using var httpClient = new HttpClient(handler);
        using var client = new SonarQubeClient(httpClient);
        var reportTask = new ReportTask("project", "https://sonarcloud.io/", "task123");

        // Act - get results
        var result = await client.GetResultsAsync(reportTask, pollingTimeout: TimeSpan.FromSeconds(5));

        // Assert - verify success status and analysis ID
        Assert.IsNotNull(result);
        Assert.AreEqual(CeTaskStatus.Success, result.Status);
        Assert.AreEqual("analysis456", result.AnalysisId);
    }

    /// <summary>
    ///     Test that GetResultsAsync throws for failed task
    /// </summary>
    [TestMethod]
    public async Task SonarQubeClient_GetResultsAsync_FailedTask_ThrowsInvalidOperationException()
    {
        // Arrange - create mock HTTP handler that returns failed response
        var handler = new MockHttpMessageHandler(request =>
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(@"{
                    ""task"": {
                        ""id"": ""task123"",
                        ""status"": ""FAILED""
                    }
                }")
            };
            return Task.FromResult(response);
        });

        using var httpClient = new HttpClient(handler);
        using var client = new SonarQubeClient(httpClient);
        var reportTask = new ReportTask("project", "https://sonarcloud.io/", "task123");

        // Act & Assert - verify exception is thrown for failed task
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await client.GetResultsAsync(reportTask, pollingTimeout: TimeSpan.FromSeconds(5)));
        Assert.Contains("failed", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    ///     Test that GetResultsAsync throws for canceled task
    /// </summary>
    [TestMethod]
    public async Task SonarQubeClient_GetResultsAsync_CanceledTask_ThrowsInvalidOperationException()
    {
        // Arrange - create mock HTTP handler that returns canceled response
        var handler = new MockHttpMessageHandler(request =>
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(@"{
                    ""task"": {
                        ""id"": ""task123"",
                        ""status"": ""CANCELED""
                    }
                }")
            };
            return Task.FromResult(response);
        });

        using var httpClient = new HttpClient(handler);
        using var client = new SonarQubeClient(httpClient);
        var reportTask = new ReportTask("project", "https://sonarcloud.io/", "task123");

        // Act & Assert - verify exception is thrown for canceled task
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await client.GetResultsAsync(reportTask, pollingTimeout: TimeSpan.FromSeconds(5)));
        Assert.Contains("canceled", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    ///     Test that GetResultsAsync polls until task completes
    /// </summary>
    [TestMethod]
    public async Task SonarQubeClient_GetResultsAsync_PendingThenSuccess_PollsUntilComplete()
    {
        // Arrange - create mock HTTP handler that returns pending then success
        var callCount = 0;
        var handler = new MockHttpMessageHandler(request =>
        {
            callCount++;
            var status = callCount < 3 ? "PENDING" : "SUCCESS";
            var analysisIdJson = callCount < 3 ? "" : @",""analysisId"": ""analysis456""";

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent($@"{{
                    ""task"": {{
                        ""id"": ""task123"",
                        ""status"": ""{status}""{analysisIdJson}
                    }}
                }}")
            };
            return Task.FromResult(response);
        });

        using var httpClient = new HttpClient(handler);
        using var client = new SonarQubeClient(httpClient);
        var reportTask = new ReportTask("project", "https://sonarcloud.io/", "task123");

        // Act - get results with polling
        var result = await client.GetResultsAsync(reportTask, pollingTimeout: TimeSpan.FromSeconds(30));

        // Assert - verify success after polling
        Assert.IsNotNull(result);
        Assert.AreEqual(CeTaskStatus.Success, result.Status);
        Assert.AreEqual("analysis456", result.AnalysisId);
        Assert.IsGreaterThanOrEqualTo(callCount, 3, $"Expected at least 3 calls, got {callCount}");
    }

    /// <summary>
    ///     Test that GetResultsAsync times out for long-running task
    /// </summary>
    [TestMethod]
    public async Task SonarQubeClient_GetResultsAsync_LongRunningTask_TimesOut()
    {
        // Arrange - create mock HTTP handler that always returns in progress
        var handler = new MockHttpMessageHandler(request =>
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(@"{
                    ""task"": {
                        ""id"": ""task123"",
                        ""status"": ""IN_PROGRESS""
                    }
                }")
            };
            return Task.FromResult(response);
        });

        using var httpClient = new HttpClient(handler);
        using var client = new SonarQubeClient(httpClient);
        var reportTask = new ReportTask("project", "https://sonarcloud.io/", "task123");

        // Act & Assert - verify timeout exception is thrown
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await client.GetResultsAsync(reportTask, pollingTimeout: TimeSpan.FromSeconds(2)));
        Assert.Contains("timed out", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    ///     Test that GetResultsAsync throws for invalid JSON response
    /// </summary>
    [TestMethod]
    public async Task SonarQubeClient_GetResultsAsync_InvalidJsonResponse_ThrowsException()
    {
        // Arrange - create mock HTTP handler that returns invalid JSON
        var handler = new MockHttpMessageHandler(request =>
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("invalid json")
            };
            return Task.FromResult(response);
        });

        using var httpClient = new HttpClient(handler);
        using var client = new SonarQubeClient(httpClient);
        var reportTask = new ReportTask("project", "https://sonarcloud.io/", "task123");

        // Act & Assert - verify exception is thrown for invalid JSON
        await Assert.ThrowsAsync<JsonException>(
            async () => await client.GetResultsAsync(reportTask, pollingTimeout: TimeSpan.FromSeconds(5)));
    }

    /// <summary>
    ///     Test that GetResultsAsync throws for missing task property
    /// </summary>
    [TestMethod]
    public async Task SonarQubeClient_GetResultsAsync_MissingTaskProperty_ThrowsInvalidOperationException()
    {
        // Arrange - create mock HTTP handler that returns response without task property
        var handler = new MockHttpMessageHandler(request =>
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(@"{""other"": ""value""}")
            };
            return Task.FromResult(response);
        });

        using var httpClient = new HttpClient(handler);
        using var client = new SonarQubeClient(httpClient);
        var reportTask = new ReportTask("project", "https://sonarcloud.io/", "task123");

        // Act & Assert - verify exception is thrown for missing task property
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await client.GetResultsAsync(reportTask, pollingTimeout: TimeSpan.FromSeconds(5)));
        Assert.Contains("task", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    ///     Test that GetResultsAsync throws for missing status property
    /// </summary>
    [TestMethod]
    public async Task SonarQubeClient_GetResultsAsync_MissingStatusProperty_ThrowsInvalidOperationException()
    {
        // Arrange - create mock HTTP handler that returns task without status
        var handler = new MockHttpMessageHandler(request =>
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(@"{""task"": {""id"": ""task123""}}")
            };
            return Task.FromResult(response);
        });

        using var httpClient = new HttpClient(handler);
        using var client = new SonarQubeClient(httpClient);
        var reportTask = new ReportTask("project", "https://sonarcloud.io/", "task123");

        // Act & Assert - verify exception is thrown for missing status
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await client.GetResultsAsync(reportTask, pollingTimeout: TimeSpan.FromSeconds(5)));
        Assert.Contains("status", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    ///     Test that GetResultsAsync throws for unknown status value
    /// </summary>
    [TestMethod]
    public async Task SonarQubeClient_GetResultsAsync_UnknownStatus_ThrowsInvalidOperationException()
    {
        // Arrange - create mock HTTP handler that returns unknown status
        var handler = new MockHttpMessageHandler(request =>
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(@"{
                    ""task"": {
                        ""id"": ""task123"",
                        ""status"": ""UNKNOWN_STATUS""
                    }
                }")
            };
            return Task.FromResult(response);
        });

        using var httpClient = new HttpClient(handler);
        using var client = new SonarQubeClient(httpClient);
        var reportTask = new ReportTask("project", "https://sonarcloud.io/", "task123");

        // Act & Assert - verify exception is thrown for unknown status
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await client.GetResultsAsync(reportTask, pollingTimeout: TimeSpan.FromSeconds(5)));
        Assert.Contains("unknown", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    ///     Test that GetResultsAsync constructs correct API URL
    /// </summary>
    [TestMethod]
    public async Task SonarQubeClient_GetResultsAsync_ValidRequest_ConstructsCorrectUrl()
    {
        // Arrange - create mock HTTP handler to capture request URL
        string? requestUrl = null;
        var handler = new MockHttpMessageHandler(request =>
        {
            requestUrl = request.RequestUri?.ToString();
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(@"{
                    ""task"": {
                        ""id"": ""task123"",
                        ""status"": ""SUCCESS"",
                        ""analysisId"": ""analysis456""
                    }
                }")
            };
            return Task.FromResult(response);
        });

        using var httpClient = new HttpClient(handler);
        using var client = new SonarQubeClient(httpClient);
        var reportTask = new ReportTask("project", "https://sonarcloud.io/", "task123");

        // Act - get results
        await client.GetResultsAsync(reportTask, pollingTimeout: TimeSpan.FromSeconds(5));

        // Assert - verify correct URL was constructed
        Assert.IsNotNull(requestUrl);
        Assert.AreEqual("https://sonarcloud.io/api/ce/task?id=task123", requestUrl);
    }

    /// <summary>
    ///     Mock HTTP message handler for testing
    /// </summary>
    private sealed class MockHttpMessageHandler : HttpMessageHandler
    {
        /// <summary>
        ///     Function to handle HTTP requests
        /// </summary>
        private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> _handler;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MockHttpMessageHandler"/> class
        /// </summary>
        /// <param name="handler">Function to handle HTTP requests</param>
        public MockHttpMessageHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> handler)
        {
            _handler = handler;
        }

        /// <summary>
        ///     Sends an HTTP request
        /// </summary>
        /// <param name="request">HTTP request message</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>HTTP response message</returns>
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return _handler(request);
        }
    }
}
