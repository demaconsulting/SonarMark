# SonarMark

## Verification Approach

SonarMark is verified through three complementary test layers, all executed by the
`test/DemaConsulting.SonarMark.Tests` MSTest project using the .NET test runner:

- **Integration tests** (`IntegrationTests.cs`) launch the compiled `DemaConsulting.SonarMark.dll` as
  a child process via `dotnet` and assert on exit code and console output. These tests exercise the full
  application boundary without any mocking.
- **Self-validation tests** run through the `--validate` flag, which exercises the complete
  SonarQubeClient and ReportGeneration pipeline using a mock HTTP handler embedded in the binary. The
  four internal scenarios — quality gate retrieval, issues retrieval, hot-spots retrieval, and markdown
  report generation — are named and surfaced in the TRX/JUnit results file.
- **Unit and subsystem tests** in the sub-folders below verify individual software items in isolation.

System requirements traceability is maintained via the ReqStream trace matrix in
`docs/reqstream/sonar-mark/sonar-mark.yaml`.

## Test Environment

Tests require:

- .NET 8 SDK or later (net8.0, net9.0, and net10.0 targets are all exercised in CI).
- The compiled `DemaConsulting.SonarMark.dll` present in the test output directory (produced automatically
  because the test project references the main project).
- No external network access; all HTTP calls in the test layer are intercepted by a mock handler.

## Acceptance Criteria

- All automated tests in `test/DemaConsulting.SonarMark.Tests` pass with zero failures.
- The `--validate` flag exits with code 0 and reports four passed tests.
- The application exits with code 1 when required parameters are missing or a quality gate fails
  under `--enforce`.

## Test Scenarios

**QualityGateRetrieval**: The system retrieves quality gate status from a SonarQube/SonarCloud server and
includes the pass/fail result in the generated markdown report. This verifies that the CLI, HTTP client,
data model, and report renderer integrate correctly end-to-end.
This scenario is tested by `SonarMark_QualityGateRetrieval`.

**IssuesRetrieval**: The system fetches all paginated issues from a SonarQube/SonarCloud server and
includes them in the rendered report, confirming that the full data path from API call to markdown output
works correctly.
This scenario is tested by `SonarMark_IssuesRetrieval`.

**HotSpotsRetrieval**: The system fetches all paginated security hot-spots from a SonarQube/SonarCloud
server and includes them in the rendered report, confirming that the security hot-spot data path
integrates correctly.
This scenario is tested by `SonarMark_HotSpotsRetrieval`.

**MarkdownReportGeneration**: The system combines quality gate status, issues, and hot-spots into a single
complete markdown report and writes it to the path specified by `--report`, confirming that all system
components collaborate correctly to produce the final deliverable.
This scenario is tested by `SonarMark_MarkdownReportGeneration`.

**ValidateFlagOutputsHeaderAndSummary**: When invoked with `--validate`, the system runs self-tests and
emits a report header identifying the tool and a summary line showing the total test count, confirming
that the CLI dispatch, self-test runner, and output pipeline integrate correctly.
This scenario is tested by `IntegrationTest_ValidateFlag_OutputsHeaderAndSummary`.

**EnforceFlagIsAccepted**: When invoked with `--enforce`, the system accepts the flag without treating it
as an unknown argument, confirming that the enforcement path is wired through the CLI and exits with a
non-zero code only when quality gate status is failing.
This scenario is tested by `IntegrationTest_EnforceFlag_IsAccepted`.
