## SelfTest

### Verification Approach

The SelfTest subsystem is verified by subsystem integration tests in
`test/DemaConsulting.SonarMark.Tests/SelfTest/SelfTestTests.cs`. These tests call `Program.Run` with a
`Context` configured for `--validate` mode and assert on the context exit code and, where applicable,
the content of a results file written to a temporary directory. The `Validation.Run` method uses its own
internal mock HTTP handler, so no external network access is required. Each test verifies an observable
subsystem output (exit code, file existence, file content) rather than internal implementation details.

### Test Environment

N/A - standard test environment.

### Acceptance Criteria

- All tests in `SelfTestTests` pass with zero failures.
- `Program.Run` in validate mode exits with code 0 when all internal self-validation tests pass.
- When `--results` specifies a TRX path, the subsystem writes a valid TRX file containing the
  self-validation test suite name.

### Test Scenarios

**AllTestsPass**: `Program.Run` with `--validate --silent` completes with `ExitCode = 0`, confirming
that all four internal self-validation scenarios pass and the subsystem pipeline integrates correctly.
This scenario is tested by `SelfTest_RunValidation_AllTestsPass`.

**ProducesResultsFile**: `Program.Run` with `--validate --silent --results <trx-path>` creates a TRX
file at the specified path that contains the self-validation suite identifier, confirming that the
results file write path works end-to-end within the subsystem.
This scenario is tested by `SelfTest_RunValidation_ProducesResultsFile`.
