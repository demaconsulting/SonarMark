### SonarHotSpot

#### Purpose

`SonarHotSpot` is an immutable positional record that represents a single security hot-spot
returned by the SonarQube/SonarCloud API. It holds the minimum set of fields needed to render
a useful entry in the markdown report.

#### Data Model

**Key**: `string` — unique identifier for the hot-spot as returned by the API.

**Component**: `string` — fully-qualified component path including the project key prefix
(e.g., `my-project:src/Foo.cs`).

**Line**: `int?` — source line number; `null` when the hot-spot has no line association.

**Message**: `string` — human-readable description of the hot-spot.

**SecurityCategory**: `string` — security category key as returned by the API
(e.g., `sql-injection`, `xss`).

**VulnerabilityProbability**: `string` — probability level returned by the API
(e.g., `HIGH`, `MEDIUM`, `LOW`).

#### Key Methods

N/A - `SonarHotSpot` is a positional record with no defined methods beyond the
compiler-generated constructor, `Equals`, `GetHashCode`, and `ToString`.

#### Error Handling

N/A - `SonarHotSpot` is a passive data record. All field values are provided by the caller
(`SonarQubeClient.ParseHotSpot`) and no validation is performed on construction.

#### Dependencies

N/A - `SonarHotSpot` has no dependencies beyond the .NET runtime.

#### Callers

- **SonarQubeClient** — creates `SonarHotSpot` records in `ParseHotSpot` from the API
  response.
- **SonarQualityResult** — holds and renders the `IReadOnlyList<SonarHotSpot>` in
  `AppendHotSpotsSection`.
