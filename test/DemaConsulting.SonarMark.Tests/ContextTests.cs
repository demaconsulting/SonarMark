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
///     Tests for Context class
/// </summary>
[TestClass]
public class ContextTests
{
    /// <summary>
    ///     Test that Create with no arguments returns context with default values
    /// </summary>
    [TestMethod]
    public void Context_Create_NoArguments_ReturnsDefaultContext()
    {
        // Arrange
        var args = Array.Empty<string>();

        // Act
        using var context = Context.Create(args);

        // Assert
        Assert.IsFalse(context.Version);
        Assert.IsFalse(context.Help);
        Assert.IsFalse(context.Silent);
        Assert.IsFalse(context.Validate);
        Assert.IsNull(context.ReportFile);
        Assert.AreEqual(1, context.ReportDepth);
        Assert.IsNull(context.Token);
        Assert.IsNull(context.WorkingDirectory);
        Assert.AreEqual(0, context.ExitCode);
    }

    /// <summary>
    ///     Test that Create with -v flag sets Version to true
    /// </summary>
    [TestMethod]
    public void Context_Create_VersionShortFlag_SetsVersionTrue()
    {
        // Arrange
        var args = new[] { "-v" };

        // Act
        using var context = Context.Create(args);

        // Assert
        Assert.IsTrue(context.Version);
    }

    /// <summary>
    ///     Test that Create with --version flag sets Version to true
    /// </summary>
    [TestMethod]
    public void Context_Create_VersionLongFlag_SetsVersionTrue()
    {
        // Arrange
        var args = new[] { "--version" };

        // Act
        using var context = Context.Create(args);

        // Assert
        Assert.IsTrue(context.Version);
    }

    /// <summary>
    ///     Test that Create with -? flag sets Help to true
    /// </summary>
    [TestMethod]
    public void Context_Create_HelpQuestionFlag_SetsHelpTrue()
    {
        // Arrange
        var args = new[] { "-?" };

        // Act
        using var context = Context.Create(args);

        // Assert
        Assert.IsTrue(context.Help);
    }

    /// <summary>
    ///     Test that Create with -h flag sets Help to true
    /// </summary>
    [TestMethod]
    public void Context_Create_HelpShortFlag_SetsHelpTrue()
    {
        // Arrange
        var args = new[] { "-h" };

        // Act
        using var context = Context.Create(args);

        // Assert
        Assert.IsTrue(context.Help);
    }

    /// <summary>
    ///     Test that Create with --help flag sets Help to true
    /// </summary>
    [TestMethod]
    public void Context_Create_HelpLongFlag_SetsHelpTrue()
    {
        // Arrange
        var args = new[] { "--help" };

        // Act
        using var context = Context.Create(args);

        // Assert
        Assert.IsTrue(context.Help);
    }

    /// <summary>
    ///     Test that Create with --silent flag sets Silent to true
    /// </summary>
    [TestMethod]
    public void Context_Create_SilentFlag_SetsSilentTrue()
    {
        // Arrange
        var args = new[] { "--silent" };

        // Act
        using var context = Context.Create(args);

        // Assert
        Assert.IsTrue(context.Silent);
    }

    /// <summary>
    ///     Test that Create with --validate flag sets Validate to true
    /// </summary>
    [TestMethod]
    public void Context_Create_ValidateFlag_SetsValidateTrue()
    {
        // Arrange
        var args = new[] { "--validate" };

        // Act
        using var context = Context.Create(args);

        // Assert
        Assert.IsTrue(context.Validate);
    }

    /// <summary>
    ///     Test that Create with --report sets ReportFile
    /// </summary>
    [TestMethod]
    public void Context_Create_ReportFlag_SetsReportFile()
    {
        // Arrange
        var args = new[] { "--report", "output.md" };

        // Act
        using var context = Context.Create(args);

        // Assert
        Assert.AreEqual("output.md", context.ReportFile);
    }

    /// <summary>
    ///     Test that Create with --report without value throws ArgumentException
    /// </summary>
    [TestMethod]
    public void Context_Create_ReportFlagWithoutValue_ThrowsArgumentException()
    {
        // Arrange
        var args = new[] { "--report" };

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => Context.Create(args));
        Assert.AreEqual("args", ex.ParamName);
        StringAssert.Contains(ex.Message, "--report requires a filename argument");
    }

    /// <summary>
    ///     Test that Create with --report-depth sets ReportDepth
    /// </summary>
    [TestMethod]
    public void Context_Create_ReportDepthFlag_SetsReportDepth()
    {
        // Arrange
        var args = new[] { "--report-depth", "3" };

        // Act
        using var context = Context.Create(args);

        // Assert
        Assert.AreEqual(3, context.ReportDepth);
    }

    /// <summary>
    ///     Test that Create with --report-depth without value throws ArgumentException
    /// </summary>
    [TestMethod]
    public void Context_Create_ReportDepthFlagWithoutValue_ThrowsArgumentException()
    {
        // Arrange
        var args = new[] { "--report-depth" };

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => Context.Create(args));
        Assert.AreEqual("args", ex.ParamName);
        StringAssert.Contains(ex.Message, "--report-depth requires a depth argument");
    }

    /// <summary>
    ///     Test that Create with --report-depth with invalid value throws ArgumentException
    /// </summary>
    [TestMethod]
    public void Context_Create_ReportDepthFlagWithInvalidValue_ThrowsArgumentException()
    {
        // Arrange
        var args = new[] { "--report-depth", "invalid" };

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => Context.Create(args));
        Assert.AreEqual("args", ex.ParamName);
        StringAssert.Contains(ex.Message, "--report-depth requires a positive integer");
    }

    /// <summary>
    ///     Test that Create with --report-depth with zero throws ArgumentException
    /// </summary>
    [TestMethod]
    public void Context_Create_ReportDepthFlagWithZero_ThrowsArgumentException()
    {
        // Arrange
        var args = new[] { "--report-depth", "0" };

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => Context.Create(args));
        Assert.AreEqual("args", ex.ParamName);
        StringAssert.Contains(ex.Message, "--report-depth requires a positive integer");
    }

    /// <summary>
    ///     Test that Create with --report-depth with negative value throws ArgumentException
    /// </summary>
    [TestMethod]
    public void Context_Create_ReportDepthFlagWithNegativeValue_ThrowsArgumentException()
    {
        // Arrange
        var args = new[] { "--report-depth", "-1" };

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => Context.Create(args));
        Assert.AreEqual("args", ex.ParamName);
        StringAssert.Contains(ex.Message, "--report-depth requires a positive integer");
    }

    /// <summary>
    ///     Test that Create with --token sets Token
    /// </summary>
    [TestMethod]
    public void Context_Create_TokenFlag_SetsToken()
    {
        // Arrange
        var args = new[] { "--token", "test-token-123" };

        // Act
        using var context = Context.Create(args);

        // Assert
        Assert.AreEqual("test-token-123", context.Token);
    }

    /// <summary>
    ///     Test that Create with --token without value throws ArgumentException
    /// </summary>
    [TestMethod]
    public void Context_Create_TokenFlagWithoutValue_ThrowsArgumentException()
    {
        // Arrange
        var args = new[] { "--token" };

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => Context.Create(args));
        Assert.AreEqual("args", ex.ParamName);
        StringAssert.Contains(ex.Message, "--token requires a token argument");
    }

    /// <summary>
    ///     Test that Create with --working-directory sets WorkingDirectory
    /// </summary>
    [TestMethod]
    public void Context_Create_WorkingDirectoryFlag_SetsWorkingDirectory()
    {
        // Arrange
        var args = new[] { "--working-directory", "/path/to/dir" };

        // Act
        using var context = Context.Create(args);

        // Assert
        Assert.AreEqual("/path/to/dir", context.WorkingDirectory);
    }

    /// <summary>
    ///     Test that Create with --working-directory without value throws ArgumentException
    /// </summary>
    [TestMethod]
    public void Context_Create_WorkingDirectoryFlagWithoutValue_ThrowsArgumentException()
    {
        // Arrange
        var args = new[] { "--working-directory" };

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => Context.Create(args));
        Assert.AreEqual("args", ex.ParamName);
        StringAssert.Contains(ex.Message, "--working-directory requires a directory argument");
    }

    /// <summary>
    ///     Test that Create with --log creates log file
    /// </summary>
    [TestMethod]
    public void Context_Create_LogFlag_CreatesLogFile()
    {
        // Arrange
        var logFile = Path.GetTempFileName();
        try
        {
            var args = new[] { "--log", logFile };

            // Act
            using var context = Context.Create(args);

            // Assert
            Assert.IsTrue(File.Exists(logFile));
        }
        finally
        {
            // Cleanup
            if (File.Exists(logFile))
            {
                File.Delete(logFile);
            }
        }
    }

    /// <summary>
    ///     Test that Create with --log without value throws ArgumentException
    /// </summary>
    [TestMethod]
    public void Context_Create_LogFlagWithoutValue_ThrowsArgumentException()
    {
        // Arrange
        var args = new[] { "--log" };

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => Context.Create(args));
        Assert.AreEqual("args", ex.ParamName);
        StringAssert.Contains(ex.Message, "--log requires a filename argument");
    }

    /// <summary>
    ///     Test that Create with invalid log path throws ArgumentException
    /// </summary>
    [TestMethod]
    public void Context_Create_LogFlagWithInvalidPath_ThrowsArgumentException()
    {
        // Arrange
        var args = new[] { "--log", "/invalid/path/that/does/not/exist/log.txt" };

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => Context.Create(args));
        Assert.AreEqual("args", ex.ParamName);
        StringAssert.Contains(ex.Message, "Failed to open log file");
    }

    /// <summary>
    ///     Test that Create with unsupported argument throws ArgumentException
    /// </summary>
    [TestMethod]
    public void Context_Create_UnsupportedArgument_ThrowsArgumentException()
    {
        // Arrange
        var args = new[] { "--unsupported" };

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => Context.Create(args));
        Assert.AreEqual("args", ex.ParamName);
        StringAssert.Contains(ex.Message, "Unsupported argument '--unsupported'");
    }

    /// <summary>
    ///     Test that Create with multiple arguments sets all properties
    /// </summary>
    [TestMethod]
    public void Context_Create_MultipleArguments_SetsAllProperties()
    {
        // Arrange
        var args = new[] { "--silent", "--validate", "--report", "output.md", "--report-depth", "2", "--token", "token123", "--working-directory", "/work/dir" };

        // Act
        using var context = Context.Create(args);

        // Assert
        Assert.IsTrue(context.Silent);
        Assert.IsTrue(context.Validate);
        Assert.AreEqual("output.md", context.ReportFile);
        Assert.AreEqual(2, context.ReportDepth);
        Assert.AreEqual("token123", context.Token);
        Assert.AreEqual("/work/dir", context.WorkingDirectory);
    }

    /// <summary>
    ///     Test that WriteLine with log file writes to log file
    /// </summary>
    [TestMethod]
    public void Context_WriteLine_WithLogFile_WritesToLogFile()
    {
        // Arrange
        var logFile = Path.GetTempFileName();
        try
        {
            var args = new[] { "--log", logFile };
            using var context = Context.Create(args);

            // Act
            context.WriteLine("Test message");
            context.Dispose();

            // Assert
            var logContent = File.ReadAllText(logFile);
            StringAssert.Contains(logContent, "Test message");
        }
        finally
        {
            if (File.Exists(logFile))
            {
                File.Delete(logFile);
            }
        }
    }

    /// <summary>
    ///     Test that WriteError with log file writes to log file
    /// </summary>
    [TestMethod]
    public void Context_WriteError_WithLogFile_WritesToLogFile()
    {
        // Arrange
        var logFile = Path.GetTempFileName();
        try
        {
            var args = new[] { "--log", logFile };
            using var context = Context.Create(args);

            // Act
            context.WriteError("Error message");
            context.Dispose();

            // Assert
            var logContent = File.ReadAllText(logFile);
            StringAssert.Contains(logContent, "Error message");
        }
        finally
        {
            if (File.Exists(logFile))
            {
                File.Delete(logFile);
            }
        }
    }

    /// <summary>
    ///     Test that WriteError sets ExitCode to 1
    /// </summary>
    [TestMethod]
    public void Context_WriteError_SetsExitCodeToOne()
    {
        // Arrange
        var args = Array.Empty<string>();
        using var context = Context.Create(args);

        // Assert initial state
        Assert.AreEqual(0, context.ExitCode);

        // Act
        context.WriteError("Error message");

        // Assert
        Assert.AreEqual(1, context.ExitCode);
    }

    /// <summary>
    ///     Test that Dispose closes log file
    /// </summary>
    [TestMethod]
    public void Context_Dispose_ClosesLogFile()
    {
        // Arrange
        var logFile = Path.GetTempFileName();
        try
        {
            var args = new[] { "--log", logFile };
            var context = Context.Create(args);

            // Act
            context.Dispose();

            // Assert - should be able to delete the file after disposal
            File.Delete(logFile);
            Assert.IsFalse(File.Exists(logFile));
        }
        finally
        {
            if (File.Exists(logFile))
            {
                File.Delete(logFile);
            }
        }
    }
}
