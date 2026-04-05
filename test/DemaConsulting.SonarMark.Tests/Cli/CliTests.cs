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

namespace DemaConsulting.SonarMark.Tests.Cli;

/// <summary>
///     Subsystem tests for the CLI subsystem (Context + Program working together).
/// </summary>
[TestClass]
public class CliTests
{
    /// <summary>
    ///     Test that --version is dispatched correctly through the CLI subsystem and outputs the version string.
    /// </summary>
    [TestMethod]
    public void Cli_VersionDispatch_OutputsVersionString()
    {
        // Arrange - capture console output
        var originalOut = Console.Out;
        using var output = new StringWriter();
        Console.SetOut(output);

        try
        {
            // Act - create context with --version and run through Program dispatch
            using var context = Context.Create(["--version"]);
            Program.Run(context);

            // Assert - output must contain the version string produced by the CLI subsystem
            var outputText = output.ToString();
            Assert.IsFalse(string.IsNullOrWhiteSpace(outputText));
            Assert.Contains(Program.Version, outputText);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test that --help is dispatched correctly through the CLI subsystem and outputs help text.
    /// </summary>
    [TestMethod]
    public void Cli_HelpDispatch_OutputsHelpText()
    {
        // Arrange - capture console output
        var originalOut = Console.Out;
        using var output = new StringWriter();
        Console.SetOut(output);

        try
        {
            // Act - create context with --help and run through Program dispatch
            using var context = Context.Create(["--help"]);
            Program.Run(context);

            // Assert - output must contain usage/help information
            var outputText = output.ToString();
            Assert.Contains("Usage:", outputText);
            Assert.AreEqual(0, context.ExitCode);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test that --silent mode suppresses output through the CLI subsystem.
    /// </summary>
    [TestMethod]
    public void Cli_SilentMode_SuppressesOutput()
    {
        // Arrange - capture console output
        var originalOut = Console.Out;
        using var output = new StringWriter();
        Console.SetOut(output);

        try
        {
            // Act - create context with --silent and run through Program dispatch
            using var context = Context.Create(["--silent"]);
            Program.Run(context);

            // Assert - silent mode must suppress banner/version output
            var outputText = output.ToString();
            Assert.DoesNotContain("SonarMark version", outputText);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test that --enforce is parsed and made available through the CLI subsystem.
    /// </summary>
    [TestMethod]
    public void Cli_EnforceMode_SetsEnforceFlag()
    {
        // Arrange - create context with --enforce (and required --server so we do not fail before enforce check)
        // We expect an error about missing --project-key, but Enforce must be true by the time Context is created
        using var context = Context.Create(["--enforce", "--server", "https://mock.example.com"]);

        // Assert - the Enforce flag must be set to true after CLI argument parsing
        Assert.IsTrue(context.Enforce);
    }
}
