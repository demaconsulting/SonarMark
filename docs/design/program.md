# Program

## Overview

`Program` is the static entry point for the SonarMark application. It contains
`Main`, which wires up the `Context` from command-line arguments and calls the
`Run` method. `Run` dispatches to the appropriate subsystem based on the flags
present in the context.

## Design Decisions

### Priority-Based Dispatch

`Run` uses an explicit priority order: version first, then help, then
self-validation, then SonarQube analysis. This ensures that lightweight informational
flags (version, help) are handled immediately without requiring any server connectivity,
and that the validate flag takes precedence over a potentially incomplete set of server
parameters.

### Banner Printed Before Help

The application banner (version and copyright) is printed before the help text so that
users always see which version of the tool they are running, even when they request help.

### Exception Handling in Main

`Main` catches `ArgumentException` (from argument parsing) and
`InvalidOperationException` (from runtime errors such as failed file opens) and prints
their messages without a stack trace. Truly unexpected exceptions are re-thrown after
printing, so that the operating system or CI runner can produce an event log entry.

### ProcessSonarAnalysis Validation

`ProcessSonarAnalysis` validates that `--server` and `--project-key` are present
before attempting any network call. Missing required parameters are reported via
`context.WriteError`, which sets the error flag and drives the non-zero exit code.

## Satisfies Requirements

- `SonarMark-Cli-Interface` — `Main` is the CLI entry point
- `SonarMark-Cli-Version` — `Run` prints the version string and returns when `context.Version` is true
- `SonarMark-Cli-Help` — `Run` prints help and returns when `context.Help` is true
- `SonarMark-Server-Connect` — `ProcessSonarAnalysis` validates the `--server` parameter
- `SonarMark-Server-Auth` — `SonarQubeClient` is created with the token from context
- `SonarMark-Server-QualityGate` — `GetQualityResultByBranchAsync` fetches quality gate status
- `SonarMark-Server-Issues` — quality result includes the issues collection
- `SonarMark-Server-HotSpots` — quality result includes the hot-spots collection
- `SonarMark-Server-ProjectKey` — `ProcessSonarAnalysis` validates the `--project-key` parameter
- `SonarMark-Server-Branch` — branch is passed through to `GetQualityResultByBranchAsync`
- `SonarMark-Report-Markdown` — `File.WriteAllText` writes the markdown report when `--report` is set
- `SonarMark-Validation-Run` — `Run` delegates to `Validation.Run` when `context.Validate` is true
- `SonarMark-Enforce-ExitCode` — `context.ExitCode` is returned from `Main`
