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
using DemaConsulting.SonarMark.SelfTest;

namespace DemaConsulting.SonarMark.Tests.SelfTest;

/// <summary>
///     Unit tests for the Validation class.
/// </summary>
[TestClass]
public class ValidationTests
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
    ///     Test that Validation.Run throws ArgumentNullException when context is null.
    /// </summary>
    [TestMethod]
    public void Validation_Run_WithNullContext_ThrowsArgumentNullException()
    {
        // Arrange - null context is the subject of this test

        // Act / Assert - calling Run with null should throw ArgumentNullException
        Assert.ThrowsExactly<ArgumentNullException>(() => Validation.Run(null!));
    }

    /// <summary>
    ///     Test that Validation.Run completes successfully with a silent context.
    /// </summary>
    [TestMethod]
    public void Validation_Run_WithSilentContext_CompletesWithZeroExitCode()
    {
        // Arrange - silent context suppresses console output; no results file needed
        using var context = Context.Create(["--silent"]);

        // Act - run self-validation
        Validation.Run(context);

        // Assert - all 4 self-validation tests pass, so exit code stays 0
        Assert.AreEqual(0, context.ExitCode);
    }

    /// <summary>
    ///     Test that Validation.Run prints the expected header text to stdout.
    /// </summary>
    [TestMethod]
    public void Validation_Run_OutputsValidationHeader()
    {
        // Arrange - capture stdout to verify header content
        var originalOut = Console.Out;
        using var output = new StringWriter();
        Console.SetOut(output);

        try
        {
            using var context = Context.Create([]);

            // Act - run self-validation so the header is emitted
            Validation.Run(context);

            // Assert - header must identify the tool by name and include its version
            var text = output.ToString();
            Assert.Contains("DEMA Consulting SonarMark", text);
            Assert.Contains("SonarMark Version", text);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test that Validation.Run respects the --depth option for the validation report header.
    /// </summary>
    [TestMethod]
    public void Validation_Run_WithDepth2_OutputsLevel2Header()
    {
        // Arrange - capture stdout to verify header uses depth-2 heading
        var originalOut = Console.Out;
        using var output = new StringWriter();
        Console.SetOut(output);

        try
        {
            // Act - run self-validation with depth=2
            using var context = Context.Create(["--depth", "2"]);
            Validation.Run(context);

            // Assert - header must use ## (level-2) heading
            var outputText = output.ToString();
            Assert.Contains("## DEMA Consulting SonarMark", outputText);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test that Validation.Run reports exactly 4 passed tests and 0 failures.
    /// </summary>
    [TestMethod]
    public void Validation_Run_ReportsFourPassedTests()
    {
        // Arrange - capture stdout to inspect the summary lines
        var originalOut = Console.Out;
        using var output = new StringWriter();
        Console.SetOut(output);

        try
        {
            using var context = Context.Create([]);

            // Act - run all 4 internal self-validation tests
            Validation.Run(context);

            // Assert - summary must show all 4 tests ran and all 4 passed
            var text = output.ToString();
            Assert.Contains("Total Tests: 4", text);
            Assert.Contains("Passed: 4", text);
            Assert.Contains("Failed: 0", text);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test that Validation.Run writes a valid TRX file when the results path ends with .trx.
    /// </summary>
    [TestMethod]
    public void Validation_Run_WithTrxResultsFile_WritesTrxFile()
    {
        // Arrange - provide a .trx results path so TRX output is triggered
        var trxPath = Path.Combine(_testDirectory, "results.trx");
        using var context = Context.Create(["--silent", "--results", trxPath]);

        // Act - run self-validation which should write the TRX file
        Validation.Run(context);

        // Assert - the file must exist and reference the self-validation suite name
        Assert.IsTrue(File.Exists(trxPath));
        var content = File.ReadAllText(trxPath);
        Assert.Contains("SonarMark Self-Validation", content);
    }

    /// <summary>
    ///     Test that Validation.Run writes a valid JUnit XML file when the results path ends with .xml.
    /// </summary>
    [TestMethod]
    public void Validation_Run_WithXmlResultsFile_WritesJUnitFile()
    {
        // Arrange - provide a .xml results path so JUnit XML output is triggered
        var xmlPath = Path.Combine(_testDirectory, "results.xml");
        using var context = Context.Create(["--silent", "--results", xmlPath]);

        // Act - run self-validation which should write the JUnit XML file
        Validation.Run(context);

        // Assert - the file must exist and reference the self-validation suite name
        Assert.IsTrue(File.Exists(xmlPath));
        var content = File.ReadAllText(xmlPath);
        Assert.Contains("SonarMark Self-Validation", content);
    }

    /// <summary>
    ///     Test that Validation.Run reports an error and sets exit code 1 for an unsupported results extension.
    /// </summary>
    [TestMethod]
    public void Validation_Run_WithUnsupportedResultsExtension_ReportsError()
    {
        // Arrange - capture both stdout (suppress noise) and stderr (capture error message)
        var originalOut = Console.Out;
        var originalError = Console.Error;
        using var suppressedOut = new StringWriter();
        using var error = new StringWriter();
        Console.SetOut(suppressedOut);
        Console.SetError(error);

        try
        {
            // A .csv extension is not a recognized results format
            var csvPath = Path.Combine(_testDirectory, "results.csv");
            using var context = Context.Create(["--results", csvPath]);

            // Act - run self-validation; the unsupported extension should trigger WriteError
            Validation.Run(context);

            // Assert - exit code must be 1 and the error message must describe the problem
            Assert.AreEqual(1, context.ExitCode);
            Assert.Contains("Unsupported results file format", error.ToString());
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetError(originalError);
        }
    }
}
