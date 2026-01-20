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

namespace DemaConsulting.SonarMark;

/// <summary>
///     Main program entry point for SonarMark application
/// </summary>
internal static class Program
{
    /// <summary>
    ///     Gets the application version string.
    /// </summary>
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
    /// <param name="args">Command line arguments.</param>
    /// <returns>Exit code.</returns>
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
            Console.WriteLine($"Error: {ex.Message}");
            return 1;
        }
        catch (InvalidOperationException ex)
        {
            // Print expected operation exceptions and return error code
            Console.WriteLine($"Error: {ex.Message}");
            return 1;
        }
        catch (Exception ex)
        {
            // Print unexpected exceptions and re-throw to generate event logs
            Console.WriteLine($"Unexpected error: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    ///     Runs the program logic based on the provided context.
    /// </summary>
    /// <param name="context">The context containing command line arguments and program state.</param>
    public static void Run(Context context)
    {
        // Priority 1: Version query
        if (context.Version)
        {
            Console.WriteLine(Version);
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
            context.WriteLine("Self-validation not yet implemented");
            return;
        }

        // Priority 4: SonarQube analysis processing
        ProcessSonarAnalysis(context);
    }

    /// <summary>
    ///     Prints the application banner.
    /// </summary>
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
        context.WriteLine("  --enforce                  Return non-zero exit code if quality gate fails");
        context.WriteLine("  --log <file>               Write output to log file");
        context.WriteLine("  --working-directory <dir>  Directory to search for report-task.txt file");
        context.WriteLine("  --token <token>            Personal access token for SonarQube/SonarCloud");
        context.WriteLine("  --report <file>            Export quality results to markdown file");
        context.WriteLine("  --report-depth <depth>     Markdown header depth for report (default: 1)");
    }

    /// <summary>
    ///     Processes SonarQube analysis results and generates reports as requested.
    /// </summary>
    /// <param name="context">The context containing command line arguments and program state.</param>
    private static void ProcessSonarAnalysis(Context context)
    {
        // Determine the working directory
        var workingDirectory = context.WorkingDirectory ?? Directory.GetCurrentDirectory();

        // Find the report-task.txt file
        context.WriteLine($"Searching for report-task.txt in {workingDirectory}...");
        var reportTaskFile = ReportTaskParser.FindReportTask(workingDirectory);

        if (reportTaskFile == null)
        {
            context.WriteError($"Error: Could not find report-task.txt in {workingDirectory}");
            return;
        }

        context.WriteLine($"Found report-task.txt at {reportTaskFile}");

        // Parse the report task file
        ReportTask reportTask;
        try
        {
            reportTask = ReportTaskParser.Parse(reportTaskFile);
            context.WriteLine($"Project: {reportTask.ProjectKey}");
            context.WriteLine($"Server: {reportTask.ServerUrl}");
            context.WriteLine($"Task ID: {reportTask.CeTaskId}");
        }
        catch (ArgumentException ex)
        {
            context.WriteError($"Error: Failed to parse report-task.txt: {ex.Message}");
            return;
        }

        // Get quality results from SonarQube/SonarCloud
        context.WriteLine("Fetching quality results from server...");
        using var client = new SonarQubeClient(context.Token);

        SonarQualityResult qualityResult;
        try
        {
            qualityResult = client.GetQualityResultAsync(reportTask).GetAwaiter().GetResult();
            context.WriteLine($"Quality Gate Status: {qualityResult.QualityGateStatus}");
        }
        catch (InvalidOperationException ex)
        {
            context.WriteError($"Error: Failed to get quality results: {ex.Message}");
            return;
        }

        // Check enforcement if requested
        if (context.Enforce && qualityResult.QualityGateStatus == "ERROR")
        {
            context.WriteError("Error: Quality gate failed");
        }

        // Export quality report if requested
        if (context.ReportFile != null)
        {
            context.WriteLine($"Writing quality report to {context.ReportFile}...");
            try
            {
                var markdown = qualityResult.ToMarkdown(context.ReportDepth);
                File.WriteAllText(context.ReportFile, markdown);
                context.WriteLine("Quality report generated successfully.");
            }
            catch (Exception ex)
            {
                context.WriteError($"Error: Failed to write report: {ex.Message}");
            }
        }
    }
}
