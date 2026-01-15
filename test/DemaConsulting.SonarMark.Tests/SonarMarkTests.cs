namespace DemaConsulting.SonarMark.Tests;

/// <summary>
/// Tests for Program class
/// </summary>
[TestClass]
public class ProgramTests
{
    /// <summary>
    /// Test that Version constant is not empty
    /// </summary>
    [TestMethod]
    public void Program_Version_NotEmpty_IsNotEmpty()
    {
        // Assert
        Assert.IsFalse(string.IsNullOrWhiteSpace(Program.Version));
    }
}
