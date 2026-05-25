### SonarQubeClient

#### Verification Approach

`SonarQubeClient` is verified by unit tests in
`test/DemaConsulting.SonarMark.Tests/SonarIntegration/SonarQubeClientTests.cs`. A
`MockHttpMessageHandler` is injected into the `HttpClient` to intercept all HTTP requests and return
canned JSON responses. This eliminates any network dependency while exercising the complete JSON parsing,
pagination, error-handling, and authentication logic within `SonarQubeClient` itself. Tests that verify
authentication inspect the `Authorization` header on the outgoing `HttpRequestMessage`.

#### Test Environment

N/A - standard test environment.

#### Acceptance Criteria

- All unit tests in `SonarQubeClientTests` pass with zero failures.
- Non-2xx HTTP responses throw `InvalidOperationException` containing the HTTP status code.
- Issue and hot-spot pagination accumulates results across all pages into a single list.
- A provided token produces a Base64-encoded Basic Authorization header; no token produces no
  Authorization header.

#### Test Scenarios

**ConstructorWithAuthTokenCreatesInstance**: `SonarQubeClient` constructed with a non-null token
creates a valid instance, confirming the constructor handles the authenticated code path correctly.
This scenario is tested by `SonarQubeClient_Constructor_WithAuthToken_CreatesInstance`.

**ConstructorWithoutAuthTokenCreatesInstance**: `SonarQubeClient` constructed without a token creates a
valid instance, confirming the constructor handles the unauthenticated code path correctly.
This scenario is tested by `SonarQubeClient_Constructor_WithoutAuthToken_CreatesInstance`.

**ConstructorWithTokenSetsAuthorizationHeader**: When constructed with a token, the `HttpClient`
DefaultRequestHeaders contain a Basic Authorization header with the Base64-encoded token, confirming
that the PAT authentication mechanism is applied correctly.
This scenario is tested by `SonarQubeClient_Constructor_WithToken_SetsAuthorizationHeader`.

**ConstructorWithoutTokenHasNoAuthorizationHeader**: When constructed without a token, no Authorization
header is present on the `HttpClient`, confirming that unauthenticated requests do not carry credentials.
This scenario is tested by `SonarQubeClient_Constructor_WithoutToken_NoAuthorizationHeader`.

**Http401ErrorThrowsInvalidOperationExceptionWithStatusCode**: When the mock handler returns HTTP 401,
`GetQualityResultByBranchAsync` throws `InvalidOperationException` containing the status code,
confirming that authentication failures are surfaced as typed exceptions.
This scenario is tested by
`SonarQubeClient_GetQualityResultByBranchAsync_Http401Error_ThrowsInvalidOperationExceptionWithStatusCode`.

**Http500ErrorThrowsInvalidOperationExceptionWithStatusCode**: When the mock handler returns HTTP 500,
`GetQualityResultByBranchAsync` throws `InvalidOperationException` containing the status code,
confirming that server-side errors are surfaced as typed exceptions.
This scenario is tested by
`SonarQubeClient_GetQualityResultByBranchAsync_Http500Error_ThrowsInvalidOperationExceptionWithStatusCode`.

**IssuesPaginatedAcrossTwoPagesAccumulatesAllIssues**: When the mock `/api/issues/search` endpoint
returns results across two pages, the client accumulates all items into a single list in the returned
`SonarQualityResult`, confirming that the pagination loop works correctly.
This scenario is tested by
`SonarQubeClient_GetQualityResultByBranchAsync_IssuesPaginatedAcrossTwoPages_AccumulatesAllIssues`.

**HotSpotsPaginatedAcrossTwoPagesAccumulatesAllHotSpots**: When the mock `/api/hotspots/search` endpoint
returns results across two pages, the client accumulates all items into a single list in the returned
`SonarQualityResult`, confirming that hot-spot pagination works correctly.
This scenario is tested by
`SonarQubeClient_GetQualityResultByBranchAsync_HotSpotsPaginatedAcrossTwoPages_AccumulatesAllHotSpots`.

**ReturnsQualityGateStatus**: `GetQualityResultByBranchAsync` with a fully configured mock handler
returns a `SonarQualityResult` whose quality gate status, project name, and metric conditions match the
canned API responses, confirming the complete happy-path flow.
This scenario is tested by `SonarQubeClient_GetQualityResultByBranchAsync_ReturnsQualityGateStatus`.

**NullServerUrlThrowsArgumentNullException**: Passing `null` as the server URL throws
`ArgumentNullException`, confirming that null inputs are rejected at the method boundary.
This scenario is tested by
`SonarQubeClient_GetQualityResultByBranchAsync_NullServerUrl_ThrowsArgumentNullException`.

**WhitespaceServerUrlThrowsArgumentException**: Passing a whitespace-only string as the server URL
throws `ArgumentException`, confirming that empty server URLs are rejected before any HTTP call is made.
This scenario is tested by
`SonarQubeClient_GetQualityResultByBranchAsync_WhitespaceServerUrl_ThrowsArgumentException`.

**NullProjectKeyThrowsArgumentNullException**: Passing `null` as the project key throws
`ArgumentNullException`, confirming that null project keys are rejected at the method boundary.
This scenario is tested by
`SonarQubeClient_GetQualityResultByBranchAsync_NullProjectKey_ThrowsArgumentNullException`.

**WhitespaceProjectKeyThrowsArgumentException**: Passing a whitespace-only string as the project key
throws `ArgumentException`, confirming that empty project keys are rejected before any HTTP call is made.
This scenario is tested by
`SonarQubeClient_GetQualityResultByBranchAsync_WhitespaceProjectKey_ThrowsArgumentException`.
