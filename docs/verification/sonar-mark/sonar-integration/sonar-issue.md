### SonarIssue

#### Verification Approach

`SonarIssue` is verified by unit tests in
`test/DemaConsulting.SonarMark.Tests/SonarIntegration/SonarIssueTests.cs`. As a positional record type
with no external dependencies, all tests exercise the constructor directly, passing specific field values
and asserting the resulting property values. No mocking is required.

#### Test Environment

N/A - standard test environment.

#### Acceptance Criteria

- All unit tests in `SonarIssueTests` pass with zero failures.
- All fields (`Key`, `Rule`, `Severity`, `Component`, `Line`, `Message`, `Type`) are accessible as
  read-only properties after construction.
- `Line` accepts `null` to represent a file-level issue with no specific line association.
- `Severity` accepts all expected severity levels (`BLOCKER`, `CRITICAL`, `MAJOR`, `MINOR`, `INFO`).

#### Test Scenarios

**AllPropertiesCreatesInstance**: A `SonarIssue` constructed with all fields stores each value in the
corresponding property, confirming that the positional record carries all required fields correctly.
This scenario is tested by `SonarIssue_Constructor_AllProperties_CreatesInstance`.

**NullLineCreatesInstance**: A `SonarIssue` constructed with `null` for the `Line` parameter stores
`null` in `Line`, confirming that file-level issues with no line association are represented correctly.
This scenario is tested by `SonarIssue_Constructor_NullLine_CreatesInstance`.

**BlockerSeverityCreatesInstance**: A `SonarIssue` constructed with `Severity = "BLOCKER"` stores the
value correctly, confirming that the highest severity level is represented without error.
This scenario is tested by `SonarIssue_Constructor_BlockerSeverity_CreatesInstance`.

**CriticalSeverityCreatesInstance**: A `SonarIssue` constructed with `Severity = "CRITICAL"` stores the
value correctly, confirming that critical-severity issues are captured accurately.
This scenario is tested by `SonarIssue_Constructor_CriticalSeverity_CreatesInstance`.

**InfoSeverityCreatesInstance**: A `SonarIssue` constructed with `Severity = "INFO"` stores the value
correctly, confirming that the lowest severity level is represented without error.
This scenario is tested by `SonarIssue_Constructor_InfoSeverity_CreatesInstance`.
