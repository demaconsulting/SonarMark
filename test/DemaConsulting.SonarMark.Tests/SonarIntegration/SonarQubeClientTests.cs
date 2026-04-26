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
                {"key":"hs-2","component":"comp","message":"Hot Spot Two","securityCategory":"sql-injection","vulnerabilityProbability":"MEDIUM"}
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
    ///     Test that quality gate status is correctly returned from the API response
    /// </summary>
    [TestMethod]
    public async Task SonarQubeClient_GetQualityResultByBranchAsync_ReturnsQualityGateStatus()
    {
        // Arrange - build mock handler returning OK quality gate status
        var handler = new MockHttpMessageHandler();

        // Component show response
        handler.EnqueueResponse(OkJson("""
            {"component":{"key":"my-project","name":"My Project"}}
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

        // Act - fetch quality result which retrieves quality gate status
        var result = await client.GetQualityResultByBranchAsync("https://sonar.example.com", "my-project");

        // Assert - quality gate status must be returned correctly from the mock response
        Assert.AreEqual("OK", result.QualityGateStatus);
    }

    private static HttpResponseMessage OkJson(string json) =>
        SonarIntegrationTestHelpers.OkJson(json);

    /// <summary>
    ///     Test that null server URL throws ArgumentNullException
    /// </summary>
    [TestMethod]
    public async Task SonarQubeClient_GetQualityResultByBranchAsync_NullServerUrl_ThrowsArgumentNullException()
    {
        // Arrange
        using var client = new SonarQubeClient(new HttpClient(), false);

        // Act & Assert
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(
            async () => await client.GetQualityResultByBranchAsync(null!, "my-project"));
    }

    /// <summary>
    ///     Test that whitespace server URL throws ArgumentException
    /// </summary>
    [TestMethod]
    public async Task SonarQubeClient_GetQualityResultByBranchAsync_WhitespaceServerUrl_ThrowsArgumentException()
    {
        // Arrange
        using var client = new SonarQubeClient(new HttpClient(), false);

        // Act & Assert
        await Assert.ThrowsExactlyAsync<ArgumentException>(
            async () => await client.GetQualityResultByBranchAsync("   ", "my-project"));
    }

    /// <summary>
    ///     Test that null project key throws ArgumentNullException
    /// </summary>
    [TestMethod]
    public async Task SonarQubeClient_GetQualityResultByBranchAsync_NullProjectKey_ThrowsArgumentNullException()
    {
        // Arrange
        using var client = new SonarQubeClient(new HttpClient(), false);

        // Act & Assert
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(
            async () => await client.GetQualityResultByBranchAsync("https://sonar.example.com", null!));
    }

    /// <summary>
    ///     Test that whitespace project key throws ArgumentException
    /// </summary>
    [TestMethod]
    public async Task SonarQubeClient_GetQualityResultByBranchAsync_WhitespaceProjectKey_ThrowsArgumentException()
    {
        // Arrange
        using var client = new SonarQubeClient(new HttpClient(), false);

        // Act & Assert
        await Assert.ThrowsExactlyAsync<ArgumentException>(
            async () => await client.GetQualityResultByBranchAsync("https://sonar.example.com", "   "));
    }
}

