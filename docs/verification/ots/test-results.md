## DemaConsulting.TestResults

### Verification Approach

`DemaConsulting.TestResults` is verified through integration tests in the SonarMark test suite
that exercise the package through the SelfTest subsystem. HTTP calls to SonarQube are replaced
by a mock handler to keep tests in-process and offline; the serialization code itself
(`TestResultsIO.WriteTrx`, `TestResultsIO.WriteJUnit`) is exercised against real output paths.
The resulting files are then inspected to confirm the expected content is present.

The tests verify that:

- `TestResults` accumulates test outcomes written by `Validation.Run` without error.
- `TestResultsIO.WriteTrx` produces a valid `.trx` file containing the SonarMark self-validation
  test suite content when the `--results` flag specifies a `.trx` path.
- `TestResultsIO.WriteJUnit` produces a valid JUnit XML file containing the expected `testsuite`
  element when the `--results` flag specifies a `.xml` path.

### Test Environment

Tests require:

- No network access; verification is entirely in-process.
- A writable temporary directory for the output results files.
- The `DemaConsulting.TestResults` NuGet package installed as a package reference in the
  production project (`src/DemaConsulting.SonarMark/DemaConsulting.SonarMark.csproj`).

### Acceptance Criteria

The OTS integration is accepted when all linked tests pass with zero failures and the output
files contain the expected content markers (`SonarMark Self-Validation` in TRX output and
`testsuite` in JUnit XML output).

### Test Scenarios

**TestResultsCollection**: Runs the full self-validation pipeline with `--validate --silent` and
confirms exit code 0, proving that `TestResults` accumulates all four scenario outcomes without
error. Tested by `SelfTest_RunValidation_AllTestsPass`.

**TrxSerializationOutput**: Runs the self-validation pipeline with a `.trx` results path and
confirms the file is created and contains `SonarMark Self-Validation`, proving that
`TestResultsIO.WriteTrx` serializes correctly. Tested by `SelfTest_RunValidation_ProducesResultsFile`.

**JUnitSerializationOutput**: Runs the self-validation pipeline with a `.xml` results path and
confirms the file is created and contains a `testsuite` element, proving that
`TestResultsIO.WriteJUnit` serializes correctly.
Tested by `SelfTest_RunValidation_WithJUnitResultsPath_ProducesJUnitResultsFile`.
