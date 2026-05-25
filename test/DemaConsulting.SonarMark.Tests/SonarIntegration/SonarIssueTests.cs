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

using DemaConsulting.SonarMark.SonarIntegration;
using Xunit;

namespace DemaConsulting.SonarMark.Tests.SonarIntegration;

/// <summary>
///     Verifies that <see cref="SonarIssue"/> correctly stores all API-sourced issue fields and supports
///     nullable line numbers across all severity levels.
/// </summary>
public class SonarIssueTests
{
    /// <summary>
    ///     Test that SonarIssue can be created with all properties
    /// </summary>
    [Fact]
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
        Assert.Equal("issue-key-123", issue.Key);
        Assert.Equal("csharpsquid:S1234", issue.Rule);
        Assert.Equal("MAJOR", issue.Severity);
        Assert.Equal("test_project:src/File.cs", issue.Component);
        Assert.Equal(42, issue.Line);
        Assert.Equal("Issue message", issue.Message);
        Assert.Equal("BUG", issue.Type);
    }

    /// <summary>
    ///     Test that SonarIssue can be created with null line number
    /// </summary>
    [Fact]
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
        Assert.Equal("issue-key-456", issue.Key);
        Assert.Equal("csharpsquid:S5678", issue.Rule);
        Assert.Equal("MINOR", issue.Severity);
        Assert.Equal("test_project:src/Global.cs", issue.Component);
        Assert.Null(issue.Line);
        Assert.Equal("Global issue", issue.Message);
        Assert.Equal("CODE_SMELL", issue.Type);
    }

    /// <summary>
    ///     Test that SonarIssue supports BLOCKER severity
    /// </summary>
    [Fact]
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
        Assert.Equal("issue-key-789", issue.Key);
        Assert.Equal("csharpsquid:S9999", issue.Rule);
        Assert.Equal("BLOCKER", issue.Severity);
        Assert.Equal("test_project:src/Critical.cs", issue.Component);
        Assert.Equal(10, issue.Line);
        Assert.Equal("Critical issue", issue.Message);
        Assert.Equal("BUG", issue.Type);
    }

    /// <summary>
    ///     Test that SonarIssue supports CRITICAL severity
    /// </summary>
    [Fact]
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
        Assert.Equal("issue-key-abc", issue.Key);
        Assert.Equal("csharpsquid:S8888", issue.Rule);
        Assert.Equal("CRITICAL", issue.Severity);
        Assert.Equal("test_project:src/Important.cs", issue.Component);
        Assert.Equal(20, issue.Line);
        Assert.Equal("Critical security issue", issue.Message);
        Assert.Equal("VULNERABILITY", issue.Type);
    }

    /// <summary>
    ///     Test that SonarIssue supports INFO severity
    /// </summary>
    [Fact]
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
        Assert.Equal("issue-key-def", issue.Key);
        Assert.Equal("csharpsquid:S7777", issue.Rule);
        Assert.Equal("INFO", issue.Severity);
        Assert.Equal("test_project:src/Helper.cs", issue.Component);
        Assert.Equal(5, issue.Line);
        Assert.Equal("Informational message", issue.Message);
        Assert.Equal("CODE_SMELL", issue.Type);
    }
}

