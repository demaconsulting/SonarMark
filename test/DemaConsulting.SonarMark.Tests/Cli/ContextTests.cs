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

using DemaConsulting.SonarMark.Cli;
using DemaConsulting.SonarMark.SonarIntegration;
using Xunit;

namespace DemaConsulting.SonarMark.Tests.Cli;

/// <summary>
///     Unit tests for the Context class.
/// </summary>
[Collection("NonParallelTests")]
public sealed class ContextTests : IDisposable
{
    private readonly string _testDirectory;

    /// <summary>
    ///     Initialize test by creating a temporary test directory.
    /// </summary>
    public ContextTests()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), $"sonarmark_test_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testDirectory);
    }

    /// <summary>
    ///     Clean up test by deleting the temporary test directory.
    /// </summary>
    public void Dispose()
    {
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, recursive: true);
        }

        GC.SuppressFinalize(this);
    }


    /// <summary>
    ///     Test creating a context with no arguments.
    /// </summary>
    [Fact]
    public void Context_Create_NoArguments_ReturnsDefaultContext()
    {
        // Act
        using var context = Context.Create([]);

        // Assert
        Assert.False(context.Version);
        Assert.False(context.Help);
        Assert.False(context.Silent);
        Assert.False(context.Validate);
        Assert.Null(context.ReportFile);
        Assert.Equal(1, context.Depth);
        Assert.Null(context.Token);
        Assert.Null(context.Server);
        Assert.Null(context.ProjectKey);
        Assert.Null(context.Branch);
        Assert.Equal(0, context.ExitCode);
    }

    /// <summary>
    ///     Test creating a context with version flag.
    /// </summary>
    [Fact]
    public void Context_Create_VersionFlag_SetsVersionProperty()
    {
        // Act/Assert
        using var context1 = Context.Create(["-v"]);
        Assert.True(context1.Version);
        Assert.Equal(0, context1.ExitCode);

        using var context2 = Context.Create(["--version"]);
        Assert.True(context2.Version);
        Assert.Equal(0, context2.ExitCode);
    }

    /// <summary>
    ///     Test creating a context with help flags.
    /// </summary>
    [Fact]
    public void Context_Create_HelpFlags_SetsHelpProperty()
    {
        // Act/Assert
        using var context1 = Context.Create(["-?"]);
        Assert.True(context1.Help);
        Assert.Equal(0, context1.ExitCode);

        using var context2 = Context.Create(["-h"]);
        Assert.True(context2.Help);
        Assert.Equal(0, context2.ExitCode);

        using var context3 = Context.Create(["--help"]);
        Assert.True(context3.Help);
        Assert.Equal(0, context3.ExitCode);
    }

    /// <summary>
    ///     Test creating a context with silent flag.
    /// </summary>
    [Fact]
    public void Context_Create_SilentFlag_SetsSilentProperty()
    {
        // Act
        using var context = Context.Create(["--silent"]);

        // Assert
        Assert.True(context.Silent);
        Assert.Equal(0, context.ExitCode);
    }

    /// <summary>
    ///     Test creating a context with validate flag.
    /// </summary>
    [Fact]
    public void Context_Create_ValidateFlag_SetsValidateProperty()
    {
        // Act
        using var context = Context.Create(["--validate"]);

        // Assert
        Assert.True(context.Validate);
        Assert.Equal(0, context.ExitCode);
    }

    /// <summary>
    ///     Test creating a context with enforce flag.
    /// </summary>
    [Fact]
    public void Context_Create_EnforceFlag_SetsEnforceProperty()
    {
        // Act
        using var context = Context.Create(["--enforce"]);

        // Assert
        Assert.True(context.Enforce);
        Assert.Equal(0, context.ExitCode);
    }

    /// <summary>
    ///     Test creating a context with report file.
    /// </summary>
    [Fact]
    public void Context_Create_ReportFile_SetsReportProperty()
    {
        // Act
        using var context = Context.Create(["--report", "report.md"]);

        // Assert
        Assert.Equal("report.md", context.ReportFile);
        Assert.Equal(0, context.ExitCode);
    }

    /// <summary>
    ///     Test creating a context with missing report filename.
    /// </summary>
    [Fact]
    public void Context_Create_MissingReportFilename_ThrowsException()
    {
        // Act/Assert
        var ex = Assert.Throws<ArgumentException>(() => Context.Create(["--report"]));
        Assert.Contains("--report requires a filename argument", ex.Message);
    }

    /// <summary>
    ///     Test that the deprecated --report-depth flag sets the Depth property across representative values.
    /// </summary>
    [Theory]
    [InlineData("1", 1)]
    [InlineData("3", 3)]
    [InlineData("6", 6)]
    public void Context_Create_ReportDepthAlias_SetsDepthProperty(string depthArg, int expectedDepth)
    {
        // Act
        using var context = Context.Create(["--report-depth", depthArg]);

        // Assert
        Assert.Equal(expectedDepth, context.Depth);
        Assert.Equal(0, context.ExitCode);
    }

    /// <summary>
    ///     Test creating a context with missing report depth.
    /// </summary>
    [Fact]
    public void Context_Create_MissingReportDepth_ThrowsException()
    {
        // Act/Assert
        var ex = Assert.Throws<ArgumentException>(() => Context.Create(["--report-depth"]));
        Assert.Contains("--report-depth requires a depth argument", ex.Message);
    }

    /// <summary>
    ///     Test creating a context with invalid report depth.
    /// </summary>
    [Fact]
    public void Context_Create_InvalidReportDepth_ThrowsException()
    {
        // Act/Assert
        var ex1 = Assert.Throws<ArgumentException>(() => Context.Create(["--report-depth", "invalid"]));
        Assert.Contains("--report-depth requires a depth between 1 and 6", ex1.Message);

        var ex2 = Assert.Throws<ArgumentException>(() => Context.Create(["--report-depth", "0"]));
        Assert.Contains("--report-depth requires a depth between 1 and 6", ex2.Message);

        var ex3 = Assert.Throws<ArgumentException>(() => Context.Create(["--report-depth", "-1"]));
        Assert.Contains("--report-depth requires a depth between 1 and 6", ex3.Message);

        var ex4 = Assert.Throws<ArgumentException>(() => Context.Create(["--report-depth", "7"]));
        Assert.Contains("--report-depth requires a depth between 1 and 6", ex4.Message);
    }

    /// <summary>
    ///     Test creating a context with --depth (primary option).
    /// </summary>
    [Fact]
    public void Context_Create_Depth_SetsDepthProperty()
    {
        // Act
        using var context = Context.Create(["--depth", "3"]);

        // Assert
        Assert.Equal(3, context.Depth);
        Assert.Equal(0, context.ExitCode);
    }

    /// <summary>
    ///     Test creating a context with missing --depth value.
    /// </summary>
    [Fact]
    public void Context_Create_MissingDepth_ThrowsException()
    {
        // Act/Assert
        var ex = Assert.Throws<ArgumentException>(() => Context.Create(["--depth"]));
        Assert.Contains("--depth requires a depth argument", ex.Message);
    }

    /// <summary>
    ///     Test creating a context with invalid --depth value.
    /// </summary>
    [Fact]
    public void Context_Create_InvalidDepth_ThrowsException()
    {
        // Act/Assert
        var ex1 = Assert.Throws<ArgumentException>(() => Context.Create(["--depth", "invalid"]));
        Assert.Contains("--depth requires a depth between 1 and 6", ex1.Message);

        var ex2 = Assert.Throws<ArgumentException>(() => Context.Create(["--depth", "0"]));
        Assert.Contains("--depth requires a depth between 1 and 6", ex2.Message);

        var ex3 = Assert.Throws<ArgumentException>(() => Context.Create(["--depth", "-1"]));
        Assert.Contains("--depth requires a depth between 1 and 6", ex3.Message);

        var ex4 = Assert.Throws<ArgumentException>(() => Context.Create(["--depth", "7"]));
        Assert.Contains("--depth requires a depth between 1 and 6", ex4.Message);
    }

    /// <summary>
    ///     Test creating a context with token.
    /// </summary>
    [Fact]
    public void Context_Create_Token_SetsTokenProperty()
    {
        // Act
        using var context = Context.Create(["--token", "test-token-123"]);

        // Assert
        Assert.Equal("test-token-123", context.Token);
        Assert.Equal(0, context.ExitCode);
    }

    /// <summary>
    ///     Test creating a context with missing token.
    /// </summary>
    [Fact]
    public void Context_Create_MissingToken_ThrowsException()
    {
        // Act/Assert
        var ex = Assert.Throws<ArgumentException>(() => Context.Create(["--token"]));
        Assert.Contains("--token requires a token argument", ex.Message);
    }

    /// <summary>
    ///     Test creating a context with server URL.
    /// </summary>
    [Fact]
    public void Context_Create_Server_SetsServerProperty()
    {
        // Act
        using var context = Context.Create(["--server", "https://sonarcloud.io"]);

        // Assert
        Assert.Equal("https://sonarcloud.io", context.Server);
        Assert.Equal(0, context.ExitCode);
    }

    /// <summary>
    ///     Test creating a context with missing server URL.
    /// </summary>
    [Fact]
    public void Context_Create_MissingServer_ThrowsException()
    {
        // Act/Assert
        var ex = Assert.Throws<ArgumentException>(() => Context.Create(["--server"]));
        Assert.Contains("--server requires a server URL argument", ex.Message);
    }

    /// <summary>
    ///     Test creating a context with project key.
    /// </summary>
    [Fact]
    public void Context_Create_ProjectKey_SetsProjectKeyProperty()
    {
        // Act
        using var context = Context.Create(["--project-key", "my-project"]);

        // Assert
        Assert.Equal("my-project", context.ProjectKey);
        Assert.Equal(0, context.ExitCode);
    }

    /// <summary>
    ///     Test creating a context with missing project key.
    /// </summary>
    [Fact]
    public void Context_Create_MissingProjectKey_ThrowsException()
    {
        // Act/Assert
        var ex = Assert.Throws<ArgumentException>(() => Context.Create(["--project-key"]));
        Assert.Contains("--project-key requires a project key argument", ex.Message);
    }

    /// <summary>
    ///     Test creating a context with branch.
    /// </summary>
    [Fact]
    public void Context_Create_Branch_SetsBranchProperty()
    {
        // Act
        using var context = Context.Create(["--branch", "main"]);

        // Assert
        Assert.Equal("main", context.Branch);
        Assert.Equal(0, context.ExitCode);
    }

    /// <summary>
    ///     Test creating a context with missing branch.
    /// </summary>
    [Fact]
    public void Context_Create_MissingBranch_ThrowsException()
    {
        // Act/Assert
        var ex = Assert.Throws<ArgumentException>(() => Context.Create(["--branch"]));
        Assert.Contains("--branch requires a branch name argument", ex.Message);
    }

    /// <summary>
    ///     Test creating a context with missing log filename.
    /// </summary>
    [Fact]
    public void Context_Create_MissingLogFilename_ThrowsException()
    {
        // Act/Assert
        var ex = Assert.Throws<ArgumentException>(() => Context.Create(["--log"]));
        Assert.Contains("--log requires a filename argument", ex.Message);
    }

    /// <summary>
    ///     Test creating a context with unsupported argument.
    /// </summary>
    [Fact]
    public void Context_Create_UnsupportedArgument_ThrowsException()
    {
        // Act/Assert
        var ex = Assert.Throws<ArgumentException>(() => Context.Create(["--unsupported"]));
        Assert.Contains("Unsupported argument '--unsupported'", ex.Message);
    }

    /// <summary>
    ///     Test WriteLine writes to console.
    /// </summary>
    [Fact]
    public void Context_WriteLine_NormalMode_WritesToConsole()
    {
        // Arrange
        var originalOut = Console.Out;
        using var output = new StringWriter();
        Console.SetOut(output);

        try
        {
            // Act
            using var context = Context.Create([]);
            context.WriteLine("Test message");

            // Assert
            Assert.Equal("Test message" + Environment.NewLine, output.ToString());
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test WriteLine in silent mode doesn't write to console.
    /// </summary>
    [Fact]
    public void Context_WriteLine_SilentMode_DoesNotWriteToConsole()
    {
        // Arrange
        var originalOut = Console.Out;
        using var output = new StringWriter();
        Console.SetOut(output);

        try
        {
            // Act
            using var context = Context.Create(["--silent"]);
            context.WriteLine("Test message");

            // Assert
            Assert.Equal(string.Empty, output.ToString());
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test WriteError writes to console.
    /// </summary>
    [Fact]
    public void Context_WriteError_NormalMode_WritesToConsole()
    {
        // Arrange
        var originalError = Console.Error;
        using var error = new StringWriter();
        Console.SetError(error);

        try
        {
            // Act
            using var context = Context.Create([]);
            context.WriteError("Error message");

            // Assert
            Assert.Equal("Error message" + Environment.NewLine, error.ToString());
            Assert.Equal(1, context.ExitCode);
        }
        finally
        {
            Console.SetError(originalError);
        }
    }

    /// <summary>
    ///     Test WriteError in silent mode doesn't write to console.
    /// </summary>
    [Fact]
    public void Context_WriteError_SilentMode_DoesNotWriteToConsole()
    {
        // Arrange
        var originalError = Console.Error;
        using var error = new StringWriter();
        Console.SetError(error);

        try
        {
            // Act
            using var context = Context.Create(["--silent"]);
            context.WriteError("Error message");

            // Assert
            Assert.Equal(string.Empty, error.ToString());
            Assert.Equal(1, context.ExitCode);
        }
        finally
        {
            Console.SetError(originalError);
        }
    }

    /// <summary>
    ///     Test log file creation and writing.
    /// </summary>
    [Fact]
    public void Context_Create_WithLogFile_WritesToLogFile()
    {
        // Arrange
        var logPath = Path.Combine(_testDirectory, "test.log");

        // Act
        using (var context = Context.Create(["--log", logPath, "--silent"]))
        {
            context.WriteLine("Normal message");
            context.WriteError("Error message");
        }

        // Assert
        Assert.True(File.Exists(logPath));
        var logContent = File.ReadAllText(logPath);
        Assert.Contains("Normal message", logContent);
        Assert.Contains("Error message", logContent);
    }

    /// <summary>
    ///     Test that creating context with invalid log file path throws exception
    /// </summary>
    [Fact]
    public void Context_Create_InvalidLogFilePath_ThrowsException()
    {
        // Arrange
        // Use a path containing illegal characters that is guaranteed to fail on all platforms
        var invalidPath = Path.Combine(
            Path.GetTempPath(),
            new string(Path.GetInvalidFileNameChars()[0], 5) + ".log");

        // Act/Assert
        var ex = Assert.Throws<InvalidOperationException>(() => Context.Create(["--log", invalidPath]));
        Assert.Contains("Failed to open log file", ex.Message);
    }

    /// <summary>
    ///     Test creating a context with results file.
    /// </summary>
    [Fact]
    public void Context_Create_ResultsFile_SetsResultsProperty()
    {
        // Act
        using var context = Context.Create(["--results", "results.trx"]);

        // Assert
        Assert.Equal("results.trx", context.ResultsFile);
        Assert.Equal(0, context.ExitCode);
    }

    /// <summary>
    ///     Test creating a context with missing results filename.
    /// </summary>
    [Fact]
    public void Context_Create_MissingResultsFilename_ThrowsException()
    {
        // Act/Assert
        var ex = Assert.Throws<ArgumentException>(() => Context.Create(["--results"]));
        Assert.Contains("--results requires a results filename argument", ex.Message);
    }

    /// <summary>
    ///     Test that --result (legacy alias) sets the ResultsFile property.
    /// </summary>
    [Fact]
    public void Context_Create_ResultAlias_SetsResultsProperty()
    {
        // Act
        using var context = Context.Create(["--result", "results.trx"]);

        // Assert
        Assert.Equal("results.trx", context.ResultsFile);
        Assert.Equal(0, context.ExitCode);
    }

    /// <summary>
    ///     Test that --result (legacy alias) requires a filename argument.
    /// </summary>
    [Fact]
    public void Context_Create_MissingResultAliasFilename_ThrowsException()
    {
        // Act/Assert
        var ex = Assert.Throws<ArgumentException>(() => Context.Create(["--result"]));
        Assert.Contains("--result requires a results filename argument", ex.Message);
    }
}
