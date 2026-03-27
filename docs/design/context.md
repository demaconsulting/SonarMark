# Context

## Overview

The `Context` class parses command-line arguments, manages console output (including
silent mode and log-file redirection), and tracks whether any errors have been reported.
It acts as the bridge between the raw argument array passed to `Main` and the strongly
typed properties used by the rest of the application.

`Context` implements `IDisposable` to ensure the log-file stream writer is properly
closed when the application exits.

## Design Decisions

### Factory Method Pattern

`Context` uses a `Create` factory method rather than a public constructor. This
separates object construction (argument parsing, file opening) from the data the
object holds, and allows the internal `ArgumentParser` helper to be reused or
tested independently.

### Internal ArgumentParser

The `ArgumentParser` nested class performs the switch-based parsing loop. Keeping it
private to `Context` prevents it from leaking into the public API while still
allowing the factory method to call it. All validation logic (e.g., missing argument
values, non-integer depth) is handled here and surfaced as `ArgumentException`.

### Output Routing

`WriteLine` and `WriteError` honour the `Silent` flag so callers never need
to check it themselves. Both methods also mirror output to the log file when one
has been opened. `WriteError` additionally sets the `_hasErrors` flag, which drives
the `ExitCode` property.

### IDisposable for Log File

The log-file stream writer is held as a nullable field and released in `Dispose`.
Using `IDisposable` rather than a finalizer is appropriate because the resource
(an open file handle) must be released deterministically at application exit.

## Satisfies Requirements

- `SonarMark-Cmd-Cli` — `Context.Create` parses the full set of supported flags
- `SonarMark-Cmd-Silent` — `Silent` property suppresses console output
- `SonarMark-Cmd-Log` — `OpenLogFile` writes all output to a log file
- `SonarMark-Cmd-Enforce` — `Enforce` property enables quality gate enforcement
- `SonarMark-Val-Results` — `ResultsFile` property specifies where results are written
- `SonarMark-Val-TrxFormat` — TRX file extension is accepted by `ResultsFile`
- `SonarMark-Val-JUnitFormat` — JUnit XML file extension is accepted by `ResultsFile`
- `SonarMark-Enf-Mode` — `Enforce` flag is parsed and stored
- `SonarMark-Enf-ExitCode` — `ExitCode` returns 1 when `_hasErrors` is true
