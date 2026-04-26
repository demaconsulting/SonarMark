# SonarQubeClient

## Overview

`SonarQubeClient` is the HTTP client responsible for communicating with the
SonarQube/SonarCloud REST API. It fetches quality gate status, conditions, issues,
and security hot-spots for a given project and branch, and assembles them into a
`SonarQualityResult` instance.

## Design Decisions

### IDisposable Wrapping HttpClient

`SonarQubeClient` wraps an `HttpClient` and implements `IDisposable`. An
`_ownsHttpClient` flag controls whether `Dispose` releases the underlying client,
enabling injection of a shared or mock client in tests without premature disposal.

### Token-Based Authentication

When an authentication token is provided, it is Base64-encoded and sent as an
HTTP Basic Authorization header with an empty password (the standard SonarQube
PAT mechanism). This approach avoids storing credentials in any intermediate
format.

### Async API

All network methods are `async` and accept a `CancellationToken`. The primary
public method `GetQualityResultByBranchAsync` composes several internal async
calls in a fixed sequence:

1. **Project name retrieval** — calls `/api/components/show?component={projectKey}`
   to obtain the human-readable project name. The response JSON must contain a
   `component.name` string; if missing, the method falls back to the raw project
   key. If the HTTP request fails (non-2xx), an `InvalidOperationException` is
   thrown. If the JSON response is malformed, a `JsonException` may be thrown
   (see Error Handling below).

2. **Quality gate status** — calls
   `/api/qualitygates/project_status?projectKey={projectKey}` (with optional
   `&branch=` parameter) to retrieve the overall gate status and its conditions.

3. **Metric name resolution** — calls `/api/metrics/search` to build a dictionary
   mapping internal metric keys (e.g., `new_coverage`) to human-readable display
   names (e.g., `Coverage on New Code`). This dictionary is passed to
   `SonarQualityResult` so the report can render friendly metric labels.

4. **Issues** — paginates through `/api/issues/search` until all pages are
   consumed.

5. **Hot-spots** — paginates through `/api/hotspots/search` until all pages are
   consumed.

Results from all five calls are assembled into a single `SonarQualityResult`
instance and returned to the caller.

### Pagination

Issues and hot-spots are fetched with server-side pagination. The client loops
until all pages have been retrieved, accumulating results into a single list.
This ensures completeness regardless of how many items the server returns per page.

### Error Handling

HTTP errors (non-2xx responses) are surfaced as `InvalidOperationException` with
a message that includes the HTTP status code, so callers can distinguish API
errors from network errors and report them appropriately.

`JsonDocument.Parse()` is called on every API response. If the server returns a
syntactically invalid JSON body, `JsonDocument.Parse` throws a `JsonException`.
This exception is not caught by `SonarQubeClient` and propagates to the caller,
indicating a malformed or unexpected server response.

## Satisfies Requirements

- `SonarMark-Server-QualityGate` — fetches quality gate status and conditions from the API
- `SonarMark-Server-Issues` — fetches issues with pagination from the API
- `SonarMark-Server-HotSpots` — fetches security hot-spots with pagination from the API
