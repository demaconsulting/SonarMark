## Cli

### Overview

The Cli subsystem handles command-line argument parsing and program output routing for the
SonarMark tool. It provides a single unit, `Context`, which carries all parsed arguments,
manages console output (including silent mode and log-file redirection), tracks whether any
errors have been reported, and exposes the resulting exit code. Argument parsing is delegated
to the nested private `ArgumentParser` class, which performs the switch-based parsing loop
and keeps `Context.Create` as a thin factory method.

### Interfaces

**Context**: The primary data-carrying object used throughout the system.

- *Contract*: `Context.Create(args)` factory method parses command-line arguments and returns
  a fully populated `Context` instance. `WriteLine` writes to stdout (and log file if open);
  `WriteError` writes to stderr in red, mirrors to log, and sets the error flag. `ExitCode`
  returns 0 or 1. Implements `IDisposable` to release the log-file writer.
- *Constraints*: `Context.Create` throws `ArgumentException` on invalid arguments.
  `OpenLogFile` throws `InvalidOperationException` if the file cannot be opened. `Context`
  must be disposed to release the log-file handle.

### Design

1. `Context.Create` delegates to the internal `ArgumentParser.ParseArguments` to perform the
   switch-based parsing loop.
2. If a `--log` path was parsed, `Context.Create` opens the log-file stream writer before
   returning.
3. The resulting `Context` is passed to `Program.Run`, which reads the boolean flag properties
   (`Version`, `Help`, `Validate`, `Enforce`) to determine the dispatch path.
4. All subsystems that produce output call `context.WriteLine` or `context.WriteError`; neither
   caller needs to check the `Silent` flag directly.
5. When `Program.Main` returns `context.ExitCode`, the `using` block disposes the context and
   closes the log-file writer.
