using System.Net;
using System.Text;
using DemaConsulting.SonarMark.Cli;
using DemaConsulting.SonarMark.SonarIntegration;

namespace DemaConsulting.SonarMark.Tests;

/// <summary>
///     Tests for Program class
/// </summary>
[TestClass]
public class ProgramTests
{
    /// <summary>
    ///     Test that Version property is not empty
    /// </summary>
    [TestMethod]
    public void Program_Version_WhenAccessed_ReturnsNonEmptyString()
    {
        // Arrange - no setup needed

        // Act - access the Version property
        var version = Program.Version;

        // Assert - verify version is not empty
        // This test proves that the Program.Version property returns a valid non-empty version string
        Assert.IsFalse(string.IsNullOrWhiteSpace(version));
    }

    /// <summary>
    ///     Test that Run method with version flag outputs only version
    /// </summary>
    [TestMethod]
    public void Program_Run_WithVersionFlag_OutputsVersionString()
    {
        // Arrange - capture console output
        var originalOut = Console.Out;
        using var output = new StringWriter();
        Console.SetOut(output);

        try
        {
            using var context = Context.Create(["--version"]);

            // Act - run the program with version flag
            Program.Run(context);

            // Assert - verify only version is output
            // This test proves that --version flag outputs the version string and nothing else
            Assert.AreEqual(Program.Version + Environment.NewLine, output.ToString());
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test that Run method with help flag outputs banner and help
    /// </summary>
    [TestMethod]
    public void Program_Run_WithHelpFlag_OutputsBannerAndHelpText()
    {
        // Arrange - capture console output
        var originalOut = Console.Out;
        using var output = new StringWriter();
        Console.SetOut(output);

        try
        {
            using var context = Context.Create(["--help"]);

            // Act - run the program with help flag
            Program.Run(context);

            // Assert - verify banner and help text are present
            // This test proves that --help flag outputs the banner and complete help information
            var outputText = output.ToString();
            Assert.Contains("SonarMark version", outputText);
            Assert.Contains("Usage: sonarmark", outputText);
            Assert.Contains("Options:", outputText);
            Assert.AreEqual(0, context.ExitCode);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test that Run method with validate flag runs validation successfully
    /// </summary>
    [TestMethod]
    public void Program_Run_WithValidateFlag_RunsValidationSuccessfully()
    {
        // Arrange - create context with validate flag
        using var context = Context.Create(["--validate"]);

        // Act - run the program with validate flag
        Program.Run(context);

        // Assert - verify validation completes successfully
        // This test proves that --validate flag triggers self-validation and completes successfully
        Assert.AreEqual(0, context.ExitCode);
    }

    /// <summary>
    ///     Test that Run method with no flags outputs banner and error for missing server
    /// </summary>
    [TestMethod]
    public void Program_Run_WithNoArguments_OutputsBannerAndRequiresServerError()
    {
        // Arrange - capture console output (stdout for banner, stderr for error messages)
        var originalOut = Console.Out;
        var originalError = Console.Error;
        using var output = new StringWriter();
        using var errorOutput = new StringWriter();
        Console.SetOut(output);
        Console.SetError(errorOutput);

        try
        {
            using var context = Context.Create([]);

            // Act - run the program with no arguments
            Program.Run(context);

            // Assert - verify banner is shown on stdout and error about missing --server parameter on stderr
            // This test proves that running without required parameters shows appropriate error
            Assert.Contains("SonarMark version", output.ToString());
            Assert.Contains("--server parameter is required", errorOutput.ToString());
            Assert.AreEqual(1, context.ExitCode);
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetError(originalError);
        }
    }

    /// <summary>
    ///     Test that Run method with server but no project key outputs error
    /// </summary>
    [TestMethod]
    public void Program_Run_WithServerButNoProjectKey_OutputsProjectKeyRequiredError()
    {
        // Arrange - capture console error output
        var originalError = Console.Error;
        using var errorOutput = new StringWriter();
        Console.SetError(errorOutput);

        try
        {
            using var context = Context.Create(["--server", "https://sonarcloud.io"]);

            // Act - run the program with server but no project key
            Program.Run(context);

            // Assert - verify error about missing --project-key parameter
            // This test proves that --server requires --project-key to also be specified
            Assert.Contains("--project-key parameter is required", errorOutput.ToString());
            Assert.AreEqual(1, context.ExitCode);
        }
        finally
        {
            Console.SetError(originalError);
        }
    }

    /// <summary>
    ///     Test that Run method with silent flag suppresses banner
    /// </summary>
    [TestMethod]
    public void Program_Run_WithSilentFlag_SuppressesBannerOutput()
    {
        // Arrange - capture console output
        var originalOut = Console.Out;
        using var output = new StringWriter();
        Console.SetOut(output);

        try
        {
            using var context = Context.Create(["--silent"]);

            // Act - run the program with silent flag
            Program.Run(context);

            // Assert - verify banner is not shown
            // This test proves that --silent flag suppresses the banner output
            var outputText = output.ToString();
            Assert.DoesNotContain("SonarMark version", outputText);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test that Run method with enforce flag and failing quality gate returns non-zero exit code
    /// </summary>
    [TestMethod]
    public void Program_Run_WithEnforceFlagAndFailingQualityGate_ReturnsNonZeroExitCode()
    {
        // Arrange - create mock HTTP client factory that returns failing quality gate (ERROR status)
        var mockFactory = (string? _) => new SonarQubeClient(CreateMockFailingQualityGateHttpClient(), false);
        using var context = Context.Create(
            ["--server", "https://mock.sonarqube.example", "--project-key", "test-project", "--enforce"],
            mockFactory);

        // Act - run the program with enforce flag and a server that reports quality gate ERROR
        Program.Run(context);

        // Assert - verify exit code is 1 (non-zero) because quality gate failed and --enforce was set
        // This test proves that Program returns a non-zero exit code when quality gate fails in enforcement mode
        Assert.AreEqual(1, context.ExitCode);
    }

    /// <summary>
    ///     Test that Run method with --report flag writes a markdown file
    /// </summary>
    [TestMethod]
    public void Program_Run_WithReportFile_WritesMarkdownToFile()
    {
        // Arrange - create a temporary report file path and a mock HTTP client factory
        var reportPath = Path.Combine(Path.GetTempPath(), $"sonarmark_report_{Guid.NewGuid()}.md");
        var mockFactory = (string? _) => new SonarQubeClient(CreateMockFailingQualityGateHttpClient(), false);

        try
        {
            using var context = Context.Create(
                ["--server", "https://mock.sonarqube.example", "--project-key", "test-project", "--report", reportPath],
                mockFactory);

            // Act - run the program with --report flag; it should write a markdown file
            Program.Run(context);

            // Assert - verify the report file was created and contains markdown content
            // This test proves that --report writes a markdown file to the specified path
            Assert.IsTrue(File.Exists(reportPath), $"Expected report file at {reportPath}");
            var content = File.ReadAllText(reportPath);
            Assert.IsFalse(string.IsNullOrWhiteSpace(content));
            Assert.Contains("test-project", content);
        }
        finally
        {
            // Clean up temp file
            if (File.Exists(reportPath))
            {
                File.Delete(reportPath);
            }
        }
    }

    /// <summary>
    ///     Test that Run method with --report-depth flag writes a markdown file with the correct heading level.
    /// </summary>
    [TestMethod]
    public void Program_Run_WithReportDepth_WritesReportWithDepthHeadings()
    {
        // Arrange - create a temporary report file path and a mock HTTP client factory
        var reportPath = Path.Combine(Path.GetTempPath(), $"sonarmark_report_{Guid.NewGuid()}.md");
        var mockFactory = (string? _) => new SonarQubeClient(CreateMockFailingQualityGateHttpClient(), false);

        try
        {
            using var context = Context.Create(
                [
                    "--server", "https://mock.sonarqube.example",
                    "--project-key", "test-project",
                    "--report", reportPath,
                    "--report-depth", "2"
                ],
                mockFactory);

            // Act - run the program with --report-depth 2; report headings must start at level 2
            Program.Run(context);

            // Assert - verify the report file uses level-2 headings (##) not level-1 headings (#)
            // This test proves that --report-depth controls the markdown heading level in the output
            Assert.IsTrue(File.Exists(reportPath), $"Expected report file at {reportPath}");
            var content = File.ReadAllText(reportPath);
            Assert.Contains("## ", content);
            Assert.IsFalse(content.StartsWith("# "), "Report must not start with a level-1 heading when --report-depth 2 is used");
        }
        finally
        {
            // Clean up temp file
            if (File.Exists(reportPath))
            {
                File.Delete(reportPath);
            }
        }
    }

    /// <summary>
    ///     Creates a mock HttpClient that returns quality gate status ERROR for testing enforcement behavior.
    /// </summary>
    /// <returns>Mock HttpClient for enforcement testing.</returns>
    private static HttpClient CreateMockFailingQualityGateHttpClient()
    {
        return new HttpClient(new FailingQualityGateMockHandler());
    }

    /// <summary>
    ///     Mock HTTP handler that returns a failing (ERROR) quality gate status.
    /// </summary>
    private sealed class FailingQualityGateMockHandler : HttpMessageHandler
    {
        /// <inheritdoc/>
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var requestUri = request.RequestUri?.ToString() ?? string.Empty;

            if (requestUri.Contains("/api/components/show"))
            {
                var json = """{"component": {"key": "test-project", "name": "Test Project"}}""";
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                });
            }

            if (requestUri.Contains("/api/qualitygates/project_status"))
            {
                var json = """{"projectStatus": {"status": "ERROR", "conditions": []}}""";
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                });
            }

            if (requestUri.Contains("/api/issues/search"))
            {
                var json = """{"issues": []}""";
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                });
            }

            if (requestUri.Contains("/api/hotspots/search"))
            {
                var json = """{"hotspots": []}""";
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                });
            }

            if (requestUri.Contains("/api/metrics/search"))
            {
                var json = """{"metrics": []}""";
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                });
            }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
        }
    }
}
