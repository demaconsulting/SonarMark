### Context

#### Purpose

`Context` parses command-line arguments, manages console output (including silent mode and
log-file redirection), and tracks whether any errors have been reported. It is the bridge
between the raw argument array passed to `Main` and the strongly typed properties used by
the rest of the application. `Context` implements `IDisposable` to ensure the log-file
stream writer is closed when the application exits.

#### Data Model

**Version**: `bool` — `true` when `--version` or `-v` was supplied; causes `Run` to print
the version string and return immediately.

**Help**: `bool` — `true` when `--help`, `-h`, or `-?` was supplied; causes `Run` to print
help text and return.

**Silent**: `bool` — `true` when `--silent` was supplied; suppresses all console output from
`WriteLine` and `WriteError`.

**Validate**: `bool` — `true` when `--validate` was supplied; causes `Run` to invoke
`Validation.Run`.

**Enforce**: `bool` — `true` when `--enforce` was supplied; causes `ProcessSonarAnalysis` to
report an error when the quality gate status is `ERROR`.

**ReportFile**: `string?` — path supplied with `--report`; `null` when not provided.

**Depth**: `int` — markdown heading depth from `--depth` (or deprecated `--report-depth`);
defaults to 1; valid range is 1–6.

**Token**: `string?` — PAT resolved in priority order: (1) value supplied with `--token`; (2)
value of the `SONAR_TOKEN` environment variable when `--token` is absent; `null` when neither
source provides a value. Passed to `SonarQubeClient` for HTTP Basic authentication.

**Server**: `string?` — SonarQube/SonarCloud server URL from `--server`; `null` when not
provided.

**ProjectKey**: `string?` — SonarQube/SonarCloud project key from `--project-key`; `null`
when not provided.

**Branch**: `string?` — branch name from `--branch`; `null` when not provided, in which case
the server uses its default branch.

**ResultsFile**: `string?` — path supplied with `--results` (or legacy `--result`); `null`
when not provided; used by `Validation.WriteResultsFile`.

**HttpClientFactory**: `Func<string?, SonarQubeClient>?` — optional factory injected via
`Context.Create(args, factory)`; used by integration tests to supply a mock client; `null`
on the production path.

**ExitCode**: `int` — read-only property; returns 1 when `_hasErrors` is `true`, 0 otherwise.

**_hasErrors**: `bool` — private field; set by `WriteError`; drives `ExitCode`.

**_logWriter**: `StreamWriter?` — private field; non-null when a log file is open; released
in `Dispose`.

#### Key Methods

**Create(string[] args)**: Factory method — parses arguments and returns a new `Context`.

- *Parameters*: `string[] args` — raw command-line arguments.
- *Returns*: `Context` — fully initialized instance.
- *Preconditions*: `args` must not be null; throws `ArgumentNullException` otherwise.
- *Postconditions*: All properties are populated from parsed arguments; log file is open if
  `--log` was supplied.

Delegates to `ArgumentParser.ParseArguments`, constructs a `Context` via object initializer,
and calls `OpenLogFile` when a log path was parsed.

**Create(string[] args, Func\<string?, SonarQubeClient\>? httpClientFactory)**: Overload that
additionally accepts an HTTP client factory for test injection.

- *Parameters*: `string[] args` — raw command-line arguments; `Func<string?, SonarQubeClient>?
  httpClientFactory` — optional mock factory.
- *Returns*: `Context` — fully initialized instance with the factory stored in
  `HttpClientFactory`.
- *Preconditions*: `args` must not be null; throws `ArgumentNullException` otherwise.
- *Postconditions*: Same as the single-argument overload; `HttpClientFactory` is set.

**WriteLine(string message)**: Writes a line to stdout (unless silent) and to the log file
(if open).

- *Parameters*: `string message` — text to write.
- *Returns*: `void`.
- *Preconditions*: None.
- *Postconditions*: Message is written to at least one destination unless both `Silent` is
  true and no log file is open.

**WriteError(string message)**: Writes a line to stderr in red (unless silent), mirrors to
the log file, and sets `_hasErrors`.

- *Parameters*: `string message` — error text to write.
- *Returns*: `void`.
- *Preconditions*: None.
- *Postconditions*: `_hasErrors` is `true`; `ExitCode` will return 1.

**Dispose**: Releases the log-file stream writer.

- *Parameters*: None.
- *Returns*: `void`.
- *Preconditions*: None.
- *Postconditions*: `_logWriter` is `null`; the log-file handle is closed.

**OpenLogFile(string logFile)**: Opens the specified path for writing and stores the resulting
`StreamWriter` in `_logWriter`.

- *Parameters*: `string logFile` — file-system path to the log file.
- *Returns*: `void`.
- *Preconditions*: `logFile` must be a valid, writable path.
- *Postconditions*: `_logWriter` is non-null; subsequent `WriteLine`/`WriteError` calls mirror
  output to the file.
- *Exceptions*: Wraps any file-system exception (e.g., `IOException`,
  `UnauthorizedAccessException`) in `InvalidOperationException` with a descriptive message.

**ArgumentParser** (nested private class): Encapsulates all switch-based argument parsing so
that `Context.Create` remains a thin factory method.

- **ParseArguments(string[] args)**: Iterates over `args` and calls `ParseArgument` for each
  token; throws `ArgumentException` for unrecognized flags or missing values.
- **ParseArgument(string arg, string[] args, int index)**: Dispatches on `arg` via `switch`;
  calls `GetRequiredStringArgument` or `GetRequiredIntArgument` for flags that take a value;
  returns the updated index.
- **GetRequiredStringArgument(...)**: Validates that a following token exists and returns it;
  throws `ArgumentException` otherwise.
- **GetRequiredIntArgument(...)**: Validates that a following token exists, parses it as an
  integer in [1, 6], and returns the value; throws `ArgumentException` otherwise.

#### Error Handling

`ArgumentParser.ParseArguments` throws `ArgumentException` for unrecognized flags, missing
required argument values, and out-of-range depth values (not 1–6). These propagate to
`Program.Main`, which catches and reports them. `OpenLogFile` wraps any file-system exception
in an `InvalidOperationException` with context, which also propagates to `Main`.

#### Dependencies

- **SonarQubeClient** — referenced only through the `HttpClientFactory` delegate type stored
  on `Context`; no direct instantiation within this unit.

#### Callers

- **Program** — creates `Context` via `Context.Create` and passes it to all subsystems.
