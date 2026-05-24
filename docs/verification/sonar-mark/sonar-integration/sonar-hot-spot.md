### SonarHotSpot

#### Verification Approach

`SonarHotSpot` is verified by unit tests in
`test/DemaConsulting.SonarMark.Tests/SonarIntegration/SonarHotSpotTests.cs`. As a positional record
type with no external dependencies, all tests exercise the constructor directly, passing specific field
values and asserting the resulting property values. No mocking is required.

#### Test Environment

N/A - standard test environment.

#### Acceptance Criteria

- All unit tests in `SonarHotSpotTests` pass with zero failures.
- All six fields (`Key`, `Component`, `Line`, `Message`, `SecurityCategory`,
  `VulnerabilityProbability`) are accessible as read-only properties after construction.
- `Line` accepts `null` to represent a file-level hot-spot with no specific line association.
- `VulnerabilityProbability` accepts all expected probability values (`HIGH`, `MEDIUM`, `LOW`).

#### Test Scenarios

**AllPropertiesCreatesInstance**: A `SonarHotSpot` constructed with all six fields stores each value in
the corresponding property, confirming that the positional record carries all required fields correctly.
This scenario is tested by `SonarHotSpot_Constructor_AllProperties_CreatesInstance`.

**NullLineCreatesInstance**: A `SonarHotSpot` constructed with `null` for the `Line` parameter stores
`null` in `Line`, confirming that file-level hot-spots with no line association are represented
correctly.
This scenario is tested by `SonarHotSpot_Constructor_NullLine_CreatesInstance`.

**LowProbabilityCreatesInstance**: A `SonarHotSpot` constructed with `VulnerabilityProbability = "LOW"`
stores the value correctly, confirming that the low-priority code path is handled the same way as higher
priority levels.
This scenario is tested by `SonarHotSpot_Constructor_LowProbability_CreatesInstance`.
