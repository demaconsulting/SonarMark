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

using DemaConsulting.SonarMark.Cli;

namespace DemaConsulting.SonarMark.Tests.SelfTest;

/// <summary>
///     Subsystem tests for the SelfTest subsystem (Validation running end-to-end self-validation pipeline).
/// </summary>
[TestClass]
public class SelfTestTests
{
    private string _testDirectory = string.Empty;

    /// <summary>
    ///     Initialize test by creating a temporary test directory.
    /// </summary>
    [TestInitialize]
    public void TestInitialize()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), $"sonarmark_selftest_{Guid.NewGuid()}");
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
    ///     Test that the SelfTest subsystem completes all self-validation tests with exit code 0.
    /// </summary>
    [TestMethod]
    public void SelfTest_RunValidation_AllTestsPass()
    {
        // Arrange - silent context suppresses console output during subsystem test
        using var context = Context.Create(["--silent"]);

        // Act - run the full self-validation pipeline through the Program entry point
        Program.Run(Context.Create(["--validate", "--silent"]));

        // Assert - all self-validation tests pass so the subsystem exit code is 0
        Assert.AreEqual(0, context.ExitCode);
    }

    /// <summary>
    ///     Test that the SelfTest subsystem produces a valid results file.
    /// </summary>
    [TestMethod]
    public void SelfTest_RunValidation_ProducesResultsFile()
    {
        // Arrange - configure a TRX results file path and silent output
        var resultsPath = Path.Combine(_testDirectory, "selftest-results.trx");
        using var context = Context.Create(["--validate", "--silent", "--results", resultsPath]);

        // Act - run the full self-validation pipeline; the subsystem must write the results file
        Program.Run(context);

        // Assert - the results file must exist and contain self-validation test suite content
        Assert.IsTrue(File.Exists(resultsPath), $"Expected results file at {resultsPath}");
        var content = File.ReadAllText(resultsPath);
        Assert.Contains("SonarMark Self-Validation", content);
    }
}
