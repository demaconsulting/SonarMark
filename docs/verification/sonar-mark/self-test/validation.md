### Validation

#### Verification Approach

`Validation` is verified by unit tests in
`test/DemaConsulting.SonarMark.Tests/SelfTest/ValidationTests.cs`. Each test creates a `Context` with
specific flags, calls `Validation.Run` directly, and asserts on observable outputs: console output via
`Console.SetOut` redirection, exit code via `context.ExitCode`, and result files written to a temporary
directory created per test. The `Validation` unit uses its own internal mock HTTP handler, so no network
calls are made. Temporary directories are cleaned up via `Dispose` (the test class implements
`IDisposable`).

#### Test Environment

N/A - standard test environment.

#### Acceptance Criteria

- All unit tests in `ValidationTests` pass with zero failures.
- `Validation.Run(null)` throws `ArgumentNullException`.
- `Validation.Run` with a silent context exits with code 0.
- The validation header contains the project name string at the configured heading depth.
- The validation summary reports four passed tests.
- TRX and JUnit XML result files are written correctly when `--results` is provided.
- An unsupported results file extension causes an error message and non-zero exit code.

#### Test Scenarios

**NullContextThrowsArgumentNullException**: `Validation.Run(null)` throws `ArgumentNullException`,
confirming that the null-guard at the top of the method is present and fires before any other logic.
This scenario is tested by `Validation_Run_WithNullContext_ThrowsArgumentNullException`.

**SilentContextCompletesWithZeroExitCode**: `Validation.Run` with a silent context completes without
error and the exit code is 0, confirming that all four internal validation scenarios pass and none call
`context.WriteError`.
This scenario is tested by `Validation_Run_WithSilentContext_CompletesWithZeroExitCode`.

**OutputsValidationHeader**: `Validation.Run` writes a header line containing the DEMA Consulting
SonarMark identifier to standard output, confirming that the report header is generated at the correct
heading level by default.
This scenario is tested by `Validation_Run_DefaultContext_OutputsValidationHeader`.

**WithDepth2OutputsLevel2Header**: `Validation.Run` with `--depth 2` writes a level-2 heading for the
validation header, confirming that the heading level respects the configured report depth.
This scenario is tested by `Validation_Run_WithDepth2_OutputsLevel2Header`.

**ReportsFourPassedTests**: The validation output includes a "Total Tests:" line reflecting four passed
tests, confirming that all four internal validation scenarios are registered and executed.
This scenario is tested by `Validation_Run_FourInternalTests_ReportsFourPassedTests`.

**WithTrxResultsFileWritesTrxFile**: `Validation.Run` with `--results <path>.trx` creates a TRX file at
the specified path, confirming that the TRX serialization path in `WriteResultsFile` is invoked and
produces a file.
This scenario is tested by `Validation_Run_WithTrxResultsFile_WritesTrxFile`.

**WithXmlResultsFileWritesJUnitFile**: `Validation.Run` with `--results <path>.xml` creates a JUnit XML
file at the specified path, confirming that the JUnit serialization path in `WriteResultsFile` is
invoked and produces a file.
This scenario is tested by `Validation_Run_WithXmlResultsFile_WritesJUnitFile`.

**WithUnsupportedResultsExtensionReportsError**: `Validation.Run` with `--results <path>.csv` writes an
error message via `context.WriteError` and does not throw, confirming that unsupported result formats
are handled gracefully without crashing the process.
This scenario is tested by `Validation_Run_WithUnsupportedResultsExtension_ReportsError`.
