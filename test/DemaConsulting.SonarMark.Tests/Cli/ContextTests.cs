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
using DemaConsulting.SonarMark.Tests.SonarIntegration;
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
        // Arrange - save and clear SONAR_TOKEN so the default-token assertion is deterministic
        // regardless of the ambient environment
        var previous = Environment.GetEnvironmentVariable("SONAR_TOKEN");
        Environment.SetEnvironmentVariable("SONAR_TOKEN", null);

        try
        {
            // Act
            using var context = Context.Create([]);

            // Assert
            Assert.False(context.Version);
            Assert.False(context.Help);
            Assert.False(context.Silent);
            Assert.False(context.Validate);
            Assert.False(context.Enforce);
            Assert.Null(context.ReportFile);
            Assert.Equal(1, context.Depth);
            Assert.Null(context.Token);
            Assert.Null(context.Server);
            Assert.Null(context.ProjectKey);
            Assert.Null(context.Branch);
            Assert.Null(context.ResultsFile);
            Assert.Equal(0, context.ExitCode);
        }
        finally
        {
            // Restore the previous environment variable value
            Environment.SetEnvironmentVariable("SONAR_TOKEN", previous);
        }
    }

    /// <summary>
    ///     Test creating a context with version flag.
    /// </summary>
    [Fact]
    public void Context_Create_VersionFlag_SetsVersionProperty()
    {
        // Arrange - no setup required

        // Act
        using var context1 = Context.Create(["-v"]);

        // Assert
        Assert.True(context1.Version);
        Assert.Equal(0, context1.ExitCode);

        // Act
        using var context2 = Context.Create(["--version"]);

        // Assert
        Assert.True(context2.Version);
        Assert.Equal(0, context2.ExitCode);
    }

    /// <summary>
    ///     Test creating a context with help flags.
    /// </summary>
    [Fact]
    public void Context_Create_HelpFlags_SetsHelpProperty()
    {
        // Arrange - no setup required

        // Act
        using var context1 = Context.Create(["-?"]);

        // Assert
        Assert.True(context1.Help);
        Assert.Equal(0, context1.ExitCode);

        // Act
        using var context2 = Context.Create(["-h"]);

        // Assert
        Assert.True(context2.Help);
        Assert.Equal(0, context2.ExitCode);

        // Act
        using var context3 = Context.Create(["--help"]);

        // Assert
        Assert.True(context3.Help);
        Assert.Equal(0, context3.ExitCode);
    }

    /// <summary>
    ///     Test creating a context with silent flag.
    /// </summary>
    [Fact]
    public void Context_Create_SilentFlag_SetsSilentProperty()
    {
        // Arrange - no setup required

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
        // Arrange - no setup required

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
        // Arrange - no setup required

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
        // Arrange - no setup required

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
        // Arrange - no setup required

        // Act
        var ex = Assert.Throws<ArgumentException>(() => Context.Create(["--report"]));

        // Assert
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
        // Arrange - no setup required

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
        // Arrange - no setup required

        // Act
        var ex = Assert.Throws<ArgumentException>(() => Context.Create(["--report-depth"]));

        // Assert
        Assert.Contains("--report-depth requires a depth argument", ex.Message);
    }

    /// <summary>
    ///     Test creating a context with invalid report depth.
    /// </summary>
    [Fact]
    public void Context_Create_InvalidReportDepth_ThrowsException()
    {
        // Arrange - no setup required

        // Act
        var ex1 = Assert.Throws<ArgumentException>(() => Context.Create(["--report-depth", "invalid"]));

        // Assert
        Assert.Contains("--report-depth requires a depth between 1 and 6", ex1.Message);

        // Act
        var ex2 = Assert.Throws<ArgumentException>(() => Context.Create(["--report-depth", "0"]));

        // Assert
        Assert.Contains("--report-depth requires a depth between 1 and 6", ex2.Message);

        // Act
        var ex3 = Assert.Throws<ArgumentException>(() => Context.Create(["--report-depth", "-1"]));

        // Assert
        Assert.Contains("--report-depth requires a depth between 1 and 6", ex3.Message);

        // Act
        var ex4 = Assert.Throws<ArgumentException>(() => Context.Create(["--report-depth", "7"]));

        // Assert
        Assert.Contains("--report-depth requires a depth between 1 and 6", ex4.Message);
    }

    /// <summary>
    ///     Test creating a context with --depth (primary option).
    /// </summary>
    [Fact]
    public void Context_Create_Depth_SetsDepthProperty()
    {
        // Arrange - no setup required

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
        // Arrange - no setup required

        // Act
        var ex = Assert.Throws<ArgumentException>(() => Context.Create(["--depth"]));

        // Assert
        Assert.Contains("--depth requires a depth argument", ex.Message);
    }

    /// <summary>
    ///     Test creating a context with invalid --depth value.
    /// </summary>
    [Fact]
    public void Context_Create_InvalidDepth_ThrowsException()
    {
        // Arrange - no setup required

        // Act
        var ex1 = Assert.Throws<ArgumentException>(() => Context.Create(["--depth", "invalid"]));

        // Assert
        Assert.Contains("--depth requires a depth between 1 and 6", ex1.Message);

        // Act
        var ex2 = Assert.Throws<ArgumentException>(() => Context.Create(["--depth", "0"]));

        // Assert
        Assert.Contains("--depth requires a depth between 1 and 6", ex2.Message);

        // Act
        var ex3 = Assert.Throws<ArgumentException>(() => Context.Create(["--depth", "-1"]));

        // Assert
        Assert.Contains("--depth requires a depth between 1 and 6", ex3.Message);

        // Act
        var ex4 = Assert.Throws<ArgumentException>(() => Context.Create(["--depth", "7"]));

        // Assert
        Assert.Contains("--depth requires a depth between 1 and 6", ex4.Message);
    }

    /// <summary>
    ///     Test creating a context with token.
    /// </summary>
    [Fact]
    public void Context_Create_Token_SetsTokenProperty()
    {
        // Arrange - no setup required

        // Act
        using var context = Context.Create(["--token", "test-token-123"]);

        // Assert
        Assert.Equal("test-token-123", context.Token);
        Assert.Equal(0, context.ExitCode);
    }

    /// <summary>
    ///     Test that SONAR_TOKEN environment variable is used as fallback when --token is not supplied.
    /// </summary>
    [Fact]
    public void Context_Create_WithTokenEnvVar_ReturnsTokenFromEnvironment()
    {
        // Arrange - set the SONAR_TOKEN environment variable and ensure it is removed after the test
        var previous = Environment.GetEnvironmentVariable("SONAR_TOKEN");
        Environment.SetEnvironmentVariable("SONAR_TOKEN", "env-token-abc");

        try
        {
            // Act - create context without --token; the env var must be used as fallback
            using var context = Context.Create([]);

            // Assert - Token must be populated from the environment variable
            Assert.Equal("env-token-abc", context.Token);
        }
        finally
        {
            // Restore the previous environment variable value
            Environment.SetEnvironmentVariable("SONAR_TOKEN", previous);
        }
    }

    /// <summary>
    ///     Test creating a context with missing token.
    /// </summary>
    [Fact]
    public void Context_Create_MissingToken_ThrowsException()
    {
        // Arrange - no setup required

        // Act
        var ex = Assert.Throws<ArgumentException>(() => Context.Create(["--token"]));

        // Assert
        Assert.Contains("--token requires a token argument", ex.Message);
    }

    /// <summary>
    ///     Test creating a context where a string-valued flag is immediately followed by another
    ///     recognized flag instead of a value. The missing value must be detected rather than the
    ///     following flag being consumed as the value.
    /// </summary>
    [Fact]
    public void Context_Create_TokenFollowedByFlag_ThrowsException()
    {
        // Arrange - no setup required

        // Act
        var ex = Assert.Throws<ArgumentException>(() => Context.Create(["--token", "--server", "foo"]));

        // Assert
        Assert.Contains("--token requires a token argument", ex.Message);
    }

    /// <summary>
    ///     Test creating a context for every string-valued flag immediately followed by another
    ///     recognized flag instead of a value, proving the general fix rather than a single case.
    /// </summary>
    [Theory]
    [InlineData("--token", "--token requires a token argument")]
    [InlineData("--server", "--server requires a server URL argument")]
    [InlineData("--project-key", "--project-key requires a project key argument")]
    [InlineData("--branch", "--branch requires a branch name argument")]
    [InlineData("--report", "--report requires a filename argument")]
    [InlineData("--results", "--results requires a results filename argument")]
    [InlineData("--result", "--result requires a results filename argument")]
    [InlineData("--log", "--log requires a filename argument")]
    public void Context_Create_StringFlagFollowedByFlag_ThrowsException(string flag, string expectedMessage)
    {
        // Arrange - no setup required

        // Act
        var ex = Assert.Throws<ArgumentException>(() => Context.Create([flag, "--silent"]));

        // Assert
        Assert.Contains(expectedMessage, ex.Message);
    }

    /// <summary>
    ///     Test creating a context with server URL.
    /// </summary>
    [Fact]
    public void Context_Create_Server_SetsServerProperty()
    {
        // Arrange - no setup required

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
        // Arrange - no setup required

        // Act
        var ex = Assert.Throws<ArgumentException>(() => Context.Create(["--server"]));

        // Assert
        Assert.Contains("--server requires a server URL argument", ex.Message);
    }

    /// <summary>
    ///     Test creating a context with project key.
    /// </summary>
    [Fact]
    public void Context_Create_ProjectKey_SetsProjectKeyProperty()
    {
        // Arrange - no setup required

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
        // Arrange - no setup required

        // Act
        var ex = Assert.Throws<ArgumentException>(() => Context.Create(["--project-key"]));

        // Assert
        Assert.Contains("--project-key requires a project key argument", ex.Message);
    }

    /// <summary>
    ///     Test creating a context with branch.
    /// </summary>
    [Fact]
    public void Context_Create_Branch_SetsBranchProperty()
    {
        // Arrange - no setup required

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
        // Arrange - no setup required

        // Act
        var ex = Assert.Throws<ArgumentException>(() => Context.Create(["--branch"]));

        // Assert
        Assert.Contains("--branch requires a branch name argument", ex.Message);
    }

    /// <summary>
    ///     Test creating a context with missing log filename.
    /// </summary>
    [Fact]
    public void Context_Create_MissingLogFilename_ThrowsException()
    {
        // Arrange - no setup required

        // Act
        var ex = Assert.Throws<ArgumentException>(() => Context.Create(["--log"]));

        // Assert
        Assert.Contains("--log requires a filename argument", ex.Message);
    }

    /// <summary>
    ///     Test creating a context with unsupported argument.
    /// </summary>
    [Fact]
    public void Context_Create_UnsupportedArgument_ThrowsException()
    {
        // Arrange - no setup required

        // Act
        var ex = Assert.Throws<ArgumentException>(() => Context.Create(["--unsupported"]));

        // Assert
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

        // Act
        var ex = Assert.Throws<InvalidOperationException>(() => Context.Create(["--log", invalidPath]));

        // Assert
        Assert.Contains("Failed to open log file", ex.Message);
    }

    /// <summary>
    ///     Test creating a context with results file.
    /// </summary>
    [Fact]
    public void Context_Create_ResultsFile_SetsResultsProperty()
    {
        // Arrange - no setup required

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
        // Arrange - no setup required

        // Act
        var ex = Assert.Throws<ArgumentException>(() => Context.Create(["--results"]));

        // Assert
        Assert.Contains("--results requires a results filename argument", ex.Message);
    }

    /// <summary>
    ///     Test that --result (legacy alias) sets the ResultsFile property.
    /// </summary>
    [Fact]
    public void Context_Create_ResultAlias_SetsResultsProperty()
    {
        // Arrange - no setup required

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
        // Arrange - no setup required

        // Act
        var ex = Assert.Throws<ArgumentException>(() => Context.Create(["--result"]));

        // Assert
        Assert.Contains("--result requires a results filename argument", ex.Message);
    }

    /// <summary>
    ///     Test creating a context with an HTTP client factory exposes the factory.
    /// </summary>
    [Fact]
    public void Context_Create_WithHttpClientFactory_ExposesFactory()
    {
        // Arrange
        Func<string?, SonarQubeClient> factory = _ =>
            new SonarQubeClient(new HttpClient(new MockHttpMessageHandler()), false);

        // Act
        using var context = Context.Create([], factory);

        // Assert
        Assert.Same(factory, context.HttpClientFactory);
    }
}
