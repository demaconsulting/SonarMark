### SonarQubeClient

![SonarIntegration Structure](SonarIntegrationView.svg)

#### Purpose

`SonarQubeClient` is the HTTP client responsible for communicating with the
SonarQube/SonarCloud REST API. It fetches quality gate status, conditions, metric names,
issues, and security hot-spots for a given project and branch, and assembles them into a
`SonarQualityResult` instance.

#### Data Model

**_httpClient**: `HttpClient` — the underlying HTTP client used for all API requests.

**_ownsHttpClient**: `bool` — when `true`, `Dispose` releases `_httpClient`; when `false`
(test injection path), the caller retains ownership and `Dispose` is a no-op for the client.

**Thread Safety**: `SonarQubeClient` is safe for concurrent calls to
`GetQualityResultByBranchAsync` on the same instance. `HttpClient` supports concurrent HTTP
requests, and all shared fields are read-only after construction. `Dispose` must not be called
while any async operation is outstanding.

#### Key Methods

**GetQualityResultByBranchAsync**: Orchestrates all API calls and returns the combined result.

- *Parameters*: `string serverUrl` — base URL of the SonarQube/SonarCloud server;
  `string projectKey` — the project's key identifier; `string? branch` — optional branch
  name; `CancellationToken cancellationToken` — optional cancellation.
- *Returns*: `Task<SonarQualityResult>` — assembled quality result.
- *Preconditions*: `serverUrl` and `projectKey` must not be null or whitespace; throws
  `ArgumentNullException` for null, `ArgumentException` for empty or whitespace.
- *Postconditions*: Returns a fully populated `SonarQualityResult` on success.

Calls `GetProjectNameByKeyAsync`, `GetQualityGateStatusByBranchAsync`,
`GetMetricNamesByServerAsync`, `GetIssuesAsync`, and `GetHotSpotsAsync` in sequence.

**GetProjectNameByKeyAsync**: Fetches the human-readable project name.

- *Parameters*: `string serverUrl`, `string projectKey`, `CancellationToken`.
- *Returns*: `Task<string>` — project name, or `projectKey` as fallback when the name is
  null or empty.
- *Preconditions*: None beyond those on `GetQualityResultByBranchAsync`.
- *Postconditions*: Returns a non-null string.

Calls `/api/components/show?component={projectKey}`. Throws `InvalidOperationException` on
non-2xx response or when the `component.name` property is absent in the JSON.

**GetQualityGateStatusByBranchAsync**: Fetches the quality gate status and conditions.

- *Parameters*: `string serverUrl`, `string projectKey`, `string? branch`,
  `CancellationToken`.
- *Returns*: `Task<(string QualityGateStatus, List<SonarQualityCondition> Conditions)>`.
- *Preconditions*: None.
- *Postconditions*: Returns gate status string (e.g., `OK`, `WARN`, `ERROR`) and conditions
  list; defaults status to `NONE` when the `status` JSON element is present but has a null
  value; throws `InvalidOperationException` when the `status` property is absent.

Calls `/api/qualitygates/project_status?projectKey={projectKey}` with an optional
`&branch={branch}` parameter.

**GetMetricNamesByServerAsync**: Fetches the metric name dictionary.

- *Parameters*: `string serverUrl`, `CancellationToken`.
- *Returns*: `Task<IReadOnlyDictionary<string, string>>` — maps metric key to display name.
- *Preconditions*: None.
- *Postconditions*: Returns a non-null dictionary; may be empty if the API returns no metrics.

Calls `/api/metrics/search`.

**GetIssuesAsync**: Fetches all open and confirmed issues for the project using pagination.

- *Parameters*: `string serverUrl`, `string projectKey`, `string? branch`, `CancellationToken`.
- *Returns*: `Task<List<SonarIssue>>` — all issues accumulated across all pages.
- *Preconditions*: None.
- *Postconditions*: Returns all matching issues; the list may be empty when no issues are present.

Calls `/api/issues/search?componentKeys={projectKey}&issueStatuses=OPEN,CONFIRMED&ps=100` with an
optional `&branch={branch}` parameter. Delegates to `FetchPaginatedAsync` for pagination.

**GetHotSpotsAsync**: Fetches all security hot-spots for the project using pagination.

- *Parameters*: `string serverUrl`, `string projectKey`, `string? branch`, `CancellationToken`.
- *Returns*: `Task<List<SonarHotSpot>>` — all hot-spots accumulated across all pages.
- *Preconditions*: None.
- *Postconditions*: Returns all matching hot-spots; the list may be empty when no hot-spots are
  present.

Calls `/api/hotspots/search?projectKey={projectKey}&ps=100` with an optional
`&branch={branch}` parameter. Delegates to `FetchPaginatedAsync` for pagination.

**CreateHttpClient**: Factory method that creates and configures the underlying `HttpClient`.

- *Parameters*: `string? authToken` — optional Personal Access Token (PAT).
- *Returns*: `HttpClient` — configured HTTP client, with or without an `Authorization` header.
- *Visibility*: `internal static` — accessible to tests without exposing public API surface.

When `authToken` is non-null and non-whitespace, encodes `"{authToken}:"` as Base64 (ASCII) and
sets the `Authorization: Basic {encoded}` request header on the returned client. When `authToken`
is null or whitespace, no authorization header is set. Called directly by tests to verify that
the correct authentication header is constructed.

**FetchPaginatedAsync**: Generic pagination helper used for issues and hot-spots.

- *Parameters*: `string baseUrl` — URL with all query parameters except the page number;
  `string itemsPropertyName` — JSON array property name on each page response;
  `Func<JsonElement, List<T>> parseItems` — page parser delegate; `CancellationToken`.
- *Returns*: `Task<List<T>>` — all items accumulated across pages.
- *Preconditions*: None.
- *Postconditions*: All pages have been fetched; items are in server-returned order.

Appends `&p={pageNumber}` on each iteration. Reads `paging.pageIndex`, `paging.pageSize`,
and `paging.total` from each response to determine whether more pages remain. Stops when
`total <= pageIndex * pageSize` or when the `paging` property is absent from the response.

#### Error Handling

Non-2xx HTTP responses raise `InvalidOperationException` with the HTTP status code and reason
phrase in the message. Missing required JSON properties raise `InvalidOperationException` with
a descriptive message. `JsonDocument.Parse` errors raise `JsonException`, which propagates to
the caller uncaught. All exceptions propagate to `Program.ProcessSonarAnalysis`, which catches
`InvalidOperationException` and reports it via `context.WriteError`.

#### Dependencies

- **SonarQualityResult** — constructed and returned by `GetQualityResultByBranchAsync`.
- **SonarQualityCondition** — constructed and returned in the quality gate conditions flow
  within `GetQualityGateStatusByBranchAsync`; collected into the `Conditions` list of
  `SonarQualityResult`.
- **SonarIssue** — populated from paginated issues responses.
- **SonarHotSpot** — populated from paginated hot-spots responses.
- **System.Net.Http.HttpClient** — .NET runtime HTTP client.
- **System.Text.Json** — .NET runtime JSON parser.

#### Callers

- **Program.ProcessSonarAnalysis** — creates a `SonarQubeClient` and calls
  `GetQualityResultByBranchAsync`.
- **Validation.RunValidationTest** — injects a mock `SonarQubeClient` via
  `context.HttpClientFactory`.
