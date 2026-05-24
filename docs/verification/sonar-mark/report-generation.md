## ReportGeneration

### Verification Approach

The ReportGeneration subsystem is verified by subsystem integration tests in
`test/DemaConsulting.SonarMark.Tests/ReportGeneration/ReportGenerationTests.cs`. These tests construct
`SonarQualityResult` instances with representative data (quality conditions, issues, hot-spots) and
call `ToMarkdown`, asserting that the rendered string contains the expected quality gate status,
condition labels, issue severity groups, and hot-spot priority information. No external dependencies
are involved.

### Test Environment

N/A - standard test environment.

### Acceptance Criteria

- All tests in `ReportGenerationTests` pass with zero failures.
- The markdown report includes quality gate status and all gate conditions with their actual values.
- Issues are categorized by type and severity in the rendered output.
- Hot-spots include priority and security category information in the rendered output.

### Test Scenarios

**QualityGateReportIncludesStatusAndConditions**: A `SonarQualityResult` with quality gate conditions
renders a markdown section that includes the overall gate status and each condition's metric, threshold,
actual value, and status, confirming that the ReportGeneration subsystem produces complete quality gate
output.
This scenario is tested by `ReportGeneration_QualityGateReport_IncludesStatusAndConditions`.

**IssuesReportCategorizesByTypeAndSeverity**: A `SonarQualityResult` containing issues of different
severities renders a markdown issues section that groups and labels items by type and severity,
confirming that the categorization logic within the subsystem produces readable, organized output.
This scenario is tested by `ReportGeneration_IssuesReport_CategorizesByTypeAndSeverity`.

**HotSpotsReportIncludesPriorityAndCategory**: A `SonarQualityResult` containing security hot-spots
renders a markdown hot-spots section that includes each hot-spot's vulnerability probability and
security category, confirming that security-specific rendering is correct.
This scenario is tested by `ReportGeneration_HotSpotsReport_IncludesPriorityAndCategory`.
