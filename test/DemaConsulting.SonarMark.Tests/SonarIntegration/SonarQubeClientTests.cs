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
using DemaConsulting.SonarMark.SonarIntegration;

namespace DemaConsulting.SonarMark.Tests.SonarIntegration;

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
    ///     Test that constructor with auth token sets the Authorization header
    /// </summary>
    [TestMethod]
    public void SonarQubeClient_Constructor_WithToken_SetsAuthorizationHeader()
    {
        // Act - create an HttpClient via the factory method that the public constructor uses
        using var httpClient = SonarQubeClient.CreateHttpClient("test-token");

        // Assert - the Authorization header must be set and use Basic scheme
        Assert.IsNotNull(httpClient.DefaultRequestHeaders.Authorization);
        Assert.AreEqual("Basic", httpClient.DefaultRequestHeaders.Authorization!.Scheme);
    }

    /// <summary>
    ///     Test that constructor without auth token does not set the Authorization header
    /// </summary>
    [TestMethod]
    public void SonarQubeClient_Constructor_WithoutToken_NoAuthorizationHeader()
    {
        // Act - create an HttpClient via the factory method without a token
        using var httpClient = SonarQubeClient.CreateHttpClient(null);

        // Assert - no Authorization header should be present
        Assert.IsNull(httpClient.DefaultRequestHeaders.Authorization);
    }

    /// <summary>
    ///     Test that a 401 Unauthorized response throws InvalidOperationException containing the status code
    /// </summary>
    [TestMethod]
    public async Task SonarQubeClient_GetQualityResultByBranchAsync_Http401Error_ThrowsInvalidOperationExceptionWithStatusCode()
    {
        // Arrange - set up handler returning 401 Unauthorized on first request (component show)
        var handler = new MockHttpMessageHandler();
        handler.EnqueueResponse(new HttpResponseMessage(HttpStatusCode.Unauthorized)
        {
            ReasonPhrase = "Unauthorized"
        });
        using var httpClient = new HttpClient(handler);
        using var client = new SonarQubeClient(httpClient, false);

        // Act - invoke the public method which internally calls GetProjectNameByKeyAsync first
        var ex = await Assert.ThrowsExactlyAsync<InvalidOperationException>(
            async () => await client.GetQualityResultByBranchAsync(
                "https://sonar.example.com",
                "my-project"));

        // Assert - exception message must include the HTTP status code
        Assert.IsTrue(
            ex.Message.Contains("401"),
            $"Expected exception message to contain '401' but was: {ex.Message}");
    }

    /// <summary>
    ///     Test that a 500 Internal Server Error response throws InvalidOperationException containing the status code
    /// </summary>
    [TestMethod]
    public async Task SonarQubeClient_GetQualityResultByBranchAsync_Http500Error_ThrowsInvalidOperationExceptionWithStatusCode()
    {
        // Arrange - set up handler returning 500 Internal Server Error on first request
        var handler = new MockHttpMessageHandler();
        handler.EnqueueResponse(new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            ReasonPhrase = "Internal Server Error"
        });
        using var httpClient = new HttpClient(handler);
        using var client = new SonarQubeClient(httpClient, false);

        // Act - invoke the public method and capture the exception
        var ex = await Assert.ThrowsExactlyAsync<InvalidOperationException>(
            async () => await client.GetQualityResultByBranchAsync(
                "https://sonar.example.com",
                "my-project"));

        // Assert - exception message must include the HTTP status code
        Assert.IsTrue(
            ex.Message.Contains("500"),
            $"Expected exception message to contain '500' but was: {ex.Message}");
    }

    /// <summary>
    ///     Test that issues spanning multiple pages are all accumulated into the result
    /// </summary>
    [TestMethod]
    public async Task SonarQubeClient_GetQualityResultByBranchAsync_IssuesPaginatedAcrossTwoPages_AccumulatesAllIssues()
    {
        // Arrange - build mock handler that serves all required API responses
        var handler = new MockHttpMessageHandler();

        // Component show response
        handler.EnqueueResponse(OkJson("""
            {"component":{"key":"my-project","name":"My Project"}}
            """));

        // Quality gate status response
        handler.EnqueueResponse(OkJson("""
            {"projectStatus":{"status":"OK","conditions":[]}}
            """));

        // Metrics search response
        handler.EnqueueResponse(OkJson("""
            {"metrics":[]}
            """));

        // Issues page 1 - paging indicates 150 total items (two pages of 100)
        handler.EnqueueResponse(OkJson("""
            {
              "paging":{"pageIndex":1,"pageSize":100,"total":150},
              "issues":[
                {"key":"issue-1","rule":"rule-1","severity":"MAJOR","component":"comp","message":"Issue One","type":"BUG"}
              ]
            }
            """));

        // Issues page 2 - last page
        handler.EnqueueResponse(OkJson("""
            {
              "paging":{"pageIndex":2,"pageSize":100,"total":150},
              "issues":[
                {"key":"issue-2","rule":"rule-2","severity":"MINOR","component":"comp","message":"Issue Two","type":"CODE_SMELL"}
              ]
            }
            """));

        // Hotspots single page
        handler.EnqueueResponse(OkJson("""
            {"paging":{"pageIndex":1,"pageSize":100,"total":0},"hotspots":[]}
            """));

        using var httpClient = new HttpClient(handler);
        using var client = new SonarQubeClient(httpClient, false);

        // Act - fetch quality result which internally paginates issues
        var result = await client.GetQualityResultByBranchAsync("https://sonar.example.com", "my-project");

        // Assert - both issues from both pages must be present in the result
        Assert.HasCount(2, result.Issues);
        Assert.AreEqual("issue-1", result.Issues[0].Key);
        Assert.AreEqual("issue-2", result.Issues[1].Key);
    }

    /// <summary>
    ///     Test that hot-spots spanning multiple pages are all accumulated into the result
    /// </summary>
    [TestMethod]
    public async Task SonarQubeClient_GetQualityResultByBranchAsync_HotSpotsPaginatedAcrossTwoPages_AccumulatesAllHotSpots()
    {
        // Arrange - build mock handler that serves all required API responses
        var handler = new MockHttpMessageHandler();

        // Component show response
        handler.EnqueueResponse(OkJson("""
            {"component":{"key":"my-project","name":"My Project"}}
            """));

        // Quality gate status response
        handler.EnqueueResponse(OkJson("""
            {"projectStatus":{"status":"OK","conditions":[]}}
            """));

        // Metrics search response
        handler.EnqueueResponse(OkJson("""
            {"metrics":[]}
            """));

        // Issues single page (no pagination needed)
        handler.EnqueueResponse(OkJson("""
            {"paging":{"pageIndex":1,"pageSize":100,"total":0},"issues":[]}
            """));

        // Hotspots page 1 - paging indicates 120 total items (two pages of 100)
        handler.EnqueueResponse(OkJson("""
            {
              "paging":{"pageIndex":1,"pageSize":100,"total":120},
              "hotspots":[
                {"key":"hs-1","component":"comp","message":"Hot Spot One","securityCategory":"xss","vulnerabilityProbability":"HIGH"}
              ]
            }
            """));

        // Hotspots page 2 - last page
        handler.EnqueueResponse(OkJson("""
            {
              "paging":{"pageIndex":2,"pageSize":100,"total":120},
              "hotspots":[
                {"key":"hs-2","component":"comp","message":"Hot Spot Two","securityCategory":"sqli","vulnerabilityProbability":"MEDIUM"}
              ]
            }
            """));

        using var httpClient = new HttpClient(handler);
        using var client = new SonarQubeClient(httpClient, false);

        // Act - fetch quality result which internally paginates hot-spots
        var result = await client.GetQualityResultByBranchAsync("https://sonar.example.com", "my-project");

        // Assert - both hot-spots from both pages must be present in the result
        Assert.HasCount(2, result.HotSpots);
        Assert.AreEqual("hs-1", result.HotSpots[0].Key);
        Assert.AreEqual("hs-2", result.HotSpots[1].Key);
    }

    /// <summary>
    ///     Creates an <see cref="HttpResponseMessage"/> with HTTP 200 OK and JSON body content
    /// </summary>
    /// <param name="json">JSON string to use as response body</param>
    /// <returns>Configured HTTP response message</returns>
    private static HttpResponseMessage OkJson(string json) =>
        new(HttpStatusCode.OK)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        };

    /// <summary>
    ///     Test double for <see cref="HttpMessageHandler"/> that serves pre-queued responses in order
    /// </summary>
    private sealed class MockHttpMessageHandler : HttpMessageHandler
    {
        /// <summary>
        ///     Queue of responses to serve in FIFO order
        /// </summary>
        private readonly Queue<HttpResponseMessage> _responses = new();

        /// <summary>
        ///     Enqueues a response to be returned by the next HTTP request
        /// </summary>
        /// <param name="response">Response to enqueue</param>
        public void EnqueueResponse(HttpResponseMessage response) =>
            _responses.Enqueue(response);

        /// <summary>
        ///     Returns the next queued response, or throws if the queue is empty
        /// </summary>
        /// <param name="request">Incoming HTTP request (not used)</param>
        /// <param name="cancellationToken">Cancellation token (not used)</param>
        /// <returns>Next queued response</returns>
        /// <exception cref="InvalidOperationException">Thrown when no more responses are queued</exception>
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
        ///     Disposes any responses that were queued but never dequeued
        /// </summary>
        /// <param name="disposing">True when called from Dispose(); false when called from finalizer</param>
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

