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
///     Integration tests that run the SonarMark application through dotnet
/// </summary>
[TestClass]
public class IntegrationTests
{
    private string _dllPath = string.Empty;

    /// <summary>
    ///     Initialize test by locating the SonarMark DLL
    /// </summary>
    [TestInitialize]
    public void TestInitialize()
    {
        // The DLL should be in the same directory as the test assembly
        // because the test project references the main project
        var baseDir = AppContext.BaseDirectory;
        _dllPath = Path.Combine(baseDir, "DemaConsulting.SonarMark.dll");
        
        Assert.IsTrue(File.Exists(_dllPath), $"Could not find SonarMark DLL at {_dllPath}");
    }

    /// <summary>
    ///     Test that version flag outputs version information
    /// </summary>
    [TestMethod]
    public void IntegrationTest_VersionFlag_OutputsVersion()
    {
        // Run the application with --version flag
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--version");

        // Verify success
        Assert.AreEqual(0, exitCode);

        // Verify version is output
        Assert.IsFalse(string.IsNullOrWhiteSpace(output));
        Assert.DoesNotContain("Error", output);
    }

    /// <summary>
    ///     Test that help flag outputs usage information
    /// </summary>
    [TestMethod]
    public void IntegrationTest_HelpFlag_OutputsUsageInformation()
    {
        // Run the application with --help flag
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--help");

        // Verify success
        Assert.AreEqual(0, exitCode);

        // Verify usage information
        Assert.Contains("Usage: sonarmark", output);
        Assert.Contains("Options:", output);
        Assert.Contains("--version", output);
        Assert.Contains("--help", output);
        Assert.Contains("--server", output);
        Assert.Contains("--project-key", output);
    }

    /// <summary>
    ///     Test that validate flag outputs not implemented message
    /// </summary>
    [TestMethod]
    public void IntegrationTest_ValidateFlag_OutputsNotImplemented()
    {
        // Run the application with --validate flag
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--validate");

        // Verify success
        Assert.AreEqual(0, exitCode);

        // Verify not implemented message
        Assert.Contains("Self-validation not yet implemented", output);
    }

    /// <summary>
    ///     Test that missing server parameter shows error
    /// </summary>
    [TestMethod]
    public void IntegrationTest_MissingServerParameter_ShowsError()
    {
        // Run the application without required parameters
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath);

        // Verify error exit code
        Assert.AreEqual(1, exitCode);

        // Verify error message
        Assert.Contains("--server parameter is required", output);
    }

    /// <summary>
    ///     Test that missing project-key parameter shows error
    /// </summary>
    [TestMethod]
    public void IntegrationTest_MissingProjectKeyParameter_ShowsError()
    {
        // Run the application with server but without project-key
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--server", "https://sonarcloud.io");

        // Verify error exit code
        Assert.AreEqual(1, exitCode);

        // Verify error message
        Assert.Contains("--project-key parameter is required", output);
    }

    /// <summary>
    ///     Test that silent flag suppresses output
    /// </summary>
    [TestMethod]
    public void IntegrationTest_SilentFlag_SuppressesOutput()
    {
        // Run the application with --silent flag
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--silent");

        // Verify error exit code (missing required parameters)
        Assert.AreEqual(1, exitCode);

        // Verify no banner in output
        Assert.DoesNotContain("SonarMark version", output);
    }

    /// <summary>
    ///     Test that invalid argument shows error
    /// </summary>
    [TestMethod]
    public void IntegrationTest_InvalidArgument_ShowsError()
    {
        // Run the application with invalid argument
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--invalid-argument");

        // Verify error exit code
        Assert.AreEqual(1, exitCode);

        // Verify error message
        Assert.Contains("Error:", output);
        Assert.Contains("Unsupported argument", output);
    }

    /// <summary>
    ///     Test that report-depth without value shows error
    /// </summary>
    [TestMethod]
    public void IntegrationTest_ReportDepthWithoutValue_ShowsError()
    {
        // Run the application with --report-depth but no value
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--report-depth");

        // Verify error exit code
        Assert.AreEqual(1, exitCode);

        // Verify error message
        Assert.Contains("Error:", output);
        Assert.Contains("--report-depth requires a depth argument", output);
    }

    /// <summary>
    ///     Test that report-depth with invalid value shows error
    /// </summary>
    [TestMethod]
    public void IntegrationTest_ReportDepthWithInvalidValue_ShowsError()
    {
        // Run the application with --report-depth and invalid value
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--report-depth", "invalid");

        // Verify error exit code
        Assert.AreEqual(1, exitCode);

        // Verify error message
        Assert.Contains("Error:", output);
        Assert.Contains("--report-depth requires a positive integer", output);
    }

    /// <summary>
    ///     Test that report-depth with zero shows error
    /// </summary>
    [TestMethod]
    public void IntegrationTest_ReportDepthWithZero_ShowsError()
    {
        // Run the application with --report-depth 0
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--report-depth", "0");

        // Verify error exit code
        Assert.AreEqual(1, exitCode);

        // Verify error message
        Assert.Contains("Error:", output);
        Assert.Contains("--report-depth requires a positive integer", output);
    }

    /// <summary>
    ///     Test that token parameter is accepted
    /// </summary>
    [TestMethod]
    public void IntegrationTest_TokenParameter_IsAccepted()
    {
        // Run the application with token parameter
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--token", "test-token");

        // Verify error exit code (missing server and project-key)
        Assert.AreEqual(1, exitCode);

        // Verify it's not an argument error
        Assert.DoesNotContain("Unsupported argument", output);
    }

    /// <summary>
    ///     Test that branch parameter is accepted
    /// </summary>
    [TestMethod]
    public void IntegrationTest_BranchParameter_IsAccepted()
    {
        // Run the application with branch parameter
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--branch", "main");

        // Verify error exit code (missing server and project-key)
        Assert.AreEqual(1, exitCode);

        // Verify it's not an argument error
        Assert.DoesNotContain("Unsupported argument", output);
    }

    /// <summary>
    ///     Test that enforce flag is accepted
    /// </summary>
    [TestMethod]
    public void IntegrationTest_EnforceFlag_IsAccepted()
    {
        // Run the application with enforce flag
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--enforce");

        // Verify error exit code (missing server and project-key)
        Assert.AreEqual(1, exitCode);

        // Verify it's not an argument error
        Assert.DoesNotContain("Unsupported argument", output);
    }

    /// <summary>
    ///     Test that report parameter is accepted
    /// </summary>
    [TestMethod]
    public void IntegrationTest_ReportParameter_IsAccepted()
    {
        // Run the application with report parameter
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--report", "output.md");

        // Verify error exit code (missing server and project-key)
        Assert.AreEqual(1, exitCode);

        // Verify it's not an argument error
        Assert.DoesNotContain("Unsupported argument", output);
    }
}
