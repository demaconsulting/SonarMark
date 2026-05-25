### SonarIssue

#### Purpose

`SonarIssue` is an immutable positional record that represents a single code quality issue
returned by the SonarQube/SonarCloud API. It holds the fields needed to render a
compiler-style issue entry in the markdown report.

#### Data Model

**Key**: `string` — unique identifier for the issue as returned by the API.

**Rule**: `string` — rule key that triggered the issue (e.g., `csharpsquid:S1135`).

**Severity**: `string` — severity level as returned by the API
(e.g., `BLOCKER`, `CRITICAL`, `MAJOR`, `MINOR`, `INFO`).

**Component**: `string` — fully-qualified component path including the project key prefix
(e.g., `my-project:src/Foo.cs`).

**Line**: `int?` — source line number; `null` when the issue has no line association.

**Message**: `string` — human-readable description of the issue.

**Type**: `string` — issue type as returned by the API (e.g., `BUG`, `VULNERABILITY`,
`CODE_SMELL`).

#### Key Methods

N/A - `SonarIssue` is a positional record with no defined methods beyond the
compiler-generated constructor, `Equals`, `GetHashCode`, and `ToString`.

#### Error Handling

N/A - `SonarIssue` is a passive data record. All field values are provided by the caller
(`SonarQubeClient.ParseIssue`) and no validation is performed on construction.

#### Dependencies

N/A - `SonarIssue` has no dependencies beyond the .NET runtime.

#### Callers

- **SonarQubeClient** — creates `SonarIssue` records in `ParseIssue` from the API response.
- **SonarQualityResult** — holds and renders the `IReadOnlyList<SonarIssue>` in
  `AppendIssuesSection`.
