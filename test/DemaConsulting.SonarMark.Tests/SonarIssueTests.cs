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

namespace DemaConsulting.SonarMark.Tests;

/// <summary>
///     Tests for SonarIssue class
/// </summary>
[TestClass]
public class SonarIssueTests
{
    /// <summary>
    ///     Test that SonarIssue can be created with all properties
    /// </summary>
    [TestMethod]
    public void SonarIssue_Constructor_AllProperties_CreatesInstance()
    {
        // Arrange & Act
        var issue = new SonarIssue(
            "issue-key-123",
            "csharpsquid:S1234",
            "MAJOR",
            "test_project:src/File.cs",
            42,
            "Issue message",
            "BUG");

        // Assert
        Assert.AreEqual("issue-key-123", issue.Key);
        Assert.AreEqual("csharpsquid:S1234", issue.Rule);
        Assert.AreEqual("MAJOR", issue.Severity);
        Assert.AreEqual("test_project:src/File.cs", issue.Component);
        Assert.AreEqual(42, issue.Line);
        Assert.AreEqual("Issue message", issue.Message);
        Assert.AreEqual("BUG", issue.Type);
    }

    /// <summary>
    ///     Test that SonarIssue can be created with null line number
    /// </summary>
    [TestMethod]
    public void SonarIssue_Constructor_NullLine_CreatesInstance()
    {
        // Arrange & Act
        var issue = new SonarIssue(
            "issue-key-456",
            "csharpsquid:S5678",
            "MINOR",
            "test_project:src/Global.cs",
            null,
            "Global issue",
            "CODE_SMELL");

        // Assert
        Assert.AreEqual("issue-key-456", issue.Key);
        Assert.AreEqual("csharpsquid:S5678", issue.Rule);
        Assert.AreEqual("MINOR", issue.Severity);
        Assert.AreEqual("test_project:src/Global.cs", issue.Component);
        Assert.IsNull(issue.Line);
        Assert.AreEqual("Global issue", issue.Message);
        Assert.AreEqual("CODE_SMELL", issue.Type);
    }

    /// <summary>
    ///     Test that SonarIssue supports BLOCKER severity
    /// </summary>
    [TestMethod]
    public void SonarIssue_Constructor_BlockerSeverity_CreatesInstance()
    {
        // Arrange & Act
        var issue = new SonarIssue(
            "issue-key-789",
            "csharpsquid:S9999",
            "BLOCKER",
            "test_project:src/Critical.cs",
            10,
            "Critical issue",
            "BUG");

        // Assert
        Assert.AreEqual("issue-key-789", issue.Key);
        Assert.AreEqual("BLOCKER", issue.Severity);
        Assert.AreEqual("BUG", issue.Type);
    }

    /// <summary>
    ///     Test that SonarIssue supports CRITICAL severity
    /// </summary>
    [TestMethod]
    public void SonarIssue_Constructor_CriticalSeverity_CreatesInstance()
    {
        // Arrange & Act
        var issue = new SonarIssue(
            "issue-key-abc",
            "csharpsquid:S8888",
            "CRITICAL",
            "test_project:src/Important.cs",
            20,
            "Critical security issue",
            "VULNERABILITY");

        // Assert
        Assert.AreEqual("issue-key-abc", issue.Key);
        Assert.AreEqual("CRITICAL", issue.Severity);
        Assert.AreEqual("VULNERABILITY", issue.Type);
    }

    /// <summary>
    ///     Test that SonarIssue supports INFO severity
    /// </summary>
    [TestMethod]
    public void SonarIssue_Constructor_InfoSeverity_CreatesInstance()
    {
        // Arrange & Act
        var issue = new SonarIssue(
            "issue-key-def",
            "csharpsquid:S7777",
            "INFO",
            "test_project:src/Helper.cs",
            5,
            "Informational message",
            "CODE_SMELL");

        // Assert
        Assert.AreEqual("issue-key-def", issue.Key);
        Assert.AreEqual("INFO", issue.Severity);
        Assert.AreEqual("CODE_SMELL", issue.Type);
    }
}
