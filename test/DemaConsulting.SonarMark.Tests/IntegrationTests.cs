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

using DemaConsulting.HelpCards.Catalog;
using DemaConsulting.HelpCards.Testing;
using DemaConsulting.SonarMark.Cli;
using Xunit;

namespace DemaConsulting.SonarMark.Tests;

/// <summary>
///     Integration tests that run the SonarMark application through dotnet
/// </summary>
public class IntegrationTests
{
    private readonly string _dllPath;

    /// <summary>
    ///     Initialize test by locating the SonarMark DLL
    /// </summary>
    public IntegrationTests()
    {
        // The DLL should be in the same directory as the test assembly
        // because the test project references the main project
        var baseDir = AppContext.BaseDirectory;
        _dllPath = Path.Combine(baseDir, "DemaConsulting.SonarMark.dll");

        Assert.True(File.Exists(_dllPath), $"Could not find SonarMark DLL at {_dllPath}");
    }

    /// <summary>
    ///     Test that version flag outputs version information
    /// </summary>
    [Fact]
    public void IntegrationTest_VersionFlag_OutputsVersion()
    {
        // Run the application with --version flag
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--version");

        // Verify success
        Assert.Equal(0, exitCode);

        // Verify version is output
        Assert.False(string.IsNullOrWhiteSpace(output));
        Assert.DoesNotContain("Error", output);
    }

    /// <summary>
    ///     Test that help flag outputs usage information
    /// </summary>
    [Fact]
    public void IntegrationTest_HelpFlag_OutputsUsageInformation()
    {
        // Run the application with --help flag
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--help");

        // Verify success
        Assert.Equal(0, exitCode);

        // Verify usage information
        Assert.Contains("Usage: sonarmark", output);
        Assert.Contains("Options:", output);
        Assert.Contains("--version", output);
        Assert.Contains("--help", output);
        Assert.Contains("--server", output);
        Assert.Contains("--project-key", output);
    }

    /// <summary>
    ///     Test that validate flag outputs not implemented message
    /// </summary>
    [Fact]
    public void IntegrationTest_ValidateFlag_OutputsHeaderAndSummary()
    {
        // Run the application with --validate flag
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--validate");

        // Verify success
        Assert.Equal(0, exitCode);

        // Verify validation runs successfully
        Assert.Contains("# DEMA Consulting SonarMark", output);
        Assert.Contains("Total Tests:", output);
    }

    /// <summary>
    ///     Test that missing server parameter shows error
    /// </summary>
    [Fact]
    public void IntegrationTest_MissingServerParameter_ShowsError()
    {
        // Run the application without required parameters
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath);

        // Verify error exit code
        Assert.Equal(1, exitCode);

        // Verify error message
        Assert.Contains("--server parameter is required", output);
    }

    /// <summary>
    ///     Test that missing project-key parameter shows error
    /// </summary>
    [Fact]
    public void IntegrationTest_MissingProjectKeyParameter_ShowsError()
    {
        // Run the application with server but without project-key
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--server", "https://sonarcloud.io");

        // Verify error exit code
        Assert.Equal(1, exitCode);

        // Verify error message
        Assert.Contains("--project-key parameter is required", output);
    }

    /// <summary>
    ///     Test that silent flag suppresses output
    /// </summary>
    [Fact]
    public void IntegrationTest_SilentFlag_SuppressesOutput()
    {
        // Run the application with --silent flag
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--silent");

        // Verify error exit code (missing required parameters)
        Assert.Equal(1, exitCode);

        // Verify no banner in output
        Assert.DoesNotContain("SonarMark version", output);
    }

    /// <summary>
    ///     Test that invalid argument shows error
    /// </summary>
    [Fact]
    public void IntegrationTest_InvalidArgument_ShowsError()
    {
        // Run the application with invalid argument
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--invalid-argument");

        // Verify error exit code
        Assert.Equal(1, exitCode);

        // Verify error message
        Assert.Contains("Error:", output);
        Assert.Contains("Unsupported argument", output);
    }

    /// <summary>
    ///     Test that report-depth without value shows error
    /// </summary>
    [Fact]
    public void IntegrationTest_ReportDepthWithoutValue_ShowsError()
    {
        // Run the application with --report-depth but no value
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--report-depth");

        // Verify error exit code
        Assert.Equal(1, exitCode);

        // Verify error message
        Assert.Contains("Error:", output);
        Assert.Contains("--report-depth requires a depth argument", output);
    }

    /// <summary>
    ///     Test that report-depth with invalid value shows error
    /// </summary>
    [Fact]
    public void IntegrationTest_ReportDepthWithInvalidValue_ShowsError()
    {
        // Run the application with --report-depth and invalid value
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--report-depth", "invalid");

        // Verify error exit code
        Assert.Equal(1, exitCode);

        // Verify error message
        Assert.Contains("Error:", output);
        Assert.Contains("--report-depth requires a depth between 1 and 6", output);
    }

    /// <summary>
    ///     Test that report-depth with zero shows error
    /// </summary>
    [Fact]
    public void IntegrationTest_ReportDepthWithZero_ShowsError()
    {
        // Run the application with --report-depth 0
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--report-depth", "0");

        // Verify error exit code
        Assert.Equal(1, exitCode);

        // Verify error message
        Assert.Contains("Error:", output);
        Assert.Contains("--report-depth requires a depth between 1 and 6", output);
    }

    /// <summary>
    ///     Test that --depth without value shows error
    /// </summary>
    [Fact]
    public void IntegrationTest_DepthWithoutValue_ShowsError()
    {
        // Run the application with --depth but no value
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--depth");

        // Verify error exit code
        Assert.Equal(1, exitCode);

        // Verify that the error message mentions --depth
        Assert.Contains("Error:", output);
        Assert.Contains("--depth requires a depth argument", output);
    }

    /// <summary>
    ///     Test that --depth with invalid value shows error
    /// </summary>
    [Fact]
    public void IntegrationTest_DepthWithInvalidValue_ShowsError()
    {
        // Run the application with --depth and invalid value
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--depth", "invalid");

        // Verify error exit code
        Assert.Equal(1, exitCode);

        // Verify that the error message mentions --depth
        Assert.Contains("Error:", output);
        Assert.Contains("--depth requires a depth between 1 and 6", output);
    }

    /// <summary>
    ///     Test that --depth with zero shows error
    /// </summary>
    [Fact]
    public void IntegrationTest_DepthWithZero_ShowsError()
    {
        // Run the application with --depth 0
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--depth", "0");

        // Verify error exit code
        Assert.Equal(1, exitCode);

        // Verify that the error message mentions --depth
        Assert.Contains("Error:", output);
        Assert.Contains("--depth requires a depth between 1 and 6", output);
    }

    /// <summary>
    ///     Test that token parameter is accepted
    /// </summary>
    [Fact]
    public void IntegrationTest_TokenParameter_IsAccepted()
    {
        // Run the application with token parameter
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--token", "test-token");

        // Verify error exit code (missing server and project-key)
        Assert.Equal(1, exitCode);

        // Verify it's not an argument error
        Assert.DoesNotContain("Unsupported argument", output);
    }

    /// <summary>
    ///     Test that branch parameter is accepted
    /// </summary>
    [Fact]
    public void IntegrationTest_BranchParameter_IsAccepted()
    {
        // Run the application with branch parameter
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--branch", "main");

        // Verify error exit code (missing server and project-key)
        Assert.Equal(1, exitCode);

        // Verify it's not an argument error
        Assert.DoesNotContain("Unsupported argument", output);
    }

    /// <summary>
    ///     Test that enforce flag is accepted
    /// </summary>
    [Fact]
    public void IntegrationTest_EnforceFlag_IsAccepted()
    {
        // Run the application with enforce flag
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--enforce");

        // Verify error exit code (missing server and project-key)
        Assert.Equal(1, exitCode);

        // Verify it's not an argument error
        Assert.DoesNotContain("Unsupported argument", output);
    }

    /// <summary>
    ///     Test that report parameter is accepted
    /// </summary>
    [Fact]
    public void IntegrationTest_ReportParameter_IsAccepted()
    {
        // Run the application with report parameter
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--report", "output.md");

        // Verify error exit code (missing server and project-key)
        Assert.Equal(1, exitCode);

        // Verify it's not an argument error
        Assert.DoesNotContain("Unsupported argument", output);
    }

    /// <summary>
    ///     Test that the help verb lists the help topics.
    /// </summary>
    [Fact]
    public void IntegrationTest_HelpTopics_HelpVerb_ListsTopics()
    {
        // Arrange: (none — constructor initializes _dllPath)

        // Act: run the tool with the help verb
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "help");

        // Assert: every topic is listed with its command, without the banner
        Assert.Equal(0, exitCode);
        foreach (var topic in HelpCommand.RootCardIds)
        {
            Assert.Contains($"{HelpCommand.ToolName} help {topic}", output);
        }

        Assert.DoesNotContain("Copyright", output);
    }

    /// <summary>
    ///     Test that an unknown help topic returns the usage-error exit code.
    /// </summary>
    [Fact]
    public void IntegrationTest_HelpTopics_UnknownTopic_ReturnsUsageError()
    {
        // Arrange: (none — constructor initializes _dllPath)

        // Act: ask for a topic that does not exist
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "help",
            "no-such-topic");

        // Assert: exit code 2 and a message that names the topic
        Assert.Equal(2, exitCode);
        Assert.Contains("no-such-topic", output);
    }

    /// <summary>
    ///     Test that every command example in the help cards gives the result that its card states.
    /// </summary>
    [Fact]
    public void IntegrationTest_HelpExamples_AllCommandExamples_GiveStatedResults()
    {
        // Arrange: the catalog the tool ships
        var catalog = HelpCatalog.Create(HelpCommand.CreateOptions().GetCatalogConfiguration());

        // Act: run every command example against the built tool in the runner's scratch folder; the runner
        // compares the exit codes and the 'expect' fragments. No example reads an input file, and the examples
        // that need a live SonarQube or SonarCloud server are snippets, which the runner never runs.
        var report = HelpExampleRunner.Run(catalog, invocation =>
        {
            var exitCode = Runner.RunInDirectory(
                out var output,
                invocation.WorkingDirectory,
                "dotnet",
                [_dllPath, .. invocation.Arguments]);
            return new HelpExampleOutcome(exitCode, output);
        });

        // Assert: the cards hold command examples, and every one gave the result its card states
        Assert.True(report.CommandsRun > 0, "The help cards contain no command examples.");
        report.AssertSucceeded();
    }
}
