# Validation

## Overview

`Validation` provides the self-validation capability of SonarMark. When the tool
is invoked with `--validate`, `Validation.Run` executes a suite of internal tests
using a mocked HTTP client, verifying that the tool's core workflows function
correctly without starting a real HTTP server or requiring a live SonarQube/SonarCloud instance.

## Design Decisions

### Mock HTTP Client

Validation uses a mock `HttpClient` that intercepts all HTTP requests and returns
pre-defined JSON responses. This allows the full SonarQubeClient code path to be
exercised without any external network dependency.

### TestResults Integration

Results are collected into a `DemaConsulting.TestResults.TestResults` instance and
optionally written to a TRX or JUnit XML file via `DemaConsulting.TestResults.IO`.
This makes validation output compatible with standard CI test result consumers
and feeds into the requirements traceability pipeline.

### Static Class

`Validation` is a static class because it has no instance state; all required
context is passed explicitly as parameters. This avoids unnecessary object
allocation and makes the dependency on `Context` explicit.

### Per-Test Helper Methods

Each validation scenario (quality gate retrieval, issues retrieval, hot-spots
retrieval, markdown report generation) is isolated in its own helper method.
This makes failures easy to diagnose and allows individual scenarios to be
added or removed independently.

### TemporaryDirectory

`TemporaryDirectory` is a private nested class that implements `IDisposable`.
It creates a uniquely named temporary directory on construction and deletes it
(including all its contents) on disposal. Each validation test creates its own
`TemporaryDirectory` instance so that log files and report files written during
test execution are cleaned up automatically, preventing cross-test interference
and leaving no file-system artifacts after the run completes.

### Helper Methods

Three private helpers manage the test lifecycle:

- **`CreateTestResult`** — allocates a new `TestResult` object pre-populated with
  the test name, class name (`Validation`), and code base (`SonarMark`). Called
  before the test body runs to provide a result container into which outcomes are
  written.

- **`HandleTestException`** — invoked from the `catch` block inside
  `RunValidationTest` when an unexpected exception escapes the test body. It marks
  the result as `Failed`, stores the exception message, and writes the failure to
  the context output. The broad `catch (Exception)` is intentional here because
  the self-test must not crash the process regardless of what a scenario throws.

- **`FinalizeTestResult`** — called after every test (pass or fail) to record
  elapsed duration and append the result to the shared `TestResults` collection.
  Placing this call in the method that wraps the test body guarantees it runs even
  when `HandleTestException` has already been invoked.

## Error Handling

### Null-Context Precondition

`Validation.Run` guards against a null `context` argument with
`ArgumentNullException.ThrowIfNull(context)` at the top of the method. Because
`context` is used for all output and all subsequent calls depend on it, an early
fail-fast check is cleaner than a `NullReferenceException` arising deep in the
call chain.

### Unsupported-Format Error in WriteResultsFile

`WriteResultsFile` inspects the file extension of `context.ResultsFile` and
branches to the TRX or JUnit serializer. If the extension is neither `.trx` nor
`.xml`, it writes an error message via `context.WriteError` and returns without
writing any file. It does **not** throw, because an unsupported format is a
configuration mistake that the caller has already communicated to the user via
the error message — aborting the whole program would be excessive.

### I/O Failure Catch Block

`WriteResultsFile` wraps `File.WriteAllText` in a `try/catch (Exception)` block.
File I/O failures (permissions, disk full, invalid path) are caught, and the
exception message is written to `context.WriteError`. This prevents an I/O
failure from propagating out of the validation run, which could mask the
test-result summary that was already printed to the console.

## Satisfies Requirements

- `SonarMark-Validation-Run` — implements the self-validation mode invoked by `--validate`
- `SonarMark-Validation-Results` — writes results to the file specified by `--results`
- `SonarMark-Validation-TrxFormat` — writes TRX format when the results file has a `.trx` extension
- `SonarMark-Validation-JUnitFormat` — writes JUnit XML when the results file has a `.xml` extension
- `SonarMark-Validation-Depth` — `PrintValidationHeader` uses `context.Depth` to set heading level
