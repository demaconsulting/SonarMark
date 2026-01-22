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

        // Create mock HTTP client factory
        var mockFactory = (string? _) => new SonarQubeClient(CreateMockHttpClient(), false);

        // Run core functionality tests
        RunQualityGateRetrievalTest(context, testResults, mockFactory);
        RunIssuesRetrievalTest(context, testResults, mockFactory);
        RunHotSpotsRetrievalTest(context, testResults, mockFactory);
        RunMarkdownReportGenerationTest(context, testResults, mockFactory);

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
    ///     Runs a test for quality gate retrieval functionality.
    /// </summary>
    /// <param name="context">The context for output.</param>
    /// <param name="testResults">The test results collection.</param>
    /// <param name="mockFactory">The mock HTTP client factory.</param>
    private static void RunQualityGateRetrievalTest(
        Context context,
        DemaConsulting.TestResults.TestResults testResults,
        Func<string?, SonarQubeClient> mockFactory)
    {
        RunValidationTest(
            context,
            testResults,
            mockFactory,
            "SonarMark_QualityGateRetrieval",
            "Quality Gate Retrieval Test",
            null,
            (logContent, _) =>
            {
                if (logContent.Contains("Quality Gate Status: ERROR") &&
                    logContent.Contains("Issues: 2") &&
                    logContent.Contains("Hot-Spots: 1"))
                {
                    return null;
                }

                return "Expected output not found in log";
            });
    }

    /// <summary>
    ///     Runs a test for issues retrieval functionality.
    /// </summary>
    /// <param name="context">The context for output.</param>
    /// <param name="testResults">The test results collection.</param>
    /// <param name="mockFactory">The mock HTTP client factory.</param>
    private static void RunIssuesRetrievalTest(
        Context context,
        DemaConsulting.TestResults.TestResults testResults,
        Func<string?, SonarQubeClient> mockFactory)
    {
        RunValidationTest(
            context,
            testResults,
            mockFactory,
            "SonarMark_IssuesRetrieval",
            "Issues Retrieval Test",
            null,
            (logContent, _) =>
            {
                if (logContent.Contains("Issues: 2"))
                {
                    return null;
                }

                return "Expected issues count not found in log";
            });
    }

    /// <summary>
    ///     Runs a test for security hot-spots retrieval functionality.
    /// </summary>
    /// <param name="context">The context for output.</param>
    /// <param name="testResults">The test results collection.</param>
    /// <param name="mockFactory">The mock HTTP client factory.</param>
    private static void RunHotSpotsRetrievalTest(
        Context context,
        DemaConsulting.TestResults.TestResults testResults,
        Func<string?, SonarQubeClient> mockFactory)
    {
        RunValidationTest(
            context,
            testResults,
            mockFactory,
            "SonarMark_HotSpotsRetrieval",
            "Hot-Spots Retrieval Test",
            null,
            (logContent, _) =>
            {
                if (logContent.Contains("Hot-Spots: 1"))
                {
                    return null;
                }

                return "Expected hot-spots count not found in log";
            });
    }

    /// <summary>
    ///     Runs a test for markdown report generation functionality.
    /// </summary>
    /// <param name="context">The context for output.</param>
    /// <param name="testResults">The test results collection.</param>
    /// <param name="mockFactory">The mock HTTP client factory.</param>
    private static void RunMarkdownReportGenerationTest(
        Context context,
        DemaConsulting.TestResults.TestResults testResults,
        Func<string?, SonarQubeClient> mockFactory)
    {
        RunValidationTest(
            context,
            testResults,
            mockFactory,
            "SonarMark_MarkdownReportGeneration",
            "Markdown Report Generation Test",
            "quality-report.md",
            (logContent, reportContent) =>
            {
                if (reportContent == null)
                {
                    return "Report file not created";
                }

                if (reportContent.Contains("Mock SonarMark Project") &&
                    reportContent.Contains("**Quality Gate Status:** ERROR") &&
                    reportContent.Contains("Found 2 issues") &&
                    reportContent.Contains("Found 1 security hot-spot"))
                {
                    return null;
                }

                return "Report file missing expected content";
            });
    }

    /// <summary>
    ///     Runs a validation test with common test execution logic.
    /// </summary>
    /// <param name="context">The context for output.</param>
    /// <param name="testResults">The test results collection.</param>
    /// <param name="mockFactory">The mock HTTP client factory.</param>
    /// <param name="testName">The name of the test.</param>
    /// <param name="displayName">The display name for console output.</param>
    /// <param name="reportFileName">Optional report file name to generate.</param>
    /// <param name="validator">Function to validate test results. Returns null on success or error message on failure.</param>
    private static void RunValidationTest(
        Context context,
        DemaConsulting.TestResults.TestResults testResults,
        Func<string?, SonarQubeClient> mockFactory,
        string testName,
        string displayName,
        string? reportFileName,
        Func<string, string?, string?> validator)
    {
        var startTime = DateTime.UtcNow;
        var test = CreateTestResult(testName);

        try
        {
            using var tempDir = new TemporaryDirectory();
            var logFile = Path.Combine(tempDir.DirectoryPath, $"{testName}.log");
            var reportFile = reportFileName != null ? Path.Combine(tempDir.DirectoryPath, reportFileName) : null;

            // Build command line arguments
            var args = new List<string>
            {
                "--silent",
                "--log", logFile,
                "--server", MockServerUrl,
                "--project-key", MockProjectKey
            };

            if (reportFile != null)
            {
                args.Add("--report");
                args.Add(reportFile);
            }

            // Run the program
            int exitCode;
            using (var testContext = Context.Create([.. args], mockFactory))
            {
                Program.Run(testContext);
                exitCode = testContext.ExitCode;
            }

            // Check if execution succeeded
            if (exitCode == 0)
            {
                // Read log and report contents
                var logContent = File.ReadAllText(logFile);
                var reportContent = reportFile != null && File.Exists(reportFile)
                    ? File.ReadAllText(reportFile)
                    : null;

                // Validate the results
                var errorMessage = validator(logContent, reportContent);

                if (errorMessage == null)
                {
                    test.Outcome = DemaConsulting.TestResults.TestOutcome.Passed;
                    context.WriteLine($"✓ {displayName} - PASSED");
                }
                else
                {
                    test.Outcome = DemaConsulting.TestResults.TestOutcome.Failed;
                    test.ErrorMessage = errorMessage;
                    context.WriteError($"✗ {displayName} - FAILED: {errorMessage}");
                }
            }
            else
            {
                test.Outcome = DemaConsulting.TestResults.TestOutcome.Failed;
                test.ErrorMessage = $"Program exited with code {exitCode}";
                context.WriteError($"✗ {displayName} - FAILED: Exit code {exitCode}");
            }
        }
        catch (Exception ex)
        {
            HandleTestException(test, context, displayName, ex);
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
                                "key": "hot-spot-1",
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

    /// <summary>
    ///     Represents a temporary directory that is automatically deleted when disposed.
    /// </summary>
    private sealed class TemporaryDirectory : IDisposable
    {
        /// <summary>
        ///     Gets the path to the temporary directory.
        /// </summary>
        public string DirectoryPath { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TemporaryDirectory"/> class.
        /// </summary>
        public TemporaryDirectory()
        {
            DirectoryPath = Path.Combine(Path.GetTempPath(), $"sonarmark_validation_{Guid.NewGuid()}");
            
            try
            {
                Directory.CreateDirectory(DirectoryPath);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException)
            {
                throw new InvalidOperationException($"Failed to create temporary directory: {ex.Message}", ex);
            }
        }

        /// <summary>
        ///     Deletes the temporary directory and all its contents.
        /// </summary>
        public void Dispose()
        {
            try
            {
                if (Directory.Exists(DirectoryPath))
                {
                    Directory.Delete(DirectoryPath, true);
                }
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                // Ignore cleanup errors during disposal
            }
        }
    }
}
