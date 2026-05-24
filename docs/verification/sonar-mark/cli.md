## Cli

### Verification Approach

The Cli subsystem is verified by subsystem integration tests in
`test/DemaConsulting.SonarMark.Tests/Cli/CliTests.cs`. These tests exercise the full dispatch path from
`Context.Create` through `Program.Run`, using a mock `HttpClientFactory` to avoid network calls.
Each test confirms that a specific combination of CLI flags results in the correct dispatch, output,
and exit code, verifying that `Context` and `Program` work together correctly as a subsystem.

### Test Environment

N/A - standard test environment.

### Acceptance Criteria

- All tests in `CliTests` pass with zero failures.
- Version and help dispatch produce the expected output without errors.
- Silent mode suppresses all console output.
- Enforce mode sets the enforce flag that drives the exit code.

### Test Scenarios

**VersionDispatchOutputsVersionString**: When the CLI receives `--version`, the full dispatch path
outputs only the version string and exits with code 0, confirming that the `Context.Version` flag
routes correctly through `Program.Run`.
This scenario is tested by `Cli_VersionDispatch_OutputsVersionString`.

**HelpDispatchOutputsHelpText**: When the CLI receives `--help`, the full dispatch path outputs the
usage and options help text and exits with code 0, confirming that the `Context.Help` flag routes
correctly through `Program.Run`.
This scenario is tested by `Cli_HelpDispatch_OutputsHelpText`.

**SilentModeSuppressesOutput**: When the CLI receives `--silent`, no content is written to the console
even when an error condition is detected, confirming that the silent flag is honoured across the entire
output path.
This scenario is tested by `Cli_SilentMode_SuppressesOutput`.

**EnforceModeSetsFlagForDownstreamProcessing**: When the CLI receives `--enforce`, the `Enforce`
property on `Context` is set to `true`, confirming that the flag is parsed and made available to
`Program.ProcessSonarAnalysis` for enforcement decisions.
This scenario is tested by `Cli_EnforceMode_SetsEnforceFlag`.
