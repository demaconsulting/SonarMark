## Program

### Purpose

`Program` is the static entry point for the SonarMark application. It contains `Main`, which
constructs a `Context` from command-line arguments and calls `Run`. `Run` dispatches to the
appropriate subsystem based on the flags present in the context.

### Data Model

N/A - `Program` is a static class with no instance fields or properties.

### Key Methods

**Version**: Public static property exposing the tool's version string.

- *Returns*: `string` — the version string, resolved via a three-step fallback chain: (1)
  `AssemblyInformationalVersionAttribute` (set by the SDK from the project version on publish),
  (2) `Assembly.GetName().Version.ToString()` as a fallback for development builds, (3) a
  zero-version sentinel string as a last-resort so callers never receive null or an empty string.

**Main**: Application entry point.

- *Parameters*: `string[] args` — raw command-line arguments from the host environment.
- *Returns*: `int` — exit code; 0 on success, 1 on handled error, non-zero re-throw on
  unexpected exception.
- *Preconditions*: None.
- *Postconditions*: All resources opened by `Context` are disposed before return.

Creates a `Context` via `Context.Create(args)`, calls `Run`, and returns `context.ExitCode`.
Catches `ArgumentException` (invalid arguments) and `InvalidOperationException` (runtime
failures such as a failed file open) and prints their messages to stderr without a stack
trace. Truly unexpected exceptions are printed to stderr and re-thrown so the OS or CI runner
can produce an event log entry.

**PrintBanner**: Private method that writes the application banner (version string and copyright
notice) to the output stream.

- *Parameters*: `Context context` — routes all output; no direct writes to `Console`.
- *Returns*: `void`.

All output is routed through the `context` parameter rather than written directly to `Console`,
preserving the silent-mode contract so callers running with `--silent` suppress banner output.

**PrintHelp**: Private method that writes the full usage and options reference to the output
stream.

- *Parameters*: `Context context` — routes all output; no direct writes to `Console`.
- *Returns*: `void`.

All output is routed through the `context` parameter rather than written directly to `Console`,
preserving the silent-mode contract so callers running with `--silent` suppress help output.

**Run**: Dispatches to the appropriate subsystem based on context flags.

- *Parameters*: `Context context` — the parsed context.
- *Returns*: `void`.
- *Preconditions*: `context` is not null.
- *Postconditions*: Exactly one subsystem path (version, help, validation, or analysis) has
  been executed.

Applies a fixed priority order: (1) version — prints the version string and returns
immediately without a banner, enabling scripts to consume it; (2) banner then help — prints
the application banner followed by help text; (3) self-validation — calls `Validation.Run`;
(4) SonarQube analysis — calls `ProcessSonarAnalysis`.

**ProcessSonarAnalysis**: Validates required parameters, fetches analysis results, and writes
the report.

- *Parameters*: `Context context` — the parsed context.
- *Returns*: `void`.
- *Preconditions*: `context.Server` and `context.ProjectKey` may be null; the method validates
  them and writes errors via `context.WriteError` if missing.
- *Postconditions*: If validation passes, a `SonarQualityResult` has been fetched; if
  `--report` was specified, the markdown file has been written or an error has been recorded.

When `context.HttpClientFactory` is non-null, invokes it to obtain a `SonarQubeClient` (the
test injection path). When null, falls back to `new SonarQubeClient(context.Token)`. On
`--enforce`, writes an error if the quality gate status is `ERROR`.

### Error Handling

`Main` catches `ArgumentException` and `InvalidOperationException` and returns exit code 1.
Unexpected exceptions are printed to stderr and re-thrown. Inside `ProcessSonarAnalysis`,
missing required parameters are reported via `context.WriteError` and the method returns
early. `SonarQubeClient` fetch failures (`InvalidOperationException`) are caught and reported
via `context.WriteError`. Report-write failures are caught with a generic `catch (Exception)`
block and reported via `context.WriteError` — justified as a top-level I/O error handler to
prevent crashing the process when the report cannot be written.

### Dependencies

- **Context** — provides parsed arguments and all output routing.
- **SonarQubeClient** — created by `ProcessSonarAnalysis` to fetch quality results.
- **SonarQualityResult** — produced by `SonarQubeClient`; rendered to markdown by `ToMarkdown`.
- **Validation** — invoked by `Run` when `context.Validate` is true.

### Callers

N/A - entry point, called by the host environment.
