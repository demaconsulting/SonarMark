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

using System.Reflection;
using DemaConsulting.SonarMark.Cli;
using DemaConsulting.SonarMark.ReportGeneration;
using DemaConsulting.SonarMark.SelfTest;
using DemaConsulting.SonarMark.SonarIntegration;

namespace DemaConsulting.SonarMark;

/// <summary>
///     Main program entry point for SonarMark application
/// </summary>
internal static class Program
{
    /// <summary>
    ///     Gets the application version string.
    /// </summary>
    /// <value>
    ///     Version string resolved via a three-step fallback chain: (1)
    ///     <see cref="AssemblyInformationalVersionAttribute"/> (set by the SDK from the project version on
    ///     publish), (2) <see cref="System.Version.ToString()"/> from the assembly name as a fallback for
    ///     development builds, (3) the literal <c>"0.0.0"</c> as a last-resort sentinel so callers never
    ///     receive <see langword="null"/> or an empty string.
    /// </value>
    /// <remarks>
    ///     The three-level fallback exists because <see cref="AssemblyInformationalVersionAttribute"/> is
    ///     only populated for published artifacts; development and test builds may not carry it. Using
    ///     <c>"0.0.0"</c> as the final sentinel ensures the <c>--version</c> flag always produces
    ///     machine-parseable output regardless of build context.
    /// </remarks>
    public static string Version
    {
        get
        {
            var assembly = typeof(Program).Assembly;
            return assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                   ?? assembly.GetName().Version?.ToString()
                   ?? "0.0.0";
        }
    }

    /// <summary>
    ///     Main entry point for the application.
    /// </summary>
    /// <remarks>
    ///     Exception handling follows a two-tier strategy: anticipated operational failures
    ///     (<see cref="ArgumentException"/> for invalid arguments, <see cref="InvalidOperationException"/>
    ///     for runtime failures such as a failed file open) are caught and reported cleanly so CI runners
    ///     receive a well-formed exit code. Truly unexpected exceptions are printed to
    ///     <see cref="Console.Error"/> and re-thrown so the OS or CI event log can capture the full stack
    ///     trace — swallowing them would make root-cause analysis much harder.
    /// </remarks>
    /// <param name="args">Command line arguments.</param>
    /// <returns>
    ///     Exit code: <c>0</c> on success; <c>1</c> when an <see cref="ArgumentException"/> or
    ///     <see cref="InvalidOperationException"/> is caught; non-zero (OS-determined) when an unexpected
    ///     <see cref="Exception"/> is re-thrown.
    /// </returns>
    /// <exception cref="Exception">
    ///     Re-thrown when an unexpected exception escapes <see cref="Run"/> — only anticipated exception
    ///     types are swallowed; all others propagate so the host environment can log them.
    /// </exception>
    private static int Main(string[] args)
    {
        try
        {
            // Create context from arguments
            using var context = Context.Create(args);

            // Run the program logic
            Run(context);

            // Return the exit code from the context
            return context.ExitCode;
        }
        catch (ArgumentException ex)
        {
            // Print expected argument exceptions and return error code
            Console.Error.WriteLine($"Error: {ex.Message}");
            return 1;
        }
        catch (InvalidOperationException ex)
        {
            // Print expected operation exceptions and return error code
            Console.Error.WriteLine($"Error: {ex.Message}");
            return 1;
        }
        catch (Exception ex)
        {
            // Print unexpected exceptions and re-throw to generate event logs
            Console.Error.WriteLine($"Unexpected error: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    ///     Runs the program logic based on the provided context.
    /// </summary>
    /// <remarks>
    ///     Dispatches to exactly one of four paths in fixed priority order:
    ///     (1) <c>--version</c> — prints the version string and returns immediately, intentionally
    ///     skipping the banner so scripts can consume the output without noise;
    ///     (2) banner then <c>--help</c> — prints the application banner followed by usage text;
    ///     (3) <c>--validate</c> — runs the self-validation suite via <see cref="Validation.Run"/>;
    ///     (4) SonarQube analysis — calls <see cref="ProcessSonarAnalysis"/> with the current context.
    /// </remarks>
    /// <param name="context">The context containing command line arguments and program state.</param>
    public static void Run(Context context)
    {
        // Priority 1: Version query
        if (context.Version)
        {
            context.WriteLine(Version);
            return;
        }

        // Print application banner
        PrintBanner(context);

        // Priority 2: Help
        if (context.Help)
        {
            PrintHelp(context);
            return;
        }

        // Priority 3: Self-Validation
        if (context.Validate)
        {
            Validation.Run(context);
            return;
        }

        // Priority 4: SonarQube analysis processing
        ProcessSonarAnalysis(context);
    }

    /// <summary>
    ///     Prints the application banner.
    /// </summary>
    /// <remarks>
    ///     All output is routed through <paramref name="context"/> rather than written directly to
    ///     <see cref="Console"/>, preserving the silent-mode contract so callers running with
    ///     <c>--silent</c> suppress the banner without any special casing here.
    /// </remarks>
    /// <param name="context">The context for output.</param>
    private static void PrintBanner(Context context)
    {
        context.WriteLine($"SonarMark version {Version}");
        context.WriteLine("Copyright (c) DEMA Consulting");
        context.WriteLine("");
    }

    /// <summary>
    ///     Prints usage information.
    /// </summary>
    /// <remarks>
    ///     All output is routed through <paramref name="context"/> rather than written directly to
    ///     <see cref="Console"/>, preserving the silent-mode contract so callers running with
    ///     <c>--silent</c> suppress help output without any special casing here.
    /// </remarks>
    /// <param name="context">The context for output.</param>
    private static void PrintHelp(Context context)
    {
        context.WriteLine("Usage: sonarmark [options]");
        context.WriteLine("");
        context.WriteLine("Options:");
        context.WriteLine("  -v, --version              Display version information");
        context.WriteLine("  -?, -h, --help             Display this help message");
        context.WriteLine("  --silent                   Suppress console output");
        context.WriteLine("  --validate                 Run self-validation");
        context.WriteLine("  --results <file>           Write validation results to file (.trx or .xml)");
        context.WriteLine("  --enforce                  Return non-zero exit code if quality gate fails");
        context.WriteLine("  --log <file>               Write output to log file");
        context.WriteLine("  --server <url>             SonarQube/SonarCloud server URL");
        context.WriteLine("  --project-key <key>        SonarQube/SonarCloud project key");
        context.WriteLine("  --branch <name>            Branch name to query (default: main branch)");
        context.WriteLine("  --token <token>            Personal access token for SonarQube/SonarCloud");
        context.WriteLine("  --report <file>            Export quality results to markdown file");
        context.WriteLine("  --depth <depth>            Markdown header depth for report (default: 1)");
        context.WriteLine("  --report-depth <depth>     Alias for --depth (deprecated)");
    }

    /// <summary>
    ///     Processes SonarQube analysis results and generates reports as requested.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Missing required parameters (<c>--server</c> and <c>--project-key</c>) are detected with
    ///         early-return guards so every subsequent step can assume valid inputs without defensive
    ///         null checks throughout the method body.
    ///     </para>
    ///     <para>
    ///         <see cref="InvalidOperationException"/> is caught around the fetch call because
    ///         <see cref="SonarQubeClient"/> wraps all non-success HTTP responses and API parse failures
    ///         in that type, providing a single predictable catch point for all server-communication
    ///         failures.
    ///     </para>
    ///     <para>
    ///         The report-write path uses a generic <c>catch (Exception)</c> block rather than a
    ///         narrower type because file-system I/O can raise <see cref="System.IO.IOException"/>,
    ///         <see cref="UnauthorizedAccessException"/>, and other unrelated types; catching them all
    ///         here prevents a report-write failure from crashing the process after a successful fetch.
    ///     </para>
    ///     <para>
    ///         <c>.GetAwaiter().GetResult()</c> is used to bridge the async
    ///         <see cref="SonarQubeClient.GetQualityResultByBranchAsync"/> call into the synchronous
    ///         console-application entry point. A console app has no <c>SynchronizationContext</c>, so
    ///         this pattern is safe and avoids the overhead of making the entire call chain async.
    ///     </para>
    /// </remarks>
    /// <param name="context">The context containing command line arguments and program state.</param>
    private static void ProcessSonarAnalysis(Context context)
    {
        // Validate that required server parameter is provided
        if (string.IsNullOrWhiteSpace(context.Server))
        {
            context.WriteError("Error: --server parameter is required");
            return;
        }

        // Validate that required project key parameter is provided
        if (string.IsNullOrWhiteSpace(context.ProjectKey))
        {
            context.WriteError("Error: --project-key parameter is required");
            return;
        }

        // Display configuration information
        context.WriteLine($"Server: {context.Server}");
        context.WriteLine($"Project Key: {context.ProjectKey}");
        if (!string.IsNullOrWhiteSpace(context.Branch))
        {
            context.WriteLine($"Branch: {context.Branch}");
        }

        // Create SonarQube client using factory or default constructor
        context.WriteLine("Fetching quality results from server...");
        using var client = context.HttpClientFactory?.Invoke(context.Token) ?? new SonarQubeClient(context.Token);

        // Fetch quality results from SonarQube/SonarCloud server
        SonarQualityResult qualityResult;
        try
        {
            qualityResult = client.GetQualityResultByBranchAsync(
                context.Server,
                context.ProjectKey,
                context.Branch).GetAwaiter().GetResult();

            // Display quality gate status and issue counts
            context.WriteLine($"Quality Gate Status: {qualityResult.QualityGateStatus}");
            context.WriteLine($"Issues: {qualityResult.Issues.Count}");
            context.WriteLine($"Hot-Spots: {qualityResult.HotSpots.Count}");
        }
        catch (InvalidOperationException ex)
        {
            context.WriteError($"Error: Failed to get quality results: {ex.Message}");
            return;
        }

        // Check quality gate enforcement if requested
        if (context.Enforce && qualityResult.QualityGateStatus == "ERROR")
        {
            context.WriteError("Error: Quality gate failed");
        }

        // Generate markdown report if requested
        if (context.ReportFile != null)
        {
            context.WriteLine($"Writing quality report to {context.ReportFile}...");
            try
            {
                var markdown = qualityResult.ToMarkdown(context.Depth);
                File.WriteAllText(context.ReportFile, markdown);
                context.WriteLine("Quality report generated successfully.");
            }
            // Generic catch is justified here as a top-level handler to log any error without crashing
            catch (Exception ex)
            {
                context.WriteError($"Error: Failed to write report: {ex.Message}");
            }
        }
    }
}
