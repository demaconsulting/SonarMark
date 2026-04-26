# System

## Overview

SonarMark operates as a single-process .NET CLI tool. When invoked for analysis, the
four subsystems collaborate in sequence:

1. **CLI** — `Context` parses arguments and `Program` validates required parameters
   before any network call is made.
2. **SonarQube Integration** — `SonarQubeClient` fetches the quality gate status,
   issues, and security hot-spots from the SonarQube/SonarCloud HTTP API.
3. **Report Generation** — `SonarQualityResult` aggregates the fetched data and
   renders it as a markdown document.
4. **Validation** — when `--validate` is supplied instead of analysis parameters,
   the `Validation` subsystem runs self-tests and writes TRX or JUnit result files.

The subsystems share no global state. Data flows forward through method parameters and
return values; no subsystem reaches back into another's internals.

## End-to-End Data Flow

When invoked for SonarQube analysis, data moves through the system in the following steps:

1. **`Context.Create`** — parses CLI flags and provides server, project-key, branch, and
   authentication token to the rest of the system.
2. **`Program.ProcessSonarAnalysis`** — validates that `--server` and `--project-key` are
   present before any network activity begins.
3. **`SonarQubeClient.GetQualityResultByBranchAsync`** — issues async HTTP requests to the
   SonarQube/SonarCloud API to fetch the quality gate status, issues collection, and
   security hot-spots collection.
4. **`SonarQualityResult`** — aggregates the quality gate status, issues, and hot-spots
   into a single object.
5. **`SonarQualityResult.ToMarkdown`** — renders the aggregated data as a markdown report
   string.
6. **`File.WriteAllText`** — writes the rendered markdown to the `--report` path if one
   was supplied.
7. **`context.ExitCode`** — is 0 when no errors are reported, or 1 if any error is written
   via `context.WriteError(...)` (for example, missing required arguments, fetch or
   report-write failures, or a `--enforce` quality gate failure).

## Design Decisions

### Single-Process Architecture

SonarMark runs entirely within a single .NET process. There is no inter-process
communication, no microservices, and no shared-memory coordination. This keeps
deployment simple (a single `dotnet tool install` command) and eliminates an entire
class of distributed-systems failure modes, which is appropriate for a CLI reporting
tool.

### Async/Await HTTP Pipeline

SonarQube HTTP calls are exposed as asynchronous methods on `SonarQubeClient` (for
example, `GetQualityResultByBranchAsync`), implemented using `async`/`await` to avoid
blocking threads during network I/O. The orchestration layer, `Program.ProcessSonarAnalysis`,
remains a synchronous method and performs a deliberate sync-over-async bridge by calling
`GetQualityResultByBranchAsync(...).GetAwaiter().GetResult()`. The entry point `Main` is a
standard synchronous `int` method that invokes `ProcessSonarAnalysis` and does not interact
with asynchronous APIs directly.

### Markdown Report Format

The output format is plain GitHub-flavoured markdown. Markdown was chosen because it
is human-readable as plain text, renders natively in GitHub pull-request comments and
repository views, and can be consumed by downstream tools (static site generators,
documentation pipelines) without a separate conversion step.

### Integrated Validation Mode

Self-validation runs inside the same process and binary as the analysis path, guarded
by the `--validate` flag. This means the validation suite always tests the exact build
artifact that will be shipped. The `Validation` subsystem writes standard TRX and
JUnit XML result files so that CI runners can consume the results without custom
post-processing.

## Satisfies Requirements

- `SonarMark-System-QualityGate` — `SonarQubeClient` retrieves quality gate status and
  `SonarQualityResult` includes it in the rendered markdown report
- `SonarMark-System-Issues` — `SonarQubeClient` retrieves the issues collection and
  `SonarQualityResult` includes it in the rendered markdown report
- `SonarMark-System-HotSpots` — `SonarQubeClient` retrieves security hot-spots and
  `SonarQualityResult` includes them in the rendered markdown report
- `SonarMark-System-Report` — `SonarQualityResult.ToMarkdown` combines all retrieved
  analysis data and `Program` writes the complete markdown report to disk
- `SonarMark-System-Validation` — `Program` dispatches to the `Validation` subsystem
  when `--validate` is supplied; the subsystem runs self-tests and emits a summary
- `SonarMark-System-Enforce` — `Program` reads `Context.Enforce` and calls
  `context.WriteError` when the quality gate status is not passing, causing a non-zero
  exit code
- `SonarMark-Platform-Windows` — the single-process .NET architecture runs natively on
  Windows with no platform-specific code paths
- `SonarMark-Platform-Linux` — the single-process .NET architecture runs natively on
  Linux with no platform-specific code paths
- `SonarMark-Platform-MacOS` — the single-process .NET architecture runs natively on
  macOS with no platform-specific code paths
- `SonarMark-Platform-Net8` — the project targets `net8.0` in the SDK multi-targeting
  list, enabling the tool to run on the .NET 8 runtime
- `SonarMark-Platform-Net9` — the project targets `net9.0` in the SDK multi-targeting
  list, enabling the tool to run on the .NET 9 runtime
- `SonarMark-Platform-Net10` — the project targets `net10.0` in the SDK multi-targeting
  list, enabling the tool to run on the .NET 10 runtime
