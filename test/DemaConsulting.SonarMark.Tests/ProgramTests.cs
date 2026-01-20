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
        var output = new StringWriter();
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
        using var context = Context.Create(["--help"]);
        Program.Run(context);

        // Just verify it doesn't throw - actual output is verified by Context tests
        Assert.AreEqual(0, context.ExitCode);
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
}
