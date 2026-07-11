### SonarQualityResult

#### Verification Approach

`SonarQualityResult` and its companion `SonarQualityCondition` are verified by unit tests in
`test/DemaConsulting.SonarMark.Tests/ReportGeneration/SonarQualityResultTests.cs`. Tests construct
instances with specific combinations of quality gate status, conditions, issues, and hot-spots, then
call `ToMarkdown` and assert on the rendered string content. No external dependencies are involved.
Boundary tests verify that `ToMarkdown` throws `ArgumentOutOfRangeException` for depth values outside
the valid range of 1 to 6.

#### Test Environment

N/A - standard test environment.

#### Acceptance Criteria

- All unit tests in `SonarQualityResultTests` pass with zero failures.
- `ToMarkdown` produces headings at the correct level for depths 1 through 6.
- `ToMarkdown` throws `ArgumentOutOfRangeException` for depth less than 1 or greater than 6.
- Conditions section is absent when no conditions are present.
- Null threshold and actual values are omitted from the rendered condition row.
- Friendly metric names are used when a metric name dictionary is provided.
- Issues are rendered in compiler-style format with blank lines between items.
- Hot-spots are rendered in compiler-style format with blank lines between items.
- Singular/plural count text is correct for exactly one issue or one hot-spot.

#### Test Scenarios

**Depth1ProducesCorrectOutput**: `ToMarkdown(1)` produces a markdown report with a level-1 heading and
all mandatory sections, confirming that the default depth renders correctly.
This scenario is tested by `SonarQualityResult_ToMarkdown_Depth1_ProducesCorrectOutput`.

**Depth3UsesCorrectHeadingLevels**: `ToMarkdown(3)` produces headings at level 3 and its sub-sections
at level 4, confirming that configurable depth shifts all headings consistently.
This scenario is tested by `SonarQualityResult_ToMarkdown_Depth3_UsesCorrectHeadingLevels`.

**Depth6ProducesCorrectOutput**: `ToMarkdown(6)` produces a valid report with the maximum supported
heading depth, confirming that the upper boundary value is accepted and renders correctly.
This scenario is tested by `SonarQualityResult_ToMarkdown_Depth6_ProducesCorrectOutput`.

**NoConditionsExcludesConditionsSection**: When the conditions list is empty, `ToMarkdown` omits the
conditions section from the report, confirming that an empty conditions list does not produce a
spurious table header in the output.
This scenario is tested by `SonarQualityResult_ToMarkdown_NoConditions_ExcludesConditionsSection`.

**NullThresholdAndActualExcludesNullValues**: When a `SonarQualityCondition` has `null`
`ErrorThreshold` and `ActualValue`, those columns are omitted from the rendered row, confirming that
optional condition fields are handled without null-reference errors or empty column output.
This scenario is tested by `SonarQualityResult_ToMarkdown_NullThresholdAndActual_RendersEmptyCells`.

**DepthLessThan1ThrowsArgumentOutOfRangeException**: `ToMarkdown(0)` throws
`ArgumentOutOfRangeException`, confirming that the lower boundary is enforced.
This scenario is tested by `SonarQualityResult_ToMarkdown_DepthLessThan1_ThrowsArgumentOutOfRangeException`.

**DepthGreaterThan6ThrowsArgumentOutOfRangeException**: `ToMarkdown(7)` throws
`ArgumentOutOfRangeException`, confirming that the upper boundary is enforced.
This scenario is tested by `SonarQualityResult_ToMarkdown_DepthGreaterThan6_ThrowsArgumentOutOfRangeException`.

**WarnStatusProducesCorrectOutput**: A result with quality gate status `WARN` renders the status label
correctly, confirming that non-OK and non-ERROR status values are handled by the rendering logic.
This scenario is tested by `SonarQualityResult_ToMarkdown_WarnStatus_ProducesCorrectOutput`.

**WithFriendlyNamesUsesFriendlyNames**: When a metric name dictionary maps internal keys to display
names, `ToMarkdown` substitutes the display names in the conditions table, confirming that friendly
metric name resolution is applied correctly.
This scenario is tested by `SonarQualityResult_ToMarkdown_WithFriendlyNames_UsesFriendlyNames`.

**WithIssuesProducesCompilerStyleOutput**: A result containing issues renders each issue in
compiler-style format (`component:line: severity: message`), confirming that the issue rendering
path produces the expected structured output.
This scenario is tested by `SonarQualityResult_ToMarkdown_WithIssues_ProducesCompilerStyleOutput`.

**WithHotSpotsProducesCompilerStyleOutput**: A result containing hot-spots renders each hot-spot in
compiler-style format, confirming that the hot-spot rendering path produces the expected output.
This scenario is tested by `SonarQualityResult_ToMarkdown_WithHotSpots_ProducesCompilerStyleOutput`.

**WithSingularCountsShowsCorrectText**: When there is exactly one issue and one hot-spot, the count
summary uses singular text ("1 issue", "1 hot-spot") rather than plural, confirming that the
singular/plural branch is handled correctly.
This scenario is tested by `SonarQualityResult_ToMarkdown_WithSingularCounts_ShowsCorrectText`.

**WithMultipleIssuesIncludesBlankLinesBetweenItems**: When multiple issues are rendered, a blank line
separates each item in the output, confirming that the inter-item spacing rule is applied correctly.
This scenario is tested by `SonarQualityResult_ToMarkdown_WithMultipleIssues_IncludesLineBreaksBetweenItems`.

**WithMultipleHotSpotsIncludesBlankLinesBetweenItems**: When multiple hot-spots are rendered, a blank
line separates each item in the output, confirming that the inter-item spacing rule is applied to
hot-spots as well as issues.
This scenario is tested by
`SonarQualityResult_ToMarkdown_WithMultipleHotSpots_IncludesLineBreaksBetweenItems`.

**ComponentWithoutProjectKeyPrefixPassesThroughUnchanged**: When a component path does not begin with
the project key prefix, it is included unchanged in the rendered output, confirming that the prefix
stripping logic is guarded and does not corrupt paths that lack the expected prefix.
This scenario is tested by
`SonarQualityResult_ToMarkdown_ComponentWithoutProjectKeyPrefix_PassesThroughUnchanged`.

**SonarQualityConditionAllPropertiesCreatesInstance**: A `SonarQualityCondition` constructed with all
five positional parameters stores each value in the corresponding property, confirming that the companion
record carries all required quality gate condition fields.
This scenario is tested by `SonarQualityCondition_Constructor_AllProperties_CreatesInstance`.
