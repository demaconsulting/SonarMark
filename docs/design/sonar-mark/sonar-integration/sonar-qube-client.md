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
calls to retrieve conditions, issues, and hot-spots in a logical sequence.

### Pagination

Issues and hot-spots are fetched with server-side pagination. The client loops
until all pages have been retrieved, accumulating results into a single list.
This ensures completeness regardless of how many items the server returns per page.

### Error Handling

HTTP errors (non-2xx responses) are surfaced as `InvalidOperationException` with
a message that includes the HTTP status code, so callers can distinguish API
errors from network errors and report them appropriately.

## Satisfies Requirements

- `SonarMark-Server-QualityGate` — fetches quality gate status and conditions from the API
- `SonarMark-Server-Issues` — fetches issues with pagination from the API
- `SonarMark-Server-HotSpots` — fetches security hot-spots with pagination from the API
