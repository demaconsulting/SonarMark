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

- `SonarMark-Cli-Interface` — `Context.Create` parses the full set of supported flags
- `SonarMark-Cli-Silent` — `Silent` property suppresses console output
- `SonarMark-Cli-Log` — `OpenLogFile` writes all output to a log file
- `SonarMark-Cli-Enforce` — `Enforce` property enables quality gate enforcement
- `SonarMark-Validation-Results` — `ResultsFile` property specifies where results are written
- `SonarMark-Validation-TrxFormat` — TRX file extension is accepted by `ResultsFile`
- `SonarMark-Validation-JUnitFormat` — JUnit XML file extension is accepted by `ResultsFile`
- `SonarMark-Enforce-Mode` — `Enforce` flag is parsed and stored
- `SonarMark-Enforce-ExitCode` — `ExitCode` returns 1 when `_hasErrors` is true
- `SonarMark-Context-ResultAlias` — `--result` is accepted as a legacy alias for `--results`

## Backward Compatibility

### Legacy `--result` Alias

The `ArgumentParser` accepts `--result` as a fall-through case immediately before `--results`
in the switch statement. Both spellings invoke the same `GetRequiredStringArgument` call and
set the same `ResultsFile` property, so downstream code is unaffected.

This alias is intentionally omitted from help text and user-facing documentation because it
exists only to avoid breaking existing automation scripts that pre-date the canonical
`--results` spelling. New users and new scripts should always use `--results`.
