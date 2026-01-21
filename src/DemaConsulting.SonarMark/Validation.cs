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
using System.Runtime.InteropServices;
using System.Text;
using DemaConsulting.TestResults.IO;

namespace DemaConsulting.SonarMark;

/// <summary>
///     Provides self-validation functionality for the SonarMark tool.
/// </summary>
internal static class Validation
{
    /// <summary>
    ///     Special project key used for mock validation data.
    /// </summary>
    private const string MockProjectKey = "SonarMarkMockProject";

    /// <summary>
    ///     Mock server URL used for validation testing.
    /// </summary>
#pragma warning disable S1075 // URIs should not be hardcoded (this is intentional for mock testing)
    private const string MockServerUrl = "https://mock.sonarqube.example";
#pragma warning restore S1075

    /// <summary>
    ///     Runs self-validation tests and optionally writes results to a file.
    /// </summary>
    /// <param name="context">The context containing command line arguments and program state.</param>
    public static void Run(Context context)
    {
        // Print validation header
        PrintValidationHeader(context);

        // Create test results collection
        var testResults = new DemaConsulting.TestResults.TestResults
        {
            Name = "SonarMark Self-Validation"
        };

        // Run core functionality tests
        RunQualityGateStatusTest(context, testResults);
        RunIssuesRetrievalTest(context, testResults);
        RunHotSpotsRetrievalTest(context, testResults);
        RunMarkdownReportGenerationTest(context, testResults);

        // Calculate totals
        var totalTests = testResults.Results.Count;
        var passedTests = testResults.Results.Count(t => t.Outcome == DemaConsulting.TestResults.TestOutcome.Passed);
        var failedTests = testResults.Results.Count(t => t.Outcome == DemaConsulting.TestResults.TestOutcome.Failed);

        // Print summary
        context.WriteLine("");
        context.WriteLine($"Total Tests: {totalTests}");
        context.WriteLine($"Passed: {passedTests}");
        if (failedTests > 0)
        {
            context.WriteError($"Failed: {failedTests}");
        }
        else
        {
            context.WriteLine($"Failed: {failedTests}");
        }

        // Write results file if requested
        if (context.ResultsFile != null)
        {
            WriteResultsFile(context, testResults);
        }
    }

    /// <summary>
    ///     Prints the validation header with system information.
    /// </summary>
    /// <param name="context">The context for output.</param>
    private static void PrintValidationHeader(Context context)
    {
        context.WriteLine("# DEMA Consulting SonarMark");
        context.WriteLine("");
        context.WriteLine("| Information         | Value                                              |");
        context.WriteLine("| :------------------ | :------------------------------------------------- |");
        context.WriteLine($"| SonarMark Version   | {Program.Version,-50} |");
        context.WriteLine($"| Machine Name        | {Environment.MachineName,-50} |");
        context.WriteLine($"| OS Version          | {RuntimeInformation.OSDescription,-50} |");
        context.WriteLine($"| DotNet Runtime      | {RuntimeInformation.FrameworkDescription,-50} |");
        context.WriteLine($"| Time Stamp          | {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC{"",-29} |");
        context.WriteLine("");
    }

    /// <summary>
    ///     Runs a test for quality gate status retrieval.
    /// </summary>
    /// <param name="context">The context for output.</param>
    /// <param name="testResults">The test results collection.</param>
    private static void RunQualityGateStatusTest(Context context, DemaConsulting.TestResults.TestResults testResults)
    {
        var startTime = DateTime.UtcNow;
        var test = CreateTestResult("QualityGateStatusRetrieval");

        try
        {
            // Create a mock HTTP client
            using var mockClient = CreateMockHttpClient();
            using var client = new SonarQubeClient(mockClient, false);

            // Fetch quality results with mock data
            var result = client.GetQualityResultByBranchAsync(
                MockServerUrl,
                MockProjectKey,
                null).GetAwaiter().GetResult();

            // Verify the results
            if (result.QualityGateStatus == "ERROR" &&
                result.Conditions.Count == 2 &&
                result.ProjectName == "Mock SonarMark Project")
            {
                test.Outcome = DemaConsulting.TestResults.TestOutcome.Passed;
                context.WriteLine("✓ Quality Gate Status Retrieval Test - PASSED");
            }
            else
            {
                test.Outcome = DemaConsulting.TestResults.TestOutcome.Failed;
                test.ErrorMessage = "Quality gate status or conditions mismatch";
                context.WriteError("✗ Quality Gate Status Retrieval Test - FAILED: Quality gate status or conditions mismatch");
            }
        }
        catch (Exception ex)
        {
            HandleTestException(test, context, "Quality Gate Status Retrieval Test", ex);
        }

        FinalizeTestResult(test, startTime, testResults);
    }

    /// <summary>
    ///     Runs a test for issues retrieval.
    /// </summary>
    /// <param name="context">The context for output.</param>
    /// <param name="testResults">The test results collection.</param>
    private static void RunIssuesRetrievalTest(Context context, DemaConsulting.TestResults.TestResults testResults)
    {
        var startTime = DateTime.UtcNow;
        var test = CreateTestResult("IssuesRetrieval");

        try
        {
            // Create a mock HTTP client
            using var mockClient = CreateMockHttpClient();
            using var client = new SonarQubeClient(mockClient, false);

            // Fetch quality results with mock data
            var result = client.GetQualityResultByBranchAsync(
                MockServerUrl,
                MockProjectKey,
                null).GetAwaiter().GetResult();

            // Verify the results
            if (result.Issues.Count == 2 &&
                result.Issues[0].Rule == "csharpsquid:S1234" &&
                result.Issues[1].Severity == "MINOR")
            {
                test.Outcome = DemaConsulting.TestResults.TestOutcome.Passed;
                context.WriteLine("✓ Issues Retrieval Test - PASSED");
            }
            else
            {
                test.Outcome = DemaConsulting.TestResults.TestOutcome.Failed;
                test.ErrorMessage = "Issues count or content mismatch";
                context.WriteError("✗ Issues Retrieval Test - FAILED: Issues count or content mismatch");
            }
        }
        catch (Exception ex)
        {
            HandleTestException(test, context, "Issues Retrieval Test", ex);
        }

        FinalizeTestResult(test, startTime, testResults);
    }

    /// <summary>
    ///     Runs a test for security hot-spots retrieval.
    /// </summary>
    /// <param name="context">The context for output.</param>
    /// <param name="testResults">The test results collection.</param>
    private static void RunHotSpotsRetrievalTest(Context context, DemaConsulting.TestResults.TestResults testResults)
    {
        var startTime = DateTime.UtcNow;
        var test = CreateTestResult("HotSpotsRetrieval");

        try
        {
            // Create a mock HTTP client
            using var mockClient = CreateMockHttpClient();
            using var client = new SonarQubeClient(mockClient, false);

            // Fetch quality results with mock data
            var result = client.GetQualityResultByBranchAsync(
                MockServerUrl,
                MockProjectKey,
                null).GetAwaiter().GetResult();

            // Verify the results
            if (result.HotSpots.Count == 1 &&
                result.HotSpots[0].SecurityCategory == "sql-injection" &&
                result.HotSpots[0].VulnerabilityProbability == "HIGH")
            {
                test.Outcome = DemaConsulting.TestResults.TestOutcome.Passed;
                context.WriteLine("✓ Hot-Spots Retrieval Test - PASSED");
            }
            else
            {
                test.Outcome = DemaConsulting.TestResults.TestOutcome.Failed;
                test.ErrorMessage = "Hot-spots count or content mismatch";
                context.WriteError("✗ Hot-Spots Retrieval Test - FAILED: Hot-spots count or content mismatch");
            }
        }
        catch (Exception ex)
        {
            HandleTestException(test, context, "Hot-Spots Retrieval Test", ex);
        }

        FinalizeTestResult(test, startTime, testResults);
    }

    /// <summary>
    ///     Runs a test for markdown report generation.
    /// </summary>
    /// <param name="context">The context for output.</param>
    /// <param name="testResults">The test results collection.</param>
    private static void RunMarkdownReportGenerationTest(Context context, DemaConsulting.TestResults.TestResults testResults)
    {
        var startTime = DateTime.UtcNow;
        var test = CreateTestResult("MarkdownReportGeneration");

        try
        {
            // Create a mock HTTP client
            using var mockClient = CreateMockHttpClient();
            using var client = new SonarQubeClient(mockClient, false);

            // Fetch quality results with mock data
            var result = client.GetQualityResultByBranchAsync(
                MockServerUrl,
                MockProjectKey,
                null).GetAwaiter().GetResult();

            // Generate markdown report
            var markdown = result.ToMarkdown(1);

            // Verify the report contains expected content
            var hasMockProject = markdown.Contains("Mock SonarMark Project");
            var hasQualityGate = markdown.Contains("**Quality Gate Status:** ERROR");
            var hasIssues = markdown.Contains("Found 2 issues");
            var hasHotSpots = markdown.Contains("Found 1 security hot-spot");

            if (hasMockProject && hasQualityGate && hasIssues && hasHotSpots)
            {
                test.Outcome = DemaConsulting.TestResults.TestOutcome.Passed;
                context.WriteLine("✓ Markdown Report Generation Test - PASSED");
            }
            else
            {
                test.Outcome = DemaConsulting.TestResults.TestOutcome.Failed;
                test.ErrorMessage = "Markdown report missing expected content";
                context.WriteError("✗ Markdown Report Generation Test - FAILED: Markdown report missing expected content");
            }
        }
        catch (Exception ex)
        {
            HandleTestException(test, context, "Markdown Report Generation Test", ex);
        }

        FinalizeTestResult(test, startTime, testResults);
    }

    /// <summary>
    ///     Creates a mock HttpClient that returns predefined responses for validation.
    /// </summary>
    /// <returns>Mock HttpClient for testing.</returns>
    private static HttpClient CreateMockHttpClient()
    {
        var handler = new MockHttpMessageHandler();
        return new HttpClient(handler);
    }

    /// <summary>
    ///     Writes test results to a file in TRX or JUnit format.
    /// </summary>
    /// <param name="context">The context for output.</param>
    /// <param name="testResults">The test results to write.</param>
    private static void WriteResultsFile(Context context, DemaConsulting.TestResults.TestResults testResults)
    {
        if (context.ResultsFile == null)
        {
            return;
        }

        try
        {
            var extension = Path.GetExtension(context.ResultsFile).ToLowerInvariant();
            string content;

            if (extension == ".trx")
            {
                content = TrxSerializer.Serialize(testResults);
            }
            else if (extension == ".xml")
            {
                // Assume JUnit format for .xml extension
                content = JUnitSerializer.Serialize(testResults);
            }
            else
            {
                context.WriteError($"Error: Unsupported results file format '{extension}'. Use .trx or .xml extension.");
                return;
            }

            File.WriteAllText(context.ResultsFile, content);
            context.WriteLine($"Results written to {context.ResultsFile}");
        }
        catch (Exception ex)
        {
            context.WriteError($"Error: Failed to write results file: {ex.Message}");
        }
    }

    /// <summary>
    ///     Creates a new test result object with common properties.
    /// </summary>
    /// <param name="testName">The name of the test.</param>
    /// <returns>A new test result object.</returns>
    private static DemaConsulting.TestResults.TestResult CreateTestResult(string testName)
    {
        return new DemaConsulting.TestResults.TestResult
        {
            Name = testName,
            ClassName = "Validation",
            CodeBase = "SonarMark"
        };
    }

    /// <summary>
    ///     Finalizes a test result by setting its duration and adding it to the collection.
    /// </summary>
    /// <param name="test">The test result to finalize.</param>
    /// <param name="startTime">The start time of the test.</param>
    /// <param name="testResults">The test results collection to add to.</param>
    private static void FinalizeTestResult(
        DemaConsulting.TestResults.TestResult test,
        DateTime startTime,
        DemaConsulting.TestResults.TestResults testResults)
    {
        test.Duration = DateTime.UtcNow - startTime;
        testResults.Results.Add(test);
    }

    /// <summary>
    ///     Handles test exceptions by setting failure information and logging the error.
    /// </summary>
    /// <param name="test">The test result to update.</param>
    /// <param name="context">The context for output.</param>
    /// <param name="testName">The name of the test for error messages.</param>
    /// <param name="ex">The exception that occurred.</param>
    private static void HandleTestException(
        DemaConsulting.TestResults.TestResult test,
        Context context,
        string testName,
        Exception ex)
    {
        test.Outcome = DemaConsulting.TestResults.TestOutcome.Failed;
        test.ErrorMessage = $"Exception: {ex.Message}";
        context.WriteError($"✗ {testName} - FAILED: {ex.Message}");
    }

    /// <summary>
    ///     Mock HTTP message handler that returns predefined responses for validation testing.
    /// </summary>
    private sealed class MockHttpMessageHandler : HttpMessageHandler
    {
        /// <summary>
        ///     Sends an HTTP request and returns a mock response based on the URL.
        /// </summary>
        /// <param name="request">The HTTP request message.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>HTTP response message.</returns>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var requestUri = request.RequestUri?.ToString() ?? string.Empty;

            // Mock project info response
            if (requestUri.Contains("/api/components/show"))
            {
                var json = """
                    {
                        "component": {
                            "key": "SonarMarkMockProject",
                            "name": "Mock SonarMark Project"
                        }
                    }
                    """;
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                });
            }

            // Mock quality gate status response
            if (requestUri.Contains("/api/qualitygates/project_status"))
            {
                var json = """
                    {
                        "projectStatus": {
                            "status": "ERROR",
                            "conditions": [
                                {
                                    "metricKey": "new_coverage",
                                    "comparator": "LT",
                                    "errorThreshold": "80",
                                    "actualValue": "65.5",
                                    "status": "ERROR"
                                },
                                {
                                    "metricKey": "new_bugs",
                                    "comparator": "GT",
                                    "errorThreshold": "0",
                                    "actualValue": "3",
                                    "status": "ERROR"
                                }
                            ]
                        }
                    }
                    """;
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                });
            }

            // Mock issues response
            if (requestUri.Contains("/api/issues/search"))
            {
                var json = """
                    {
                        "issues": [
                            {
                                "key": "issue1",
                                "rule": "csharpsquid:S1234",
                                "severity": "MAJOR",
                                "component": "SonarMarkMockProject:src/Program.cs",
                                "line": 42,
                                "message": "Remove this unused variable",
                                "type": "CODE_SMELL"
                            },
                            {
                                "key": "issue2",
                                "rule": "csharpsquid:S5678",
                                "severity": "MINOR",
                                "component": "SonarMarkMockProject:src/Helper.cs",
                                "line": 15,
                                "message": "Refactor this method to reduce complexity",
                                "type": "CODE_SMELL"
                            }
                        ]
                    }
                    """;
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                });
            }

            // Mock hot-spots response
            if (requestUri.Contains("/api/hotspots/search"))
            {
                var json = """
                    {
                        "hotspots": [
                            {
                                "key": "hotspot1",
                                "component": "SonarMarkMockProject:src/Database.cs",
                                "line": 88,
                                "message": "Make sure using this SQL query is safe",
                                "securityCategory": "sql-injection",
                                "vulnerabilityProbability": "HIGH"
                            }
                        ]
                    }
                    """;
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                });
            }

            // Mock metrics response
            if (requestUri.Contains("/api/metrics/search"))
            {
                var json = """
                    {
                        "metrics": [
                            {
                                "key": "new_coverage",
                                "name": "Coverage on New Code"
                            },
                            {
                                "key": "new_bugs",
                                "name": "New Bugs"
                            }
                        ]
                    }
                    """;
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                });
            }

            // Default response for unknown URLs
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
        }
    }
}
