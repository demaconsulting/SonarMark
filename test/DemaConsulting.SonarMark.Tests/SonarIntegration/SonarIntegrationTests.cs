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
using DemaConsulting.SonarMark.SonarIntegration;

namespace DemaConsulting.SonarMark.Tests.SonarIntegration;

/// <summary>
///     Subsystem tests for the SonarIntegration subsystem (SonarQubeClient, SonarIssue, SonarHotSpot working together).
/// </summary>
[TestClass]
public class SonarIntegrationTests
{
    /// <summary>
    ///     Test that the subsystem fetches quality gate status from the server.
    /// </summary>
    [TestMethod]
    public async Task SonarIntegration_FetchQualityResult_ReturnsQualityGateStatus()
    {
        // Arrange - build mock handler returning OK quality gate status
        var handler = new MockHttpMessageHandler();

        // Component show response
        handler.EnqueueResponse(OkJson("""
            {"component":{"key":"test-project","name":"Test Project"}}
            """));

        // Quality gate status — reporting OK
        handler.EnqueueResponse(OkJson("""
            {"projectStatus":{"status":"OK","conditions":[]}}
            """));

        // Metrics search response
        handler.EnqueueResponse(OkJson("""
            {"metrics":[]}
            """));

        // Issues — none
        handler.EnqueueResponse(OkJson("""
            {"paging":{"pageIndex":1,"pageSize":100,"total":0},"issues":[]}
            """));

        // Hot-spots — none
        handler.EnqueueResponse(OkJson("""
            {"paging":{"pageIndex":1,"pageSize":100,"total":0},"hotspots":[]}
            """));

        using var httpClient = new HttpClient(handler);
        using var client = new SonarQubeClient(httpClient, false);

        // Act - fetch quality result through the subsystem
        var result = await client.GetQualityResultByBranchAsync("https://sonar.example.com", "test-project");

        // Assert - quality gate status must be propagated from server response through subsystem
        Assert.AreEqual("OK", result.QualityGateStatus);
        Assert.AreEqual("Test Project", result.ProjectName);
    }

    /// <summary>
    ///     Test that the subsystem fetches issues from the server and includes them in the result.
    /// </summary>
    [TestMethod]
    public async Task SonarIntegration_FetchQualityResult_ReturnsIssues()
    {
        // Arrange - build mock handler returning one issue
        var handler = new MockHttpMessageHandler();

        // Component show response
        handler.EnqueueResponse(OkJson("""
            {"component":{"key":"test-project","name":"Test Project"}}
            """));

        // Quality gate status response
        handler.EnqueueResponse(OkJson("""
            {"projectStatus":{"status":"OK","conditions":[]}}
            """));

        // Metrics search response
        handler.EnqueueResponse(OkJson("""
            {"metrics":[]}
            """));

        // Issues — one issue on a single page
        handler.EnqueueResponse(OkJson("""
            {
              "paging":{"pageIndex":1,"pageSize":100,"total":1},
              "issues":[
                {"key":"issue-1","rule":"rule-1","severity":"MAJOR","component":"comp","message":"Issue One","type":"BUG"}
              ]
            }
            """));

        // Hot-spots — none
        handler.EnqueueResponse(OkJson("""
            {"paging":{"pageIndex":1,"pageSize":100,"total":0},"hotspots":[]}
            """));

        using var httpClient = new HttpClient(handler);
        using var client = new SonarQubeClient(httpClient, false);

        // Act - fetch quality result through the subsystem
        var result = await client.GetQualityResultByBranchAsync("https://sonar.example.com", "test-project");

        // Assert - the issue must be included in the result
        Assert.HasCount(1, result.Issues);
        Assert.AreEqual("issue-1", result.Issues[0].Key);
        Assert.AreEqual("MAJOR", result.Issues[0].Severity);
    }

    /// <summary>
    ///     Test that the subsystem fetches hot-spots from the server and includes them in the result.
    /// </summary>
    [TestMethod]
    public async Task SonarIntegration_FetchQualityResult_ReturnsHotSpots()
    {
        // Arrange - build mock handler returning one hot-spot
        var handler = new MockHttpMessageHandler();

        // Component show response
        handler.EnqueueResponse(OkJson("""
            {"component":{"key":"test-project","name":"Test Project"}}
            """));

        // Quality gate status response
        handler.EnqueueResponse(OkJson("""
            {"projectStatus":{"status":"OK","conditions":[]}}
            """));

        // Metrics search response
        handler.EnqueueResponse(OkJson("""
            {"metrics":[]}
            """));

        // Issues — none
        handler.EnqueueResponse(OkJson("""
            {"paging":{"pageIndex":1,"pageSize":100,"total":0},"issues":[]}
            """));

        // Hot-spots — one hot-spot on a single page
        handler.EnqueueResponse(OkJson("""
            {
              "paging":{"pageIndex":1,"pageSize":100,"total":1},
              "hotspots":[
                {"key":"hs-1","component":"comp","message":"Hot Spot One","securityCategory":"xss","vulnerabilityProbability":"HIGH"}
              ]
            }
            """));

        using var httpClient = new HttpClient(handler);
        using var client = new SonarQubeClient(httpClient, false);

        // Act - fetch quality result through the subsystem
        var result = await client.GetQualityResultByBranchAsync("https://sonar.example.com", "test-project");

        // Assert - the hot-spot must be included in the result
        Assert.HasCount(1, result.HotSpots);
        Assert.AreEqual("hs-1", result.HotSpots[0].Key);
        Assert.AreEqual("HIGH", result.HotSpots[0].VulnerabilityProbability);
    }

    /// <summary>
    ///     Creates an <see cref="HttpResponseMessage"/> with HTTP 200 OK and JSON body content.
    /// </summary>
    /// <param name="json">JSON string to use as response body.</param>
    /// <returns>Configured HTTP response message.</returns>
    private static HttpResponseMessage OkJson(string json) =>
        new(HttpStatusCode.OK)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        };

    /// <summary>
    ///     Test double for <see cref="HttpMessageHandler"/> that serves pre-queued responses in order.
    /// </summary>
    private sealed class MockHttpMessageHandler : HttpMessageHandler
    {
        /// <summary>
        ///     Queue of responses to serve in FIFO order.
        /// </summary>
        private readonly Queue<HttpResponseMessage> _responses = new();

        /// <summary>
        ///     Enqueues a response to be returned by the next HTTP request.
        /// </summary>
        /// <param name="response">Response to enqueue.</param>
        public void EnqueueResponse(HttpResponseMessage response) =>
            _responses.Enqueue(response);

        /// <summary>
        ///     Returns the next queued response, or throws if the queue is empty.
        /// </summary>
        /// <param name="request">Incoming HTTP request (not used).</param>
        /// <param name="cancellationToken">Cancellation token (not used).</param>
        /// <returns>Next queued response.</returns>
        /// <exception cref="InvalidOperationException">Thrown when no more responses are queued.</exception>
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            // Dequeue and return the next pre-configured response
            if (_responses.Count == 0)
            {
                throw new InvalidOperationException(
                    $"MockHttpMessageHandler has no more queued responses. Request was: {request.RequestUri}");
            }

            return Task.FromResult(_responses.Dequeue());
        }

        /// <summary>
        ///     Disposes any responses that were queued but never dequeued.
        /// </summary>
        /// <param name="disposing">True when called from Dispose(); false when called from finalizer.</param>
        protected override void Dispose(bool disposing)
        {
            // Drain and dispose any remaining queued response objects to avoid resource leaks
            if (disposing)
            {
                while (_responses.Count > 0)
                {
                    _responses.Dequeue().Dispose();
                }
            }

            base.Dispose(disposing);
        }
    }
}
