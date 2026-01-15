namespace DemaConsulting.SonarMark.Tests;

/// <summary>
/// Tests for SonarMark class
/// </summary>
[TestClass]
public class SonarMarkTests
{
    /// <summary>
    /// Test that Version constant is not empty
    /// </summary>
    [TestMethod]
    public void Version_IsNotEmpty()
    {
        // Assert
        Assert.IsFalse(string.IsNullOrWhiteSpace(SonarMark.Version));
    }
}
