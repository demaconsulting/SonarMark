# SonarMark

## Verification Approach

SonarMark is verified through three complementary test layers, all executed by the
`test/DemaConsulting.SonarMark.Tests` xUnit project using the .NET test runner:

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

- .NET 8 SDK or later (multiple .NET versions including net8.0, net9.0, and net10.0 are all
  exercised in CI across Windows, Linux, and macOS platforms).
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
as an unknown argument, confirming that the enforcement argument is recognized and does not produce an
"Unsupported argument" error.
This scenario is tested by `IntegrationTest_EnforceFlag_IsAccepted`.

**EnforceFlag_FailingQualityGate_ReturnsNonZeroExitCode**: When invoked with `--enforce` and the
SonarQube/SonarCloud quality gate status is `ERROR`, the system returns a non-zero exit code,
confirming that enforcement mode correctly signals build failure to the CI/CD pipeline.
This scenario is tested by `Program_Run_WithEnforceFlag_AndFailingQualityGate_ReturnsNonZeroExitCode`.

**SonarMark-Core-Help**: When invoked with `--help`, the system displays usage help text including
all available options, confirming that the CLI argument is correctly recognized and the Program
dispatch writes usage information to the console.
This scenario is tested by `IntegrationTest_HelpFlag_OutputsUsageInformation`.

**SonarMark-Core-Version**: When invoked with `--version`, the system displays the tool version
string, confirming that the version string is correctly emitted by the full system.
This scenario is tested by `IntegrationTest_VersionFlag_OutputsVersion`.

**SonarMark-Core-Silent**: When invoked with `--silent`, the system suppresses console output
(including the banner), confirming that the flag is correctly parsed and all standard output is
suppressed throughout the full execution path.
This scenario is tested by `IntegrationTest_SilentFlag_SuppressesOutput`.

**SonarMark-Core-Token**: When invoked with `--token`, the system accepts the flag without
treating it as an unknown argument, confirming that the token argument is recognized and does
not interfere with subsequent argument processing.
This scenario is tested by `IntegrationTest_TokenParameter_IsAccepted`.

**SonarMark-Core-Branch**: When invoked with `--branch`, the system accepts the flag without
treating it as an unknown argument, confirming that branch selection is correctly parsed.
This scenario is tested by `IntegrationTest_BranchParameter_IsAccepted`.

**SonarMark-Core-ReportFile**: When invoked with `--report`, the system accepts the flag
without treating it as an unknown argument, confirming that the report path argument is
correctly parsed and does not cause an unrecognized-argument error.
This scenario is tested by `IntegrationTest_ReportParameter_IsAccepted`.

**SonarMark-Core-ReportDepth**: When invoked with `--depth` or `--report-depth` and invalid
or out-of-range values, the system rejects them with a clear error message, confirming that
both flags are accepted and that invalid values are correctly validated.
This scenario is tested by `IntegrationTest_ReportDepthWithoutValue_ShowsError`,
`IntegrationTest_ReportDepthWithInvalidValue_ShowsError`,
`IntegrationTest_ReportDepthWithZero_ShowsError`,
`IntegrationTest_DepthWithoutValue_ShowsError`,
`IntegrationTest_DepthWithInvalidValue_ShowsError`, and
`IntegrationTest_DepthWithZero_ShowsError`.
