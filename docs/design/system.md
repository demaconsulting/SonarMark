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

```text
CLI arguments
    │
    ▼
Context.Create          — parses flags; provides server, project-key, branch, token
    │
    ▼
Program.ProcessSonarAnalysis
    │   validates --server and --project-key are present
    │
    ▼
SonarQubeClient         — issues async HTTP requests to SonarQube/SonarCloud API
    │   GetQualityResultByBranchAsync:
    │     • fetches quality gate status
    │     • fetches issues collection
    │     • fetches security hot-spots collection
    │
    ▼
SonarQualityResult      — aggregates QualityGate, Issues, HotSpots into one object
    │   ToMarkdown():
    │     • renders markdown report string
    │
    ▼
File.WriteAllText       — writes rendered markdown to the --report path (if supplied)
    │
    ▼
context.ExitCode        — 0 on success; 1 if --enforce and quality gate failed
```

## Design Decisions

### Single-Process Architecture

SonarMark runs entirely within a single .NET process. There is no inter-process
communication, no microservices, and no shared-memory coordination. This keeps
deployment simple (a single `dotnet tool install` command) and eliminates an entire
class of distributed-systems failure modes, which is appropriate for a CLI reporting
tool.

### Async/Await HTTP Pipeline

All SonarQube API calls are performed using `async`/`await` throughout the call stack,
from `SonarQubeClient` up through `Program.ProcessSonarAnalysis`. This allows the
application to remain responsive and avoids thread-pool exhaustion on longer requests
without introducing background threads or callback chains. The entry point `Main` is
declared `async Task<int>` to support this cleanly.

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
