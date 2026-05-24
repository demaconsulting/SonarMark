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

using DemaConsulting.SonarMark.SonarIntegration;
using Xunit;

namespace DemaConsulting.SonarMark.Tests.SonarIntegration;

/// <summary>
///     Subsystem tests for the SonarIntegration subsystem (SonarQubeClient, SonarIssue, SonarHotSpot working together).
/// </summary>
public class SonarIntegrationTests
{
    /// <summary>
    ///     Test that the subsystem fetches quality gate status from the server.
    /// </summary>
    [Fact]
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
        var result = await client.GetQualityResultByBranchAsync("https://sonar.example.com", "test-project", cancellationToken: TestContext.Current.CancellationToken);

        // Assert - quality gate status must be propagated from server response through subsystem
        Assert.Equal("OK", result.QualityGateStatus);
        Assert.Equal("Test Project", result.ProjectName);
    }

    /// <summary>
    ///     Test that the subsystem fetches issues from the server and includes them in the result.
    /// </summary>
    [Fact]
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
        var result = await client.GetQualityResultByBranchAsync("https://sonar.example.com", "test-project", cancellationToken: TestContext.Current.CancellationToken);

        // Assert - the issue must be included in the result
        Assert.Single(result.Issues);
        Assert.Equal("issue-1", result.Issues[0].Key);
        Assert.Equal("MAJOR", result.Issues[0].Severity);
    }

    /// <summary>
    ///     Test that the subsystem fetches hot-spots from the server and includes them in the result.
    /// </summary>
    [Fact]
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
        var result = await client.GetQualityResultByBranchAsync("https://sonar.example.com", "test-project", cancellationToken: TestContext.Current.CancellationToken);

        // Assert - the hot-spot must be included in the result
        Assert.Single(result.HotSpots);
        Assert.Equal("hs-1", result.HotSpots[0].Key);
        Assert.Equal("HIGH", result.HotSpots[0].VulnerabilityProbability);
    }

    /// <summary>
    ///     Test that a null/empty server URL causes ArgumentException before any HTTP call.
    /// </summary>
    [Fact]
    public async Task SonarIntegration_NullServerUrl_ThrowsArgumentException()
    {
        // Arrange - mock handler is never called because validation happens first
        var handler = new MockHttpMessageHandler();
        using var httpClient = new HttpClient(handler);
        using var client = new SonarQubeClient(httpClient, false);

        // Act / Assert - null server URL must throw before any HTTP call is made
        await Assert.ThrowsAnyAsync<ArgumentException>(
            async () => await client.GetQualityResultByBranchAsync(null!, "test-project", cancellationToken: TestContext.Current.CancellationToken));
    }

    /// <summary>
    ///     Test that a non-2xx HTTP response raises InvalidOperationException through the subsystem.
    /// </summary>
    [Fact]
    public async Task SonarIntegration_NonSuccessHttpResponse_ThrowsInvalidOperationException()
    {
        // Arrange - configure mock to return HTTP 500 on first API call
        var handler = new MockHttpMessageHandler();
        handler.EnqueueResponse(new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError)
        {
            ReasonPhrase = "Internal Server Error"
        });
        using var httpClient = new HttpClient(handler);
        using var client = new SonarQubeClient(httpClient, false);

        // Act / Assert - non-success HTTP response must propagate as InvalidOperationException
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await client.GetQualityResultByBranchAsync("https://sonar.example.com", "test-project", cancellationToken: TestContext.Current.CancellationToken));
    }

    /// <summary>
    ///     Test that a malformed JSON response raises JsonException through the subsystem.
    /// </summary>
    [Fact]
    public async Task SonarIntegration_MalformedJsonResponse_ThrowsJsonException()
    {
        // Arrange - configure mock to return HTTP 200 with invalid JSON body
        var handler = new MockHttpMessageHandler();
        handler.EnqueueResponse(new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.OK)
        {
            Content = new System.Net.Http.StringContent(
                "{ this is not valid json !!!",
                System.Text.Encoding.UTF8,
                "application/json")
        });
        using var httpClient = new HttpClient(handler);
        using var client = new SonarQubeClient(httpClient, false);

        // Act / Assert - malformed JSON must propagate as JsonException
        await Assert.ThrowsAnyAsync<System.Text.Json.JsonException>(
            async () => await client.GetQualityResultByBranchAsync("https://sonar.example.com", "test-project", cancellationToken: TestContext.Current.CancellationToken));
    }

    private static HttpResponseMessage OkJson(string json) =>
        SonarIntegrationTestHelpers.OkJson(json);
}

