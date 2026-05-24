### Validation

#### Purpose

`Validation` provides the self-validation capability of SonarMark. When the tool is invoked
with `--validate`, `Validation.Run` executes a suite of four internal test scenarios using a
mocked HTTP client, verifying that the tool's core workflows function correctly without
requiring a real SonarQube/SonarCloud server.

#### Data Model

N/A - `Validation` is a static class with no instance fields. The following private constants
are used internally: `JsonContentType` (`"application/json"`),
`MockProjectKey` (`"SonarMarkMockProject"`), and
`MockServerUrl` (`"https://mock.sonarqube.example"`).

#### Key Methods

**Run**: Entry point; runs all validation tests and optionally writes a results file.

- *Parameters*: `Context context` — provides output routing and `ResultsFile` path.
- *Returns*: `void`.
- *Preconditions*: `context` must not be null; throws `ArgumentNullException` otherwise.
- *Postconditions*: All four test scenarios have been executed; results are printed; the
  results file is written if `context.ResultsFile` is non-null.

Prints a validation header via `PrintValidationHeader` (which uses `context.Depth` for the
heading level and displays SonarMark version, machine name, OS, .NET runtime, and timestamp),
creates a `TestResults` collection, runs the four test methods in sequence, prints pass/fail
totals, and calls `WriteResultsFile` if a results path is set.

**RunValidationTest**: Shared test runner that wraps a single test scenario.

- *Parameters*: `Context context`; `TestResults testResults`; `Func<string?, SonarQubeClient>
  mockFactory`; `string testName`; `string? reportFileName`; `Func<string, string?, string?>
  validator`.
- *Returns*: `void`.
- *Preconditions*: All parameters must be non-null.
- *Postconditions*: A `TestResult` record has been appended to `testResults`.

Creates a `TemporaryDirectory`, builds CLI arguments (`--silent`, `--log`, `--server`,
`--project-key`, and optionally `--report`), creates a `Context` with the mock factory,
calls `Program.Run`, reads log and report files, invokes the `validator` function, and
records `Passed` or `Failed`. Catches any unexpected exception via `HandleTestException`.

**WriteResultsFile**: Serializes test results to a TRX or JUnit XML file.

- *Parameters*: `Context context`; `TestResults testResults`.
- *Returns*: `void`.
- *Preconditions*: `context.ResultsFile` is non-null.
- *Postconditions*: The results file has been written, or an error has been reported via
  `context.WriteError`.

Inspects the file extension of `context.ResultsFile`: `.trx` triggers TRX serialization,
`.xml` triggers JUnit XML serialization, any other extension writes an error via
`context.WriteError` without writing a file. File I/O failures are caught and reported via
`context.WriteError`.

#### Error Handling

`Run` guards against a null `context` with `ArgumentNullException.ThrowIfNull`. Individual
test failures inside `RunValidationTest` are caught by a broad `catch (Exception)` block in
`HandleTestException`; the exception message is written via `context.WriteError` and the
remaining tests continue. `WriteResultsFile` catches I/O failures and unsupported-extension
errors via `context.WriteError` rather than propagating, so a results-file failure cannot
mask the pass/fail summary already printed to the console.

#### Dependencies

- **Context** — provides output routing and all configuration properties.
- **Program** — called by `RunValidationTest` to exercise the full analysis pipeline.
- **SonarQubeClient** — instantiated with a mock `HttpClient` for test isolation.
- **DemaConsulting.TestResults** — provides `TestResults`, `TestResult`, and `TestOutcome`
  types for collecting test results.
- **DemaConsulting.TestResults.IO** — provides TRX and JUnit XML serialization.

#### Callers

- **Program.Run** — calls `Validation.Run(context)` when `context.Validate` is `true`.
