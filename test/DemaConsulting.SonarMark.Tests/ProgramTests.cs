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
    public void Program_Version_NotEmpty_IsNotEmpty()
    {
        // Assert
        Assert.IsFalse(string.IsNullOrWhiteSpace(Program.Version));
    }

    /// <summary>
    ///     Test that Run method with version flag outputs only version
    /// </summary>
    [TestMethod]
    public void Program_Run_VersionFlag_OutputsVersion()
    {
        var originalOut = Console.Out;
        using var output = new StringWriter();
        Console.SetOut(output);

        try
        {
            using var context = Context.Create(["--version"]);
            Program.Run(context);

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
    public void Program_Run_HelpFlag_OutputsBannerAndHelp()
    {
        var originalOut = Console.Out;
        using var output = new StringWriter();
        Console.SetOut(output);

        try
        {
            using var context = Context.Create(["--help"]);
            Program.Run(context);

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
    ///     Test that Run method with validate flag outputs not implemented message
    /// </summary>
    [TestMethod]
    public void Program_Run_ValidateFlag_OutputsNotImplemented()
    {
        using var context = Context.Create(["--validate"]);
        Program.Run(context);

        // Just verify it doesn't throw and succeeds
        Assert.AreEqual(0, context.ExitCode);
    }

    /// <summary>
    ///     Test that Run method with no flags outputs banner and error for missing server
    /// </summary>
    [TestMethod]
    public void Program_Run_NoFlags_OutputsBannerAndRequiresServer()
    {
        var originalOut = Console.Out;
        using var output = new StringWriter();
        Console.SetOut(output);

        try
        {
            using var context = Context.Create([]);
            Program.Run(context);

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
    public void Program_Run_ServerWithoutProjectKey_OutputsError()
    {
        var originalOut = Console.Out;
        using var output = new StringWriter();
        Console.SetOut(output);

        try
        {
            using var context = Context.Create(["--server", "https://sonarcloud.io"]);
            Program.Run(context);

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
    public void Program_Run_SilentFlag_SuppressesBanner()
    {
        var originalOut = Console.Out;
        using var output = new StringWriter();
        Console.SetOut(output);

        try
        {
            using var context = Context.Create(["--silent"]);
            Program.Run(context);

            var outputText = output.ToString();
            Assert.DoesNotContain("SonarMark version", outputText);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }
}
