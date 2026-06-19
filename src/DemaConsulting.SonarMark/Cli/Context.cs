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

using DemaConsulting.SonarMark.SonarIntegration;

namespace DemaConsulting.SonarMark.Cli;

/// <summary>
///     Context class that handles command-line arguments and program output.
/// </summary>
internal sealed class Context : IDisposable
{
    /// <summary>
    ///     Log file stream writer (if logging is enabled).
    /// </summary>
    private StreamWriter? _logWriter;

    /// <summary>
    ///     Indicates whether errors have been reported.
    /// </summary>
    private bool _hasErrors;

    /// <summary>
    ///     Gets a value indicating whether the version flag was specified.
    /// </summary>
    public bool Version { get; private init; }

    /// <summary>
    ///     Gets a value indicating whether the help flag was specified.
    /// </summary>
    public bool Help { get; private init; }

    /// <summary>
    ///     Gets a value indicating whether the silent flag was specified.
    /// </summary>
    public bool Silent { get; private init; }

    /// <summary>
    ///     Gets a value indicating whether the validate flag was specified.
    /// </summary>
    public bool Validate { get; private init; }

    /// <summary>
    ///     Gets a value indicating whether the enforce flag was specified.
    /// </summary>
    public bool Enforce { get; private init; }

    /// <summary>
    ///     Gets the report file path.
    /// </summary>
    public string? ReportFile { get; private init; }

    /// <summary>
    ///     Gets the report markdown depth.
    /// </summary>
    public int Depth { get; private init; } = 1;

    /// <summary>
    ///     Gets the personal access token for SonarQube/SonarCloud authentication.
    /// </summary>
    public string? Token { get; private init; }

    /// <summary>
    ///     Gets the SonarQube/SonarCloud server URL.
    /// </summary>
    public string? Server { get; private init; }

    /// <summary>
    ///     Gets the SonarQube/SonarCloud project key.
    /// </summary>
    public string? ProjectKey { get; private init; }

    /// <summary>
    ///     Gets the branch name to query.
    /// </summary>
    public string? Branch { get; private init; }

    /// <summary>
    ///     Gets the validation results file path.
    /// </summary>
    public string? ResultsFile { get; private init; }

    /// <summary>
    ///     Gets the HTTP client factory for creating SonarQube clients (for testing).
    /// </summary>
    internal Func<string?, SonarQubeClient>? HttpClientFactory { get; private init; }

    /// <summary>
    ///     Gets the proposed exit code for the application (0 for success, 1 for errors).
    /// </summary>
    public int ExitCode => _hasErrors ? 1 : 0;

    /// <summary>
    ///     Private constructor - use Create factory method instead.
    /// </summary>
    private Context()
    {
    }

    /// <summary>
    ///     Creates a Context instance from command-line arguments.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    /// <returns>A new Context instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="args"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when arguments are invalid.</exception>
    /// <remarks>
    ///     This factory method is the primary public entry point for creating a <see cref="Context" />.
    ///     It delegates to <see cref="Create(string[], Func{string?, SonarQubeClient}?)" /> with a
    ///     <see langword="null" /> HTTP client factory, which causes the production
    ///     <see cref="SonarQubeClient" /> to be used. Tests that need to inject a mock client should
    ///     use the two-argument overload instead.
    /// </remarks>
    public static Context Create(string[] args)
    {
        // Validate that args is not null
        ArgumentNullException.ThrowIfNull(args);

        return Create(args, null);
    }

    /// <summary>
    ///     Creates a Context instance from command-line arguments with optional HTTP client factory.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    /// <param name="httpClientFactory">Optional HTTP client factory for testing.</param>
    /// <returns>A new Context instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="args"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when arguments are invalid.</exception>
    /// <remarks>
    ///     This overload supports test injection of a mock <see cref="SonarQubeClient" /> without
    ///     modifying production code paths. The <paramref name="httpClientFactory" /> delegate is
    ///     stored on the returned <see cref="Context" /> and called by
    ///     <c>Program.ProcessSonarAnalysis</c> when creating the HTTP client; passing
    ///     <see langword="null" /> restores the production behavior of constructing a real client.
    ///     <para>
    ///         Token resolution order: if <c>--token</c> is not supplied on the command line,
    ///         the <c>SONAR_TOKEN</c> environment variable is read as a fallback. The explicit
    ///         flag always takes priority over the environment variable.
    ///     </para>
    /// </remarks>
    public static Context Create(string[] args, Func<string?, SonarQubeClient>? httpClientFactory)
    {
        // Validate that args is not null
        ArgumentNullException.ThrowIfNull(args);

        // Parse command-line arguments into structured form
        var parser = new ArgumentParser();
        parser.ParseArguments(args);

        // Create context with parsed arguments, falling back to SONAR_TOKEN environment variable if --token not supplied
        var result = new Context
        {
            Version = parser.Version,
            Help = parser.Help,
            Silent = parser.Silent,
            Validate = parser.Validate,
            Enforce = parser.Enforce,
            ReportFile = parser.ReportFile,
            Depth = parser.Depth,
            Token = parser.Token ?? Environment.GetEnvironmentVariable("SONAR_TOKEN"),
            Server = parser.Server,
            ProjectKey = parser.ProjectKey,
            Branch = parser.Branch,
            ResultsFile = parser.ResultsFile,
            HttpClientFactory = httpClientFactory
        };

        // Open log file if specified
        if (parser.LogFile != null)
        {
            result.OpenLogFile(parser.LogFile);
        }

        return result;
    }

    /// <summary>
    ///     Opens the log file for writing
    /// </summary>
    /// <param name="logFile">Log file path</param>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the log file cannot be opened due to file-system errors such as
    ///     <see cref="IOException" />, <see cref="UnauthorizedAccessException" />, or
    ///     invalid path characters.
    /// </exception>
    private void OpenLogFile(string logFile)
    {
        try
        {
            _logWriter = new StreamWriter(logFile, append: false) { AutoFlush = true };
        }
        // Generic catch is justified here to wrap any file system exception with context.
        // Expected exceptions include IOException, UnauthorizedAccessException, ArgumentException,
        // NotSupportedException, and other file system-related exceptions.
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to open log file '{logFile}': {ex.Message}", ex);
        }
    }

    /// <summary>
    ///     Helper class for parsing command-line arguments
    /// </summary>
    private sealed class ArgumentParser
    {
        /// <summary>
        ///     Gets a value indicating whether the version flag was specified.
        /// </summary>
        public bool Version { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether the help flag was specified.
        /// </summary>
        public bool Help { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether the silent flag was specified.
        /// </summary>
        public bool Silent { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether the validate flag was specified.
        /// </summary>
        public bool Validate { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether the enforce flag was specified.
        /// </summary>
        public bool Enforce { get; private set; }

        /// <summary>
        ///     Gets the report file path.
        /// </summary>
        public string? ReportFile { get; private set; }

        /// <summary>
        ///     Gets the report markdown depth.
        /// </summary>
        public int Depth { get; private set; } = 1;

        /// <summary>
        ///     Gets the personal access token for SonarQube/SonarCloud authentication.
        /// </summary>
        public string? Token { get; private set; }

        /// <summary>
        ///     Gets the SonarQube/SonarCloud server URL.
        /// </summary>
        public string? Server { get; private set; }

        /// <summary>
        ///     Gets the SonarQube/SonarCloud project key.
        /// </summary>
        public string? ProjectKey { get; private set; }

        /// <summary>
        ///     Gets the branch name to query.
        /// </summary>
        public string? Branch { get; private set; }

        /// <summary>
        ///     Gets the log file path.
        /// </summary>
        public string? LogFile { get; private set; }

        /// <summary>
        ///     Gets the validation results file path.
        /// </summary>
        public string? ResultsFile { get; private set; }

        /// <summary>
        ///     Parses command-line arguments
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        /// <exception cref="ArgumentException">
        ///     Thrown for unrecognized flags, missing required argument values, or
        ///     out-of-range depth values (not 1–6).
        /// </exception>
        public void ParseArguments(string[] args)
        {
            // Iterate through all arguments, processing each one
            int i = 0;
            while (i < args.Length)
            {
                var arg = args[i++];
                i = ParseArgument(arg, args, i);
            }
        }

        /// <summary>
        ///     Parses a single argument
        /// </summary>
        /// <param name="arg">Argument to parse</param>
        /// <param name="args">All arguments</param>
        /// <param name="index">Current index</param>
        /// <returns>Updated index</returns>
        /// <exception cref="ArgumentException">
        ///     Thrown for unrecognized flags, missing required argument values, or
        ///     out-of-range depth values (not 1–6).
        /// </exception>
        private int ParseArgument(string arg, string[] args, int index)
        {
            switch (arg)
            {
                case "-v":
                case "--version":
                    Version = true;
                    return index;

                case "-?":
                case "-h":
                case "--help":
                    Help = true;
                    return index;

                case "--silent":
                    Silent = true;
                    return index;

                case "--validate":
                    Validate = true;
                    return index;

                case "--enforce":
                    Enforce = true;
                    return index;

                case "--log":
                    LogFile = GetRequiredStringArgument(arg, args, index, "a filename argument");
                    return index + 1;

                case "--report":
                    ReportFile = GetRequiredStringArgument(arg, args, index, "a filename argument");
                    return index + 1;

                case "--depth":
                case "--report-depth":
                    Depth = GetRequiredIntArgument(arg, args, index);
                    return index + 1;

                case "--token":
                    Token = GetRequiredStringArgument(arg, args, index, "a token argument");
                    return index + 1;

                case "--server":
                    Server = GetRequiredStringArgument(arg, args, index, "a server URL argument");
                    return index + 1;

                case "--project-key":
                    ProjectKey = GetRequiredStringArgument(arg, args, index, "a project key argument");
                    return index + 1;

                case "--branch":
                    Branch = GetRequiredStringArgument(arg, args, index, "a branch name argument");
                    return index + 1;

                // Accept --result as a legacy alias so older scripts continue working
                case "--result":
                case "--results":
                    ResultsFile = GetRequiredStringArgument(arg, args, index, "a results filename argument");
                    return index + 1;

                default:
                    throw new ArgumentException($"Unsupported argument '{arg}'", nameof(args));
            }
        }

        /// <summary>
        ///     Gets a required string argument value
        /// </summary>
        /// <param name="arg">Argument name</param>
        /// <param name="args">All arguments</param>
        /// <param name="index">Current index</param>
        /// <param name="description">Description of what's required</param>
        /// <returns>Argument value</returns>
        /// <exception cref="ArgumentException">
        ///     Thrown when no value follows the argument (i.e., <paramref name="index" /> is
        ///     beyond the end of <paramref name="args" />).
        /// </exception>
        private static string GetRequiredStringArgument(string arg, string[] args, int index, string description)
        {
            if (index >= args.Length)
            {
                throw new ArgumentException($"{arg} requires {description}", nameof(args));
            }

            return args[index];
        }

        /// <summary>
        ///     Gets a required positive integer argument value
        /// </summary>
        /// <param name="arg">Argument name</param>
        /// <param name="args">All arguments</param>
        /// <param name="index">Current index</param>
        /// <returns>Argument value</returns>
        /// <exception cref="ArgumentException">
        ///     Thrown when no value follows the argument or when the value is not a valid
        ///     integer in the range 1–6.
        /// </exception>
        private static int GetRequiredIntArgument(string arg, string[] args, int index)
        {
            if (index >= args.Length)
            {
                throw new ArgumentException($"{arg} requires a depth argument", nameof(args));
            }

            if (!int.TryParse(args[index], out var value) || value < 1 || value > 6)
            {
                throw new ArgumentException($"{arg} requires a depth between 1 and 6", nameof(args));
            }

            return value;
        }
    }

    /// <summary>
    ///     Writes a line of output to the console and log file (if logging is enabled).
    /// </summary>
    /// <param name="message">The message to write.</param>
    /// <remarks>
    ///     When <see cref="Silent" /> is <see langword="true" /> the message is suppressed from
    ///     the console but still written to the log file if one is open.
    /// </remarks>
    public void WriteLine(string message)
    {
        // Write to console unless silent mode is enabled
        if (!Silent)
        {
            Console.WriteLine(message);
        }

        // Write to log file if logging is enabled
        _logWriter?.WriteLine(message);
    }

    /// <summary>
    ///     Writes an error message to the error console and log file (if logging is enabled).
    /// </summary>
    /// <param name="message">The error message to write.</param>
    /// <remarks>
    ///     Sets the internal error flag so that <see cref="ExitCode" /> returns 1. When
    ///     <see cref="Silent" /> is <see langword="true" /> the message is suppressed from the
    ///     console but still written to the log file if one is open. When not in silent mode,
    ///     the error output is rendered in red (<see cref="ConsoleColor.Red" />) on the console
    ///     to make it visually distinct from normal output.
    /// </remarks>
    public void WriteError(string message)
    {
        // Mark that we have encountered errors
        _hasErrors = true;

        // Write to error console unless silent mode is enabled
        if (!Silent)
        {
            var previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(message);
            Console.ForegroundColor = previousColor;
        }

        // Write to log file if logging is enabled
        _logWriter?.WriteLine(message);
    }

    /// <summary>
    ///     Disposes resources used by the Context.
    /// </summary>
    public void Dispose()
    {
        // Close and dispose the log file writer if it exists
        _logWriter?.Dispose();
        _logWriter = null;
    }
}
