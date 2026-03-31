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
}
