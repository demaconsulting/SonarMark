## Cli

### Verification Approach

The Cli subsystem is verified by subsystem integration tests in
`test/DemaConsulting.SonarMark.Tests/Cli/CliTests.cs`. Most tests exercise the full dispatch path from
`Context.Create` through `Program.Run` directly, without a mock `HttpClientFactory`, since the covered
scenarios (version, help, silent mode, enforce-flag parsing, and unknown-argument parsing) do not reach
the network-bound code paths. One test, `Cli_EnforceMode_WithFailingQualityGate_ReturnsNonZeroExitCode`,
supplies a mocked `HttpClientFactory` returning a failing (`ERROR`) quality gate response to verify that
`--enforce` drives a non-zero exit code end-to-end through the full dispatch path. Each test confirms
that a specific combination of CLI flags results in the correct dispatch, output, and exit code,
verifying that `Context` and `Program` work together correctly as a subsystem.

### Test Environment

N/A - standard test environment.

### Acceptance Criteria

- All tests in `CliTests` pass with zero failures.
- Version and help dispatch produce the expected output without errors.
- Silent mode suppresses all console output.
- Enforce mode sets the enforce flag that drives the exit code.
- Enforce mode with a failing quality gate produces a non-zero exit code end-to-end.
- An unrecognized command-line argument throws `ArgumentException` before any dispatch occurs.

### Test Scenarios

**VersionDispatchOutputsVersionString**: When the CLI receives `--version`, the full dispatch path
outputs only the version string and exits with code 0, confirming that the `Context.Version` flag
routes correctly through `Program.Run`.
This scenario is tested by `Cli_VersionDispatch_WithVersionFlag_OutputsVersionString`.

**HelpDispatchOutputsHelpText**: When the CLI receives `--help`, the full dispatch path outputs the
usage and options help text and exits with code 0, confirming that the `Context.Help` flag routes
correctly through `Program.Run`.
This scenario is tested by `Cli_HelpDispatch_WithHelpFlag_OutputsHelpText`.

**SilentModeSuppressesOutput**: When the CLI receives `--silent`, no content is written to the console
even when an error condition is detected, confirming that the silent flag is honoured across the entire
output path.
This scenario is tested by `Cli_SilentMode_WithSilentFlag_SuppressesOutput`.

**EnforceModeSetsFlagForDownstreamProcessing**: When the CLI receives `--enforce`, the `Enforce`
property on `Context` is set to `true`, confirming that the flag is parsed and made available to
`Program.ProcessSonarAnalysis` for enforcement decisions.
This scenario is tested by `Cli_EnforceMode_WithEnforceFlag_SetsEnforceFlag`.

**EnforceModeWithFailingQualityGateReturnsNonZeroExitCode**: When the CLI receives `--enforce` and a
mocked `HttpClientFactory` returns a failing (`ERROR`) quality gate response, the full dispatch path
through `Program.Run` returns a non-zero exit code, confirming that enforce mode drives the exit code
end-to-end using an externally supplied HTTP client.
This scenario is tested by `Cli_EnforceMode_WithFailingQualityGate_ReturnsNonZeroExitCode`.

**UnknownArgumentThrowsArgumentException**: When the CLI receives an unrecognized flag, `Context.Create`
throws `ArgumentException` before any dispatch occurs, confirming that argument parsing rejects
unsupported flags up front rather than allowing them to reach `Program.Run`.
This scenario is tested by `Cli_ArgumentParsing_WithUnknownFlag_ThrowsArgumentException`.
