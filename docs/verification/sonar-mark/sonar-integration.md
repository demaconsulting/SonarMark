## SonarIntegration

### Verification Approach

The SonarIntegration subsystem is verified by subsystem integration tests in
`test/DemaConsulting.SonarMark.Tests/SonarIntegration/SonarIntegrationTests.cs`. These tests exercise
the complete path from `SonarQubeClient.GetQualityResultByBranchAsync` through the JSON parsing and
data assembly logic, using a `MockHttpMessageHandler` that intercepts all HTTP requests and returns
pre-defined JSON responses. No real network calls are made. The tests assert on the returned
`SonarQualityResult` object to confirm that quality gate status, issues, and hot-spots are correctly
fetched, parsed, and assembled.

### Test Environment

N/A - standard test environment.

### Acceptance Criteria

- All tests in `SonarIntegrationTests` pass with zero failures.
- `GetQualityResultByBranchAsync` returns a `SonarQualityResult` containing the quality gate status
  from the mocked `/api/qualitygates/project_status` response.
- `GetQualityResultByBranchAsync` returns a `SonarQualityResult` containing all issues from the mocked
  `/api/issues/search` response.
- `GetQualityResultByBranchAsync` returns a `SonarQualityResult` containing all hot-spots from the
  mocked `/api/hotspots/search` response.

### Test Scenarios

**FetchQualityResultReturnsQualityGateStatus**: `SonarQubeClient.GetQualityResultByBranchAsync` with a
mock HTTP handler returns a `SonarQualityResult` whose quality gate status matches the status value in
the canned API response, confirming that the full subsystem data path from HTTP response to result object
is wired correctly.
This scenario is tested by `SonarIntegration_FetchQualityResult_ReturnsQualityGateStatus`.

**FetchQualityResultReturnsIssues**: `SonarQubeClient.GetQualityResultByBranchAsync` with a mock HTTP
handler returns a `SonarQualityResult` containing the issues defined in the canned API response,
confirming that issue parsing and assembly work correctly within the subsystem.
This scenario is tested by `SonarIntegration_FetchQualityResult_ReturnsIssues`.

**FetchQualityResultReturnsHotSpots**: `SonarQubeClient.GetQualityResultByBranchAsync` with a mock HTTP
handler returns a `SonarQualityResult` containing the hot-spots defined in the canned API response,
confirming that security hot-spot parsing and assembly work correctly within the subsystem.
This scenario is tested by `SonarIntegration_FetchQualityResult_ReturnsHotSpots`.
