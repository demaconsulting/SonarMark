## SonarIntegration

### Overview

The SonarIntegration subsystem is responsible for communicating with SonarQube/SonarCloud
servers via their REST API. It fetches quality gate status, issues, and security hot-spots
for a given project and branch, and assembles the results into a `SonarQualityResult`
instance. The subsystem contains three units: `SonarQubeClient` (the HTTP client),
`SonarHotSpot` (an immutable data record for a single security hot-spot), and `SonarIssue`
(an immutable data record for a single code quality issue).

### Interfaces

**SonarQubeClient.GetQualityResultByBranchAsync**: Primary analysis retrieval method.

- *Type*: In-process .NET public API.
- *Role*: Provider — called by `Program.ProcessSonarAnalysis`.
- *Contract*: Accepts `serverUrl`, `projectKey`, optional `branch`, and an optional
  `CancellationToken`. Performs five sequential HTTP API calls and returns a
  `SonarQualityResult` containing the gate status, conditions, metric name dictionary,
  issues list, and hot-spots list.
- *Constraints*: `serverUrl` and `projectKey` must not be null or whitespace; throws
  `ArgumentException` otherwise. Non-2xx HTTP responses raise `InvalidOperationException`.
  Malformed JSON raises `JsonException`. Both propagate to the caller.

**SonarQubeClient constructor**: Creates a client with optional token-based authentication.

- *Type*: In-process .NET public API.
- *Role*: Provider.
- *Contract*: `new SonarQubeClient(string? authToken)` creates an `HttpClient` with an HTTP
  Basic Authorization header when `authToken` is non-null. The internal constructor
  `SonarQubeClient(HttpClient, bool ownsHttpClient)` is used by tests to inject a mock.
- *Constraints*: Implements `IDisposable`; disposes the `HttpClient` only when
  `_ownsHttpClient` is `true`.

### Design

1. `Program.ProcessSonarAnalysis` creates a `SonarQubeClient` (or obtains one from
   `context.HttpClientFactory`) and calls `GetQualityResultByBranchAsync`.
2. `GetQualityResultByBranchAsync` makes five sequential async HTTP calls: project name from
   `/api/components/show`, quality gate status from `/api/qualitygates/project_status`,
   metric names from `/api/metrics/search`, paginated issues from `/api/issues/search`, and
   paginated hot-spots from `/api/hotspots/search`.
3. Issues and hot-spots are parsed into `SonarIssue` and `SonarHotSpot` records respectively.
4. All results are assembled into a `SonarQualityResult` and returned to `Program`.
