### Context

#### Verification Approach

`Context` is verified by unit tests in `test/DemaConsulting.SonarMark.Tests/Cli/ContextTests.cs`. All
dependencies on the file system (log file creation) are handled via a temporary directory created and
destroyed per test. Console output is verified by redirecting `Console.Out` and `Console.Error` to
`StringWriter` instances. No network dependencies are involved. The `Context.Create` factory method is
called directly with various argument arrays, and the resulting properties and exceptions are asserted.

#### Test Environment

N/A - standard test environment.

#### Acceptance Criteria

- All unit tests in `ContextTests` pass with zero failures.
- `Context.Create` with no arguments returns a default context with `Version=false`, `Help=false`,
  `Silent=false`, `Validate=false`, `Depth=1`, and `ExitCode=0`.
- Invalid or missing argument values throw `ArgumentException` with a descriptive message.
- `WriteError` always sets `ExitCode` to 1 regardless of silent mode.

#### Test Scenarios

**NoArgumentsReturnsDefaultContext**: `Context.Create` with an empty argument array creates a context
with all flags at their default values, confirming that no flags are set without explicit input.
This scenario is tested by `Context_Create_NoArguments_ReturnsDefaultContext`.

**VersionFlagSetsVersionProperty**: Both `-v` and `--version` set `context.Version = true`,
confirming that both short and long flag spellings are accepted.
This scenario is tested by `Context_Create_VersionFlag_SetsVersionProperty`.

**HelpFlagsSetsHelpProperty**: The `-?`, `-h`, and `--help` flags all set `context.Help = true`,
confirming that all three accepted spellings are handled.
This scenario is tested by `Context_Create_HelpFlags_SetsHelpProperty`.

**SilentFlagSetsSilentProperty**: `--silent` sets `context.Silent = true` and the exit code remains 0,
confirming that the silent flag is parsed without side effects.
This scenario is tested by `Context_Create_SilentFlag_SetsSilentProperty`.

**ValidateFlagSetsValidateProperty**: `--validate` sets `context.Validate = true` and the exit code
remains 0, confirming that the validation dispatch flag is parsed correctly.
This scenario is tested by `Context_Create_ValidateFlag_SetsValidateProperty`.

**EnforceFlagSetsEnforceProperty**: `--enforce` sets `context.Enforce = true` and the exit code
remains 0, confirming that the enforcement flag is parsed and made available for downstream use.
This scenario is tested by `Context_Create_EnforceFlag_SetsEnforceProperty`.

**ReportFileSetsReportProperty**: `--report report.md` stores the file path in `context.ReportFile`,
confirming that the report output path is parsed and accessible.
This scenario is tested by `Context_Create_ReportFile_SetsReportProperty`.

**MissingReportFilenameThrowsException**: `--report` without a following filename throws
`ArgumentException` containing the flag name, confirming that the factory method validates required
argument values.
This scenario is tested by `Context_Create_MissingReportFilename_ThrowsException`.

**ReportDepthAliasSetsDepthProperty**: `--report-depth 3` sets `context.Depth = 3`, confirming that the
deprecated `--report-depth` alias is still accepted and maps to the same property.
This scenario is tested by `Context_Create_ReportDepthAlias_SetsDepthProperty`.

**DepthSetsDepthProperty**: `--depth 3` sets `context.Depth = 3`, confirming that the canonical
`--depth` flag is parsed correctly.
This scenario is tested by `Context_Create_Depth_SetsDepthProperty`.

**InvalidDepthThrowsException**: `--depth` with a non-integer value, zero, or a value outside 1–6
throws `ArgumentException`, confirming that the depth validation range is enforced for both spellings.
This scenario is tested by `Context_Create_InvalidDepth_ThrowsException`.

**TokenSetsTokenProperty**: `--token test-token-123` stores the value in `context.Token`, confirming
that the authentication token is parsed and made available to downstream HTTP calls.
This scenario is tested by `Context_Create_Token_SetsTokenProperty`.

**ServerSetsServerProperty**: `--server https://sonarcloud.io` stores the URL in `context.Server`,
confirming that the server base URL is parsed and available for `SonarQubeClient` construction.
This scenario is tested by `Context_Create_Server_SetsServerProperty`.

**ProjectKeySetsProjectKeyProperty**: `--project-key my-project` stores the key in
`context.ProjectKey`, confirming that the project identifier is parsed and available for API calls.
This scenario is tested by `Context_Create_ProjectKey_SetsProjectKeyProperty`.

**BranchSetsBranchProperty**: `--branch main` stores the branch name in `context.Branch`, confirming
that branch filtering is parsed and available for API calls.
This scenario is tested by `Context_Create_Branch_SetsBranchProperty`.

**UnsupportedArgumentThrowsException**: An unrecognized flag such as `--unsupported` throws
`ArgumentException` containing the flag name, confirming that unknown flags are rejected early.
This scenario is tested by `Context_Create_UnsupportedArgument_ThrowsException`.

**WriteLineNormalModeWritesToConsole**: `context.WriteLine("message")` in normal mode writes the message
to standard output, confirming that output routing works correctly when silent mode is not active.
This scenario is tested by `Context_WriteLine_NormalMode_WritesToConsole`.

**WriteLineSilentModeDoesNotWriteToConsole**: `context.WriteLine("message")` in silent mode produces no
console output, confirming that the silent flag is honoured by the output routing logic.
This scenario is tested by `Context_WriteLine_SilentMode_DoesNotWriteToConsole`.

**WriteErrorNormalModeWritesToConsoleAndSetsExitCode**: `context.WriteError("Error message")` writes to
standard error and sets `ExitCode` to 1, confirming that error output and exit code tracking are linked.
This scenario is tested by `Context_WriteError_NormalMode_WritesToConsole`.

**WriteErrorSilentModeDoesNotWriteToConsole**: `context.WriteError("Error message")` in silent mode
produces no console output but still sets `ExitCode` to 1, confirming that silent mode suppresses
output without affecting the error state.
This scenario is tested by `Context_WriteError_SilentMode_DoesNotWriteToConsole`.

**WithLogFileWritesToLogFile**: When `--log` specifies a file path, both `WriteLine` and `WriteError`
messages are mirrored to the log file, confirming that log-file mirroring works regardless of silent mode.
This scenario is tested by `Context_Create_WithLogFile_WritesToLogFile`.

**InvalidLogFilePathThrowsException**: `--log` with a path in a non-existent directory throws
`InvalidOperationException` containing "Failed to open log file", confirming that file-open failures are
surfaced as a typed exception.
This scenario is tested by `Context_Create_InvalidLogFilePath_ThrowsException`.

**ResultsFileSetsResultsProperty**: `--results results.trx` stores the path in `context.ResultsFile`,
confirming that the validation results file is parsed and available for the `Validation` subsystem.
This scenario is tested by `Context_Create_ResultsFile_SetsResultsProperty`.

**ResultAliasSetsResultsProperty**: `--result results.trx` (legacy alias) stores the same value in
`context.ResultsFile`, confirming that the legacy spelling is accepted for backward compatibility.
This scenario is tested by `Context_Create_ResultAlias_SetsResultsProperty`.
