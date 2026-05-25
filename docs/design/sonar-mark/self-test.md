## SelfTest

### Overview

The SelfTest subsystem provides a built-in self-validation mode that verifies the tool's
core workflows without requiring a live SonarQube/SonarCloud server or any external network
access. It contains a single unit, `Validation`, which runs four integration scenarios using
a mock HTTP client and optionally writes the results to a TRX or JUnit XML file.

### Interfaces

**Validation.Run**: Entry point for the self-validation mode.

- *Type*: In-process .NET public API.
- *Role*: Provider — called by `Program.Run` when `--validate` is supplied.
- *Contract*: Accepts a `Context`. Prints a validation header using `context.Depth` for the
  heading level, runs four test scenarios (`SonarMark_QualityGateRetrieval`,
  `SonarMark_IssuesRetrieval`, `SonarMark_HotSpotsRetrieval`,
  `SonarMark_MarkdownReportGeneration`), prints a pass/fail summary, and optionally writes
  results to `context.ResultsFile`.
- *Constraints*: Throws `ArgumentNullException` when `context` is null. Individual test
  failures are reported via `context.WriteError` without exception propagation.

### Design

1. `Program.Run` calls `Validation.Run(context)` when `context.Validate` is `true`.
2. `Validation.Run` creates a `DemaConsulting.TestResults.TestResults` collection and a mock
   `SonarQubeClient` factory via `CreateMockHttpClient`, then calls four `RunValidationTest`
   helpers in sequence.
3. Each `RunValidationTest` creates a `TemporaryDirectory`, assembles command-line arguments
   (`--silent --log {file} --server {MockServerUrl} --project-key {MockProjectKey}` plus
   optional `--report`), creates a `Context` with the mock factory, calls `Program.Run`,
   reads the log and report files, and validates their contents using a per-test validator
   function.
4. Results are collected into `testResults`; pass/fail counts are printed via the context.
5. When `context.ResultsFile` is non-null, `WriteResultsFile` serializes the results to TRX
   (`.trx`) or JUnit XML (`.xml`) format.
