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

namespace DemaConsulting.SonarMark;

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
    public int ReportDepth { get; private init; } = 1;

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
    /// <exception cref="ArgumentException">Thrown when arguments are invalid.</exception>
    public static Context Create(string[] args)
    {
        // Initialize flag variables
        var version = false;
        var help = false;
        var silent = false;
        var validate = false;
        var enforce = false;

        // Initialize optional parameters
        string? reportFile = null;
        var reportDepth = 1;
        string? token = null;
        string? server = null;
        string? projectKey = null;
        string? branch = null;
        string? logFile = null;

        // Parse command-line arguments
        int i = 0;
        while (i < args.Length)
        {
            // Get current argument and advance index
            var arg = args[i++];

            switch (arg)
            {
                case "-v":
                case "--version":
                    version = true;
                    break;

                case "-?":
                case "-h":
                case "--help":
                    help = true;
                    break;

                case "--silent":
                    silent = true;
                    break;

                case "--validate":
                    validate = true;
                    break;

                case "--enforce":
                    enforce = true;
                    break;

                case "--log":
                    // Ensure argument has a value
                    if (i >= args.Length)
                    {
                        throw new ArgumentException($"{arg} requires a filename argument", nameof(args));
                    }

                    logFile = args[i++];
                    break;

                case "--report":
                    // Ensure argument has a value
                    if (i >= args.Length)
                    {
                        throw new ArgumentException($"{arg} requires a filename argument", nameof(args));
                    }

                    reportFile = args[i++];
                    break;

                case "--report-depth":
                    // Ensure argument has a value
                    if (i >= args.Length)
                    {
                        throw new ArgumentException($"{arg} requires a depth argument", nameof(args));
                    }

                    // Parse and validate depth value
                    if (!int.TryParse(args[i++], out reportDepth) || reportDepth < 1)
                    {
                        throw new ArgumentException($"{arg} requires a positive integer", nameof(args));
                    }

                    break;

                case "--token":
                    // Ensure argument has a value
                    if (i >= args.Length)
                    {
                        throw new ArgumentException($"{arg} requires a token argument", nameof(args));
                    }

                    token = args[i++];
                    break;

                case "--server":
                    // Ensure argument has a value
                    if (i >= args.Length)
                    {
                        throw new ArgumentException($"{arg} requires a server URL argument", nameof(args));
                    }

                    server = args[i++];
                    break;

                case "--project-key":
                    // Ensure argument has a value
                    if (i >= args.Length)
                    {
                        throw new ArgumentException($"{arg} requires a project key argument", nameof(args));
                    }

                    projectKey = args[i++];
                    break;

                case "--branch":
                    // Ensure argument has a value
                    if (i >= args.Length)
                    {
                        throw new ArgumentException($"{arg} requires a branch name argument", nameof(args));
                    }

                    branch = args[i++];
                    break;

                default:
                    throw new ArgumentException($"Unsupported argument '{arg}'", nameof(args));
            }
        }

        // Create the context with parsed values
        var result = new Context
        {
            Version = version,
            Help = help,
            Silent = silent,
            Validate = validate,
            Enforce = enforce,
            ReportFile = reportFile,
            ReportDepth = reportDepth,
            Token = token,
            Server = server,
            ProjectKey = projectKey,
            Branch = branch
        };

        // Open log file if specified
        if (logFile != null)
        {
            try
            {
                result._logWriter = new StreamWriter(logFile, append: false);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Failed to open log file '{logFile}': {ex.Message}", nameof(args), ex);
            }
        }

        return result;
    }

    /// <summary>
    ///     Writes a line of output to the console and log file (if logging is enabled).
    /// </summary>
    /// <param name="message">The message to write.</param>
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
    public void WriteError(string message)
    {
        // Mark that we have encountered errors
        _hasErrors = true;

        // Write to error console unless silent mode is enabled
        if (!Silent)
        {
            var previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
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
