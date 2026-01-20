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
///     Unit tests for the Context class.
/// </summary>
[TestClass]
public class ContextTests
{
    private string _testDirectory = string.Empty;

    /// <summary>
    ///     Initialize test by creating a temporary test directory.
    /// </summary>
    [TestInitialize]
    public void TestInitialize()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), $"sonarmark_test_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testDirectory);
    }

    /// <summary>
    ///     Clean up test by deleting the temporary test directory.
    /// </summary>
    [TestCleanup]
    public void TestCleanup()
    {
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, recursive: true);
        }
    }


    /// <summary>
    ///     Test creating a context with no arguments.
    /// </summary>
    [TestMethod]
    public void Context_Create_NoArguments_ReturnsDefaultContext()
    {
        using var context = Context.Create([]);

        Assert.IsFalse(context.Version);
        Assert.IsFalse(context.Help);
        Assert.IsFalse(context.Silent);
        Assert.IsFalse(context.Validate);
        Assert.IsNull(context.ReportFile);
        Assert.AreEqual(1, context.ReportDepth);
        Assert.IsNull(context.Token);
        Assert.IsNull(context.Server);
        Assert.IsNull(context.ProjectKey);
        Assert.IsNull(context.Branch);
        Assert.AreEqual(0, context.ExitCode);
    }

    /// <summary>
    ///     Test creating a context with version flag.
    /// </summary>
    [TestMethod]
    public void Context_Create_VersionFlag_SetsVersionProperty()
    {
        using var context1 = Context.Create(["-v"]);
        Assert.IsTrue(context1.Version);
        Assert.AreEqual(0, context1.ExitCode);

        using var context2 = Context.Create(["--version"]);
        Assert.IsTrue(context2.Version);
        Assert.AreEqual(0, context2.ExitCode);
    }

    /// <summary>
    ///     Test creating a context with help flags.
    /// </summary>
    [TestMethod]
    public void Context_Create_HelpFlags_SetsHelpProperty()
    {
        using var context1 = Context.Create(["-?"]);
        Assert.IsTrue(context1.Help);
        Assert.AreEqual(0, context1.ExitCode);

        using var context2 = Context.Create(["-h"]);
        Assert.IsTrue(context2.Help);
        Assert.AreEqual(0, context2.ExitCode);

        using var context3 = Context.Create(["--help"]);
        Assert.IsTrue(context3.Help);
        Assert.AreEqual(0, context3.ExitCode);
    }

    /// <summary>
    ///     Test creating a context with silent flag.
    /// </summary>
    [TestMethod]
    public void Context_Create_SilentFlag_SetsSilentProperty()
    {
        using var context = Context.Create(["--silent"]);

        Assert.IsTrue(context.Silent);
        Assert.AreEqual(0, context.ExitCode);
    }

    /// <summary>
    ///     Test creating a context with validate flag.
    /// </summary>
    [TestMethod]
    public void Context_Create_ValidateFlag_SetsValidateProperty()
    {
        using var context = Context.Create(["--validate"]);

        Assert.IsTrue(context.Validate);
        Assert.AreEqual(0, context.ExitCode);
    }

    /// <summary>
    ///     Test creating a context with enforce flag.
    /// </summary>
    [TestMethod]
    public void Context_Create_EnforceFlag_SetsEnforceProperty()
    {
        using var context = Context.Create(["--enforce"]);

        Assert.IsTrue(context.Enforce);
        Assert.AreEqual(0, context.ExitCode);
    }

    /// <summary>
    ///     Test creating a context with report file.
    /// </summary>
    [TestMethod]
    public void Context_Create_ReportFile_SetsReportProperty()
    {
        using var context = Context.Create(["--report", "report.md"]);

        Assert.AreEqual("report.md", context.ReportFile);
        Assert.AreEqual(0, context.ExitCode);
    }

    /// <summary>
    ///     Test creating a context with missing report filename.
    /// </summary>
    [TestMethod]
    public void Context_Create_MissingReportFilename_ThrowsException()
    {
        try
        {
            Context.Create(["--report"]);
            Assert.Fail("Expected ArgumentException was not thrown");
        }
        catch (ArgumentException ex)
        {
            Assert.Contains("--report requires a filename argument", ex.Message);
        }
    }

    /// <summary>
    ///     Test creating a context with report depth.
    /// </summary>
    [TestMethod]
    public void Context_Create_ReportDepth_SetsReportDepthProperty()
    {
        using var context = Context.Create(["--report-depth", "3"]);

        Assert.AreEqual(3, context.ReportDepth);
        Assert.AreEqual(0, context.ExitCode);
    }

    /// <summary>
    ///     Test creating a context with missing report depth.
    /// </summary>
    [TestMethod]
    public void Context_Create_MissingReportDepth_ThrowsException()
    {
        try
        {
            Context.Create(["--report-depth"]);
            Assert.Fail("Expected ArgumentException was not thrown");
        }
        catch (ArgumentException ex)
        {
            Assert.Contains("--report-depth requires a depth argument", ex.Message);
        }
    }

    /// <summary>
    ///     Test creating a context with invalid report depth.
    /// </summary>
    [TestMethod]
    public void Context_Create_InvalidReportDepth_ThrowsException()
    {
        try
        {
            Context.Create(["--report-depth", "invalid"]);
            Assert.Fail("Expected ArgumentException was not thrown");
        }
        catch (ArgumentException ex)
        {
            Assert.Contains("--report-depth requires a positive integer", ex.Message);
        }

        try
        {
            Context.Create(["--report-depth", "0"]);
            Assert.Fail("Expected ArgumentException was not thrown");
        }
        catch (ArgumentException ex)
        {
            Assert.Contains("--report-depth requires a positive integer", ex.Message);
        }

        try
        {
            Context.Create(["--report-depth", "-1"]);
            Assert.Fail("Expected ArgumentException was not thrown");
        }
        catch (ArgumentException ex)
        {
            Assert.Contains("--report-depth requires a positive integer", ex.Message);
        }
    }

    /// <summary>
    ///     Test creating a context with token.
    /// </summary>
    [TestMethod]
    public void Context_Create_Token_SetsTokenProperty()
    {
        using var context = Context.Create(["--token", "test-token-123"]);

        Assert.AreEqual("test-token-123", context.Token);
        Assert.AreEqual(0, context.ExitCode);
    }

    /// <summary>
    ///     Test creating a context with missing token.
    /// </summary>
    [TestMethod]
    public void Context_Create_MissingToken_ThrowsException()
    {
        try
        {
            Context.Create(["--token"]);
            Assert.Fail("Expected ArgumentException was not thrown");
        }
        catch (ArgumentException ex)
        {
            Assert.Contains("--token requires a token argument", ex.Message);
        }
    }

    /// <summary>
    ///     Test creating a context with server URL.
    /// </summary>
    [TestMethod]
    public void Context_Create_Server_SetsServerProperty()
    {
        using var context = Context.Create(["--server", "https://sonarcloud.io"]);

        Assert.AreEqual("https://sonarcloud.io", context.Server);
        Assert.AreEqual(0, context.ExitCode);
    }

    /// <summary>
    ///     Test creating a context with missing server URL.
    /// </summary>
    [TestMethod]
    public void Context_Create_MissingServer_ThrowsException()
    {
        try
        {
            Context.Create(["--server"]);
            Assert.Fail("Expected ArgumentException was not thrown");
        }
        catch (ArgumentException ex)
        {
            Assert.Contains("--server requires a server URL argument", ex.Message);
        }
    }

    /// <summary>
    ///     Test creating a context with project key.
    /// </summary>
    [TestMethod]
    public void Context_Create_ProjectKey_SetsProjectKeyProperty()
    {
        using var context = Context.Create(["--project-key", "my-project"]);

        Assert.AreEqual("my-project", context.ProjectKey);
        Assert.AreEqual(0, context.ExitCode);
    }

    /// <summary>
    ///     Test creating a context with missing project key.
    /// </summary>
    [TestMethod]
    public void Context_Create_MissingProjectKey_ThrowsException()
    {
        try
        {
            Context.Create(["--project-key"]);
            Assert.Fail("Expected ArgumentException was not thrown");
        }
        catch (ArgumentException ex)
        {
            Assert.Contains("--project-key requires a project key argument", ex.Message);
        }
    }

    /// <summary>
    ///     Test creating a context with branch.
    /// </summary>
    [TestMethod]
    public void Context_Create_Branch_SetsBranchProperty()
    {
        using var context = Context.Create(["--branch", "main"]);

        Assert.AreEqual("main", context.Branch);
        Assert.AreEqual(0, context.ExitCode);
    }

    /// <summary>
    ///     Test creating a context with missing branch.
    /// </summary>
    [TestMethod]
    public void Context_Create_MissingBranch_ThrowsException()
    {
        try
        {
            Context.Create(["--branch"]);
            Assert.Fail("Expected ArgumentException was not thrown");
        }
        catch (ArgumentException ex)
        {
            Assert.Contains("--branch requires a branch name argument", ex.Message);
        }
    }

    /// <summary>
    ///     Test creating a context with missing log filename.
    /// </summary>
    [TestMethod]
    public void Context_Create_MissingLogFilename_ThrowsException()
    {
        try
        {
            Context.Create(["--log"]);
            Assert.Fail("Expected ArgumentException was not thrown");
        }
        catch (ArgumentException ex)
        {
            Assert.Contains("--log requires a filename argument", ex.Message);
        }
    }

    /// <summary>
    ///     Test creating a context with unsupported argument.
    /// </summary>
    [TestMethod]
    public void Context_Create_UnsupportedArgument_ThrowsException()
    {
        try
        {
            Context.Create(["--unsupported"]);
            Assert.Fail("Expected ArgumentException was not thrown");
        }
        catch (ArgumentException ex)
        {
            Assert.Contains("Unsupported argument '--unsupported'", ex.Message);
        }
    }

    /// <summary>
    ///     Test WriteLine writes to console.
    /// </summary>
    [TestMethod]
    public void Context_WriteLine_NormalMode_WritesToConsole()
    {
        var originalOut = Console.Out;
        var output = new StringWriter();
        Console.SetOut(output);

        try
        {
            using var context = Context.Create([]);
            context.WriteLine("Test message");

            Assert.AreEqual("Test message" + Environment.NewLine, output.ToString());
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test WriteLine in silent mode doesn't write to console.
    /// </summary>
    [TestMethod]
    public void Context_WriteLine_SilentMode_DoesNotWriteToConsole()
    {
        var originalOut = Console.Out;
        var output = new StringWriter();
        Console.SetOut(output);

        try
        {
            using var context = Context.Create(["--silent"]);
            context.WriteLine("Test message");

            Assert.AreEqual(string.Empty, output.ToString());
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test WriteError writes to console.
    /// </summary>
    [TestMethod]
    public void Context_WriteError_NormalMode_WritesToConsole()
    {
        var originalOut = Console.Out;
        var output = new StringWriter();
        Console.SetOut(output);

        try
        {
            using var context = Context.Create([]);
            context.WriteError("Error message");

            Assert.AreEqual("Error message" + Environment.NewLine, output.ToString());
            Assert.AreEqual(1, context.ExitCode);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test WriteError in silent mode doesn't write to console.
    /// </summary>
    [TestMethod]
    public void Context_WriteError_SilentMode_DoesNotWriteToConsole()
    {
        var originalOut = Console.Out;
        var output = new StringWriter();
        Console.SetOut(output);

        try
        {
            using var context = Context.Create(["--silent"]);
            context.WriteError("Error message");

            Assert.AreEqual(string.Empty, output.ToString());
            Assert.AreEqual(1, context.ExitCode);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test log file creation and writing.
    /// </summary>
    [TestMethod]
    public void Context_Create_WithLogFile_WritesToLogFile()
    {
        var logPath = Path.Combine(_testDirectory, "test.log");

        using (var context = Context.Create(["--log", logPath, "--silent"]))
        {
            context.WriteLine("Normal message");
            context.WriteError("Error message");
        }

        Assert.IsTrue(File.Exists(logPath));
        var logContent = File.ReadAllText(logPath);
        Assert.Contains("Normal message", logContent);
        Assert.Contains("Error message", logContent);
    }
}
