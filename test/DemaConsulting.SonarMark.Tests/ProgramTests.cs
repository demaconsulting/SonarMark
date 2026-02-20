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
        // Arrange - capture console output
        var originalOut = Console.Out;
        using var output = new StringWriter();
        Console.SetOut(output);

        try
        {
            using var context = Context.Create([]);

            // Act - run the program with no arguments
            Program.Run(context);

            // Assert - verify banner is shown and error about missing --server parameter
            // This test proves that running without required parameters shows appropriate error
            var outputText = output.ToString();
            Assert.Contains("SonarMark version", outputText);
            Assert.Contains("--server parameter is required", outputText);
            Assert.AreEqual(1, context.ExitCode);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test that Run method with server but no project key outputs error
    /// </summary>
    [TestMethod]
    public void Program_Run_WithServerButNoProjectKey_OutputsProjectKeyRequiredError()
    {
        // Arrange - capture console output
        var originalOut = Console.Out;
        using var output = new StringWriter();
        Console.SetOut(output);

        try
        {
            using var context = Context.Create(["--server", "https://sonarcloud.io"]);

            // Act - run the program with server but no project key
            Program.Run(context);

            // Assert - verify error about missing --project-key parameter
            // This test proves that --server requires --project-key to also be specified
            var outputText = output.ToString();
            Assert.Contains("--project-key parameter is required", outputText);
            Assert.AreEqual(1, context.ExitCode);
        }
        finally
        {
            Console.SetOut(originalOut);
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
}
