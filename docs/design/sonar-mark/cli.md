## Cli

![Cli Structure](CliView.svg)

### Overview

The Cli subsystem handles command-line argument parsing and program output routing for the
SonarMark tool, and answers the built-in `help` command. It contains two units:

- **Context**: carries all parsed arguments, manages console output (including silent mode and
  log-file redirection), tracks whether any errors have been reported, and exposes the
  resulting exit code. Argument parsing is delegated to the nested private `ArgumentParser`
  class, which performs the switch-based parsing loop and keeps `Context.Create` as a thin
  factory method.
- **HelpCommand**: the built-in `help` command, answered from embedded help cards.

### Interfaces

**Context**: The primary data-carrying object used throughout the system.

- *Contract*: `Context.Create(args)` factory method parses command-line arguments and returns
  a fully populated `Context` instance. `WriteLine` writes to stdout (and log file if open);
  `WriteError` writes to stderr in red, mirrors to log, and sets the error flag. `ExitCode`
  returns 0 or 1. Implements `IDisposable` to release the log-file writer.
- *Constraints*: `Context.Create` throws `ArgumentNullException` when `args` is null and
  throws `ArgumentException` on invalid arguments.
  `OpenLogFile` throws `InvalidOperationException` if the file cannot be opened. `Context`
  must be disposed to release the log-file handle.

**HelpCommand.TryRun**: Answers the `help` command from the built-in help cards.

- *Contract*: Called on the shared `HelpCommand.Default` instance. Returns `false`, writing
  nothing, when the first argument is not `help`. Otherwise writes the answer to the console,
  returns `true`, and sets the exit code (see *Help Command* in the system design).
- *Constraints*: Does not throw for user input; does not use `Context`.

**HelpCommand.Footer**: The sentence that points a reader of the usage text to the help command.

- *Contract*: Returns the one-line pointer that HelpCards supplies.
- *Constraints*: Reading it never builds the help catalog.

### Design

`Context` and `HelpCommand` do not call each other. `Program.Main` offers the arguments to
`HelpCommand.TryRun` first; when the help command answers, the invocation ends with its exit
code and no `Context` is created. `HelpCommand` writes its answers to the console itself. It is
the one exception to `Context` owning all output, because the `help` command is answered before
a `Context` exists.

For every other invocation:

1. `Context.Create` delegates to the internal `ArgumentParser.ParseArguments` to perform the
   switch-based parsing loop.
2. If a `--log` path was parsed, `Context.Create` opens the log-file stream writer before
   returning.
3. The resulting `Context` is passed to `Program.Run`, which reads the boolean flag properties
   (`Version`, `Help`, `Validate`) to determine the dispatch path. `Enforce` is not used to
   select a dispatch path; instead, it is evaluated by `Program.ProcessSonarAnalysis` after
   the analysis fetch completes, and causes a non-zero exit code when the quality gate status
   is `ERROR`.
4. All subsystems that produce output call `context.WriteLine` or `context.WriteError`; neither
   caller needs to check the `Silent` flag directly.
5. When `Program.Main` returns `context.ExitCode`, the `using` block disposes the context and
   closes the log-file writer.
