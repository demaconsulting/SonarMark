## DemaConsulting.TestResults

### Purpose

`DemaConsulting.TestResults` is a NuGet package produced by DEMA Consulting. It provides a
lightweight, format-agnostic model for collecting and serializing test results inside a running
process. SonarMark uses it in the SelfTest subsystem to record the outcomes of its four
self-validation scenarios and to write those outcomes to TRX or JUnit XML files when the user
supplies the `--results` flag alongside `--validate`.

The package was chosen because it produces the TRX and JUnit XML formats that standard CI/CD
result consumers (Azure DevOps, GitHub Actions, Jenkins) expect, without introducing a dependency
on a full test framework.

### Classification

Although `DemaConsulting.TestResults` is produced by DEMA Consulting — the same organization that
produces SonarMark — it is an independently developed and separately released package with its own
requirements, versioning, and release lifecycle. It is not part of the SonarMark product and its
internal design requirements do not drive SonarMark requirements. It is therefore classified as an
OTS software item.

### Features Used

- **`TestResults`** — mutable collection that accumulates `TestResult` records during a validation
  run. Passed to `WriteResultsFile` after all four self-validation scenarios have executed.
- **`TestResult`** — immutable record holding a test name, `TestOutcome`, and optional failure
  message. One record is appended per scenario inside `RunValidationTest`.
- **`TestOutcome`** — enumeration of `Passed` and `Failed` values used to mark each test record.
- **`TestResultsIO`** — static serializer class (namespace `DemaConsulting.TestResults.IO`)
  providing:
  - `WriteTrx(TestResults, string)` — serializes to TRX format for `.trx` output files.
  - `WriteJUnit(TestResults, string)` — serializes to JUnit XML format for `.xml` output files.

### Integration Pattern

`DemaConsulting.TestResults` is referenced as a `PackageReference` in the production project
`src/DemaConsulting.SonarMark/DemaConsulting.SonarMark.csproj`. No initialization or
configuration is required; all types are used directly.

Inside `Validation.Run`, a `TestResults` instance is created at the start of the validation
pass and passed into each call to `RunValidationTest`. After all four scenarios complete,
`WriteResultsFile` inspects the file extension of `context.ResultsFile` and delegates to
`TestResultsIO.WriteTrx` (`.trx`) or `TestResultsIO.WriteJUnit` (`.xml`) accordingly. Any
serialization failure is caught and reported via `context.WriteError`; it does not propagate
to the caller.

There are no global initialization, thread-affinity, or disposal requirements.
