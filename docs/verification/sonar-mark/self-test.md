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
- `Program.Run` in validate mode with a null context throws `ArgumentNullException`.
- `Program.Run` in validate mode exits with code 0 when all internal self-validation tests pass.
- The validation header respects `context.Depth` for its heading level.
- When `--results` specifies a TRX path, the subsystem writes a valid TRX file containing the
  self-validation test suite name.
- When `--results` specifies an XML path, the subsystem writes a valid JUnit XML file containing
  the self-validation test suite content.

### Test Scenarios

**NullContextThrowsArgumentNullException**: `Validation.Run(null)` throws `ArgumentNullException`,
confirming that the subsystem entry point guards against a missing context before any validation
work begins.
This scenario is tested by `SelfTest_RunValidation_NullContext_ThrowsArgumentNullException`.

**AllTestsPass**: `Program.Run` with `--validate --silent` completes with `ExitCode = 0`, confirming
that all four internal self-validation scenarios pass and the subsystem pipeline integrates correctly.
This scenario is tested by `SelfTest_RunValidation_AllTestsPass`.

**RespectsContextDepth**: `Validation.Run` prints the validation header using `context.Depth` for the
heading level, confirming that the subsystem respects the configured report depth when producing its
header.
This scenario is tested by `Validation_Run_WithDepth2_OutputsLevel2Header` in
`test/DemaConsulting.SonarMark.Tests/SelfTest/ValidationTests.cs`.

**ProducesResultsFile**: `Program.Run` with `--validate --silent --results <trx-path>` creates a TRX
file at the specified path that contains the self-validation suite identifier, confirming that the
results file write path works end-to-end within the subsystem.
This scenario is tested by `SelfTest_RunValidation_ProducesResultsFile`.

**ProducesJUnitResultsFile**: `Program.Run` with `--validate --silent --results <xml-path>` creates a
JUnit XML file at the specified path that contains the self-validation test suite content, confirming
that the JUnit XML results file write path also works end-to-end within the subsystem.
This scenario is tested by `SelfTest_RunValidation_WithJUnitResultsPath_ProducesJUnitResultsFile`.
