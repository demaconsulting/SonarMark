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
using Xunit;

namespace DemaConsulting.SonarMark.Tests.Cli;

/// <summary>
///     Subsystem tests for the CLI subsystem (Context + Program working together).
/// </summary>
[Collection("NonParallelTests")]
public class CliTests
{
    /// <summary>
    ///     Test that --version is dispatched correctly through the CLI subsystem and outputs the version string.
    /// </summary>
    [Fact]
    public void Cli_VersionDispatch_WithVersionFlag_OutputsVersionString()
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
            Assert.False(string.IsNullOrWhiteSpace(outputText));
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
    [Fact]
    public void Cli_HelpDispatch_WithHelpFlag_OutputsHelpText()
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
            Assert.Equal(0, context.ExitCode);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test that --silent mode suppresses output through the CLI subsystem.
    /// </summary>
    [Fact]
    public void Cli_SilentMode_WithSilentFlag_SuppressesOutput()
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

            // Assert - silent mode must suppress all standard output
            var outputText = output.ToString();
            Assert.True(string.IsNullOrWhiteSpace(outputText));
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test that --enforce is parsed and the flag is exposed through the CLI subsystem after dispatch.
    /// </summary>
    [Fact]
    public void Cli_EnforceMode_WithEnforceFlag_SetsEnforceFlag()
    {
        // Arrange - create context with --enforce and --server so dispatch reaches the project-key check
        using var context = Context.Create(["--enforce", "--server", "https://mock.example.com"]);

        // Act - run through Program dispatch; we expect an error about missing --project-key,
        // but Enforce must be true on the Context that was dispatched
        Program.Run(context);

        // Assert - the Enforce flag must be set to true and dispatch must have been attempted
        Assert.True(context.Enforce);
        Assert.Equal(1, context.ExitCode);
    }

    /// <summary>
    ///     Test that an unrecognized argument throws ArgumentException.
    /// </summary>
    [Fact]
    public void Cli_ArgumentParsing_WithUnknownFlag_ThrowsArgumentException()
    {
        // Act / Assert - unsupported flag must throw ArgumentException before any dispatch occurs
        Assert.Throws<ArgumentException>(() => Context.Create(["--unknown-flag"]));
    }
}

