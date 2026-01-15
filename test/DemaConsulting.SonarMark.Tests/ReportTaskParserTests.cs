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
/// Tests for ReportTaskParser class
/// </summary>
[TestClass]
public class ReportTaskParserTests
{
    /// <summary>
    /// Gets the test directory path
    /// </summary>
    private static string TestDirectory => Path.Combine(Path.GetTempPath(), "SonarMarkTests", Guid.NewGuid().ToString());

    /// <summary>
    /// Test that FindReportTask returns null for non-existent directory
    /// </summary>
    [TestMethod]
    public void ReportTaskParser_FindReportTask_NonExistentDirectory_ReturnsNull()
    {
        // Arrange
        var nonExistentPath = Path.Combine(TestDirectory, "nonexistent");

        // Act
        var result = ReportTaskParser.FindReportTask(nonExistentPath);

        // Assert
        Assert.IsNull(result);
    }

    /// <summary>
    /// Test that FindReportTask returns null when file doesn't exist
    /// </summary>
    [TestMethod]
    public void ReportTaskParser_FindReportTask_FileDoesNotExist_ReturnsNull()
    {
        // Arrange
        var testDir = TestDirectory;
        Directory.CreateDirectory(testDir);

        try
        {
            // Act
            var result = ReportTaskParser.FindReportTask(testDir);

            // Assert
            Assert.IsNull(result);
        }
        finally
        {
            Directory.Delete(testDir, true);
        }
    }

    /// <summary>
    /// Test that FindReportTask finds file in root directory
    /// </summary>
    [TestMethod]
    public void ReportTaskParser_FindReportTask_FileInRootDirectory_ReturnsPath()
    {
        // Arrange
        var testDir = TestDirectory;
        Directory.CreateDirectory(testDir);
        var reportTaskPath = Path.Combine(testDir, "report-task.txt");
        File.WriteAllText(reportTaskPath, "test");

        try
        {
            // Act
            var result = ReportTaskParser.FindReportTask(testDir);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(reportTaskPath, result);
        }
        finally
        {
            Directory.Delete(testDir, true);
        }
    }

    /// <summary>
    /// Test that FindReportTask finds file in subdirectory
    /// </summary>
    [TestMethod]
    public void ReportTaskParser_FindReportTask_FileInSubdirectory_ReturnsPath()
    {
        // Arrange
        var testDir = TestDirectory;
        var subDir = Path.Combine(testDir, "subfolder", "nested");
        Directory.CreateDirectory(subDir);
        var reportTaskPath = Path.Combine(subDir, "report-task.txt");
        File.WriteAllText(reportTaskPath, "test");

        try
        {
            // Act
            var result = ReportTaskParser.FindReportTask(testDir);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(reportTaskPath, result);
        }
        finally
        {
            Directory.Delete(testDir, true);
        }
    }

    /// <summary>
    /// Test that Parse throws for non-existent file
    /// </summary>
    [TestMethod]
    public void ReportTaskParser_Parse_NonExistentFile_ThrowsArgumentException()
    {
        // Arrange
        var nonExistentPath = Path.Combine(TestDirectory, "nonexistent", "report-task.txt");

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => ReportTaskParser.Parse(nonExistentPath));
        Assert.Contains("File not found", exception.Message);
    }

    /// <summary>
    /// Test that Parse throws when projectKey is missing
    /// </summary>
    [TestMethod]
    public void ReportTaskParser_Parse_MissingProjectKey_ThrowsArgumentException()
    {
        // Arrange
        var testDir = TestDirectory;
        Directory.CreateDirectory(testDir);
        var reportTaskPath = Path.Combine(testDir, "report-task.txt");
        File.WriteAllText(reportTaskPath, @"serverUrl=https://sonarcloud.io/
ceTaskId=task123");

        try
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => ReportTaskParser.Parse(reportTaskPath));
            Assert.Contains("projectKey", exception.Message);
        }
        finally
        {
            Directory.Delete(testDir, true);
        }
    }

    /// <summary>
    /// Test that Parse throws when serverUrl is missing
    /// </summary>
    [TestMethod]
    public void ReportTaskParser_Parse_MissingServerUrl_ThrowsArgumentException()
    {
        // Arrange
        var testDir = TestDirectory;
        Directory.CreateDirectory(testDir);
        var reportTaskPath = Path.Combine(testDir, "report-task.txt");
        File.WriteAllText(reportTaskPath, @"projectKey=test_project
ceTaskId=task123");

        try
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => ReportTaskParser.Parse(reportTaskPath));
            Assert.Contains("serverUrl", exception.Message);
        }
        finally
        {
            Directory.Delete(testDir, true);
        }
    }

    /// <summary>
    /// Test that Parse throws when ceTaskId is missing
    /// </summary>
    [TestMethod]
    public void ReportTaskParser_Parse_MissingCeTaskId_ThrowsArgumentException()
    {
        // Arrange
        var testDir = TestDirectory;
        Directory.CreateDirectory(testDir);
        var reportTaskPath = Path.Combine(testDir, "report-task.txt");
        File.WriteAllText(reportTaskPath, @"projectKey=test_project
serverUrl=https://sonarcloud.io/");

        try
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => ReportTaskParser.Parse(reportTaskPath));
            Assert.Contains("ceTaskId", exception.Message);
        }
        finally
        {
            Directory.Delete(testDir, true);
        }
    }

    /// <summary>
    /// Test that Parse successfully parses valid file
    /// </summary>
    [TestMethod]
    public void ReportTaskParser_Parse_ValidFile_ReturnsReportTask()
    {
        // Arrange
        var testDir = TestDirectory;
        Directory.CreateDirectory(testDir);
        var reportTaskPath = Path.Combine(testDir, "report-task.txt");
        File.WriteAllText(reportTaskPath, @"projectKey=demaconsulting_SonarMark
serverUrl=https://sonarcloud.io/
serverVersion=1.2.3.45678
dashboardUrl=http://sonarcloud.io/dashboard?id=demaconsulting_SonarMark
ceTaskId=AZvC9TxNj4Ttv7fX4CVs
ceTaskUrl=https://sonarcloud.io/api/ce/task?id=AZvC9TxNj4Ttv7fX4CVs");

        try
        {
            // Act
            var result = ReportTaskParser.Parse(reportTaskPath);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("demaconsulting_SonarMark", result.ProjectKey);
            Assert.AreEqual("https://sonarcloud.io/", result.ServerUrl);
            Assert.AreEqual("AZvC9TxNj4Ttv7fX4CVs", result.CeTaskId);
        }
        finally
        {
            Directory.Delete(testDir, true);
        }
    }

    /// <summary>
    /// Test that Parse ignores empty lines and comments
    /// </summary>
    [TestMethod]
    public void ReportTaskParser_Parse_FileWithCommentsAndEmptyLines_ParsesCorrectly()
    {
        // Arrange
        var testDir = TestDirectory;
        Directory.CreateDirectory(testDir);
        var reportTaskPath = Path.Combine(testDir, "report-task.txt");
        File.WriteAllText(reportTaskPath, @"# This is a comment
projectKey=test_project

serverUrl=https://sonarcloud.io/
# Another comment
ceTaskId=task123

");

        try
        {
            // Act
            var result = ReportTaskParser.Parse(reportTaskPath);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("test_project", result.ProjectKey);
            Assert.AreEqual("https://sonarcloud.io/", result.ServerUrl);
            Assert.AreEqual("task123", result.CeTaskId);
        }
        finally
        {
            Directory.Delete(testDir, true);
        }
    }

    /// <summary>
    /// Test that Parse handles whitespace around values
    /// </summary>
    [TestMethod]
    public void ReportTaskParser_Parse_FileWithWhitespace_TrimsValues()
    {
        // Arrange
        var testDir = TestDirectory;
        Directory.CreateDirectory(testDir);
        var reportTaskPath = Path.Combine(testDir, "report-task.txt");
        File.WriteAllText(reportTaskPath, @"projectKey = test_project 
serverUrl = https://sonarcloud.io/ 
ceTaskId = task123 ");

        try
        {
            // Act
            var result = ReportTaskParser.Parse(reportTaskPath);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("test_project", result.ProjectKey);
            Assert.AreEqual("https://sonarcloud.io/", result.ServerUrl);
            Assert.AreEqual("task123", result.CeTaskId);
        }
        finally
        {
            Directory.Delete(testDir, true);
        }
    }
}
