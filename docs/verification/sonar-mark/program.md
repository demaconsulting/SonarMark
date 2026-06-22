## Program

### Verification Approach

`Program` is verified by unit tests in `test/DemaConsulting.SonarMark.Tests/ProgramTests.cs`. Each test
constructs a `Context` directly and calls `Program.Run`, capturing console output through `Console.SetOut`
or `Console.SetError` redirection. Network dependencies are eliminated by supplying a mock
`HttpClientFactory` on the context, which returns a pre-configured `SonarQubeClient` backed by a
`MockHttpMessageHandler` carrying canned JSON responses. The real `HttpClient` stack is never invoked.

### Test Environment

N/A - standard test environment.

### Acceptance Criteria

- All unit tests in `ProgramTests` pass with zero failures.
- `Program.Run` with `--version` outputs only the version string and returns exit code 0.
- `Program.Run` with `--help` outputs the banner and full help text and returns exit code 0.
- `Program.Run` with `--enforce` and a failing quality gate returns exit code 1.
- `Program.Run` writes a markdown file to the path specified by `--report` when a mock client is injected.

### Test Scenarios

**VersionPropertyReturnsNonEmptyString**: `Program.Version` returns a non-empty string, confirming that
the assembly version attribute is set correctly and accessible at runtime.
This scenario is tested by `Program_Version_WhenAccessed_ReturnsNonEmptyString`.

**VersionFlagOutputsVersionString**: When run with `--version`, `Program.Run` writes only the version
string to standard output and exits with code 0, confirming that the version dispatch path is wired
correctly.
This scenario is tested by `Program_Run_WithVersionFlag_OutputsVersionString`.

**HelpFlagOutputsBannerAndHelpText**: When run with `--help`, `Program.Run` writes the application banner
and the full usage/options help text to standard output, confirming that the help dispatch path produces
the expected content.
This scenario is tested by `Program_Run_WithHelpFlag_OutputsBannerAndHelpText`.

**ValidateFlagRunsValidationSuccessfully**: When run with `--validate`, `Program.Run` executes the
self-validation suite, outputs the `"DEMA Consulting SonarMark"` header, reports `"Passed: 4"` for all
four internal scenarios, and exits with code 0, confirming that the validate dispatch path reaches the
`Validation` subsystem.
This scenario is tested by `Program_Run_WithValidateFlag_RunsValidationSuccessfully`.

**NoArgumentsOutputsServerRequiredError**: When run with no arguments, `Program.Run` outputs the banner
and writes an error indicating that `--server` is required, returning exit code 1.
This scenario is tested by `Program_Run_WithNoArguments_OutputsBannerAndRequiresServerError`.

**ServerWithoutProjectKeyOutputsError**: When `--server` is provided without `--project-key`,
`Program.ProcessSonarAnalysis` writes an error and returns exit code 1, confirming that parameter
validation guards network calls.
This scenario is tested by `Program_Run_WithServerButNoProjectKey_OutputsProjectKeyRequiredError`.

**SilentFlagSuppressesBannerOutput**: When run with `--silent`, the banner is not written to standard
output, confirming that silent mode is honoured before any output is produced.
This scenario is tested by `Program_Run_WithSilentFlag_SuppressesBannerOutput`.

**EnforceFlagWithFailingQualityGateReturnsNonZeroExitCode**: When run with `--enforce` and a mock client
returning a failing quality gate, `Program.Run` calls `context.WriteError` and returns exit code 1,
confirming enforcement mode drives the exit code correctly.
This scenario is tested by `Program_Run_WithEnforceFlagAndFailingQualityGate_ReturnsNonZeroExitCode`.

**ReportFileWritesMarkdownToFile**: When `--report` specifies a file path and a mock client is injected,
`Program.Run` writes the rendered markdown to that file, confirming the report write path works end-to-end.
This scenario is tested by `Program_Run_WithReportFile_WritesMarkdownToFile`.

**ReportDepthFlagWritesHeadingsAtCorrectLevel**: When `--report-depth` specifies a depth, the markdown
written to the report file uses headings at the correct level, confirming that the deprecated alias is
passed through to `ToMarkdown`.
This scenario is tested by `Program_Run_WithReportDepth_WritesReportWithDepthHeadings`.

**DepthFlagWritesHeadingsAtCorrectLevel**: When `--depth` specifies a depth, the markdown written to the
report file uses headings at the correct level, confirming that the canonical flag is passed through to
`ToMarkdown`.
This scenario is tested by `Program_Run_WithDepth_WritesReportWithDepthHeadings`.

**TokenAddsAuthorizationHeaderToRequests**: When `--token` is provided, the HTTP requests sent by
`SonarQubeClient` include a Basic Authorization header constructed from the token, confirming that token
authentication is wired through `Program.ProcessSonarAnalysis`.
This scenario is tested by `Program_Run_WithToken_AddsAuthorizationHeaderToServerRequests`.

**BranchFlagIncludesBranchInServerRequest**: When `--branch` is provided, the branch parameter is
appended to the SonarQube API URL, confirming that branch filtering is passed through to
`GetQualityResultByBranchAsync`.
This scenario is tested by `Program_Run_WithBranchFlag_IncludesBranchInServerRequest`.
