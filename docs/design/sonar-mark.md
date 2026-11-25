# SonarMark

![SonarMark Structure](SonarMarkView.svg)

## Architecture

SonarMark operates as a single-process .NET CLI tool. The four subsystems collaborate in
sequence under the `Program` entry point:

```mermaid
flowchart TD
    subgraph Cli
        Context
        HelpCommand
    end
    subgraph SonarIntegration
        SonarQubeClient
        SonarHotSpot
        SonarIssue
    end
    subgraph ReportGeneration
        SonarQualityResult
    end
    subgraph SelfTest
        Validation
    end
    Program --> HelpCommand
    Program --> Context
    Program --> SonarQubeClient
    Program --> SonarQualityResult
    Program --> Validation
    SonarQubeClient --> SonarHotSpot
    SonarQubeClient --> SonarIssue
    SonarQualityResult --> SonarHotSpot
    SonarQualityResult --> SonarIssue
```

There is no system-level code. `Program` acts as the orchestrator. It first offers the
arguments to `HelpCommand` (in `Cli`), which answers the `help` command from the built-in help
cards. Otherwise it delegates to the appropriate subsystem based on the flags present in
`Context`.

## External Interfaces

**CLI Interface**: The `sonarmark` command-line executable.

- *Type*: CLI
- *Role*: Provider — the host environment (shell, CI runner) invokes this interface.
- *Contract*: Accepts the `help` command as the first argument (see *Help Command* below), or
  flags `--server`, `--project-key`, `--branch`, `--token`, `--report`,
  `--depth`, `--report-depth`, `--enforce`, `--validate`, `--results`, `--silent`, `--log`,
  `--version`, and `--help`. Returns exit code 0 on success, 1 on error or quality-gate
  enforcement failure.
- *Constraints*: `--server` and `--project-key` are required when running analysis. `--depth`
  must be an integer in the range 1–6. `--report-depth` is a deprecated alias for `--depth`
  and also accepts the range 1–6. When `--token` is not supplied on the command line, the
  `SONAR_TOKEN` environment variable is read as a fallback; the explicit `--token` flag always
  takes priority over the environment variable. Argument errors are found before `--silent`
  and `--log` take effect, so they are always printed and never logged.

**Help Command**: Built-in help, answered from help cards embedded in the tool assembly.

- *Type*: CLI
- *Role*: Provider — the tool answers the `help` command from the shell.
- *Contract*: `sonarmark help` prints the topic index and `sonarmark help <topic>` prints a
  topic card with its examples; HelpCards adds `--example`, `--search`, and `--json`. Returns exit
  code 0 for success, 2 for a wrong help argument or an unknown topic, and 70 for a defective card
  catalog.
- *Constraints*: `help` must be the first argument. It is answered before the options are parsed,
  so `--silent` and `--log` do not apply and no banner is printed.

**SonarQube/SonarCloud REST API**: The HTTP API exposed by the remote server.

- *Type*: HTTP/REST
- *Role*: Consumer — SonarMark calls out to the server.
- *Contract*: Uses five endpoints: `/api/components/show`, `/api/qualitygates/project_status`,
  `/api/metrics/search`, `/api/issues/search`, and `/api/hotspots/search`. Responses are JSON.
  Authentication uses HTTP Basic Authorization with a PAT as the username and an empty password.
- *Constraints*: Non-2xx responses raise `InvalidOperationException`. Pagination is handled
  automatically for issues and hot-spots.

**Markdown Report File**: The optional file written when `--report` is specified.

- *Type*: File
- *Role*: Provider — SonarMark writes this file for downstream tools.
- *Contract*: GitHub-flavored markdown containing a project header with dashboard link,
  quality gate status, a conditions table, an issues list, and a security hot-spots list.
- *Constraints*: The `--depth` flag controls the top-level heading depth (1–6).

**Validation Results File**: The optional TRX or JUnit XML file written when `--results` is
specified alongside `--validate`.

- *Type*: File
- *Role*: Provider — SonarMark writes this file for CI test result consumers.
- *Contract*: TRX format for `.trx` extension; JUnit XML format for `.xml` extension.
- *Constraints*: Written only when `--validate` is also passed.

## Dependencies

- **DemaConsulting.TestResults**: provides `TestResults`, `TestResult`, `TestOutcome`, and
  `TestResultsIO` types used by the SelfTest subsystem — see *DemaConsulting.TestResults*
- **DemaConsulting.HelpCards**: used by `HelpCommand` to load, validate, and serve the built-in
  help cards for the `help` command — see *DemaConsulting.HelpCards*

## Risk Control Measures

N/A - not a safety-classified software item.

## Data Flow

When invoked for SonarQube analysis, data moves through the system in the following steps:

1. `Program.Main` offers the arguments to `HelpCommand.TryRun`. When the first argument is
   `help`, the help command builds the card catalog, writes its answer to the console, and
   `Program.Main` returns its exit code. Any other first argument returns at once without
   building the catalog.
2. `Context.Create` parses CLI flags and provides server, project-key, branch, and
   authentication token to the rest of the system.
3. `Program.ProcessSonarAnalysis` validates that `--server` and `--project-key` are present
   before any network activity begins.
4. `SonarQubeClient.GetQualityResultByBranchAsync` issues async HTTP requests to five
   SonarQube/SonarCloud API endpoints and assembles the results into a `SonarQualityResult`.
5. `SonarQualityResult.ToMarkdown` renders the aggregated data as a markdown report string.
6. `File.WriteAllText` writes the rendered markdown to the `--report` path when supplied.
7. `context.ExitCode` returns 0 on success, or 1 if any error was written via
   `context.WriteError`.

## Design Constraints

- Platform: targets multiple .NET versions (supported .NET LTS and current releases); runs
  natively on Windows, Linux, and macOS with no platform-specific code paths.
- Architecture: single-process; no inter-process communication, microservices, or
  shared-memory coordination.
- Output format: plain GitHub-flavored markdown; no binary output formats.
- Network: all SonarQube/SonarCloud calls are asynchronous (async/await); the orchestration
  layer in `Program` performs a deliberate sync-over-async bridge.
- Self-test: validation runs inside the same process and binary as the analysis path,
  guarded by the `--validate` flag.
