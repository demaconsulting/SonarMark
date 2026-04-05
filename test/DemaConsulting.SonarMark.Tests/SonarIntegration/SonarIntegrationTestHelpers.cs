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

namespace DemaConsulting.SonarMark.Tests.SonarIntegration;

/// <summary>
///     Shared test helpers for SonarIntegration tests.
/// </summary>
internal static class SonarIntegrationTestHelpers
{
    /// <summary>
    ///     Creates an <see cref="HttpResponseMessage"/> with HTTP 200 OK and JSON body content.
    /// </summary>
    /// <param name="json">JSON string to use as response body.</param>
    /// <returns>Configured HTTP response message.</returns>
    public static HttpResponseMessage OkJson(string json) =>
        new(HttpStatusCode.OK)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        };
}

/// <summary>
///     Test double for <see cref="HttpMessageHandler"/> that serves pre-queued responses in order.
/// </summary>
internal sealed class MockHttpMessageHandler : HttpMessageHandler
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
