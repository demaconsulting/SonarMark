# SonarHotSpot

## Overview

`SonarHotSpot` is an immutable record type that represents a single security
hot-spot returned by the SonarQube/SonarCloud API. It holds the minimum set of
fields needed to render a useful entry in the markdown report: key, component,
line (optional), message, security category, and vulnerability probability.

## Design Decisions

### Record Type

`SonarHotSpot` is declared as a `record` (positional record) rather than a class.
Records provide value-based equality, concise syntax for immutable data, and
built-in `ToString` with property names — all appropriate for a data-transfer
object whose sole job is to carry API response data.

### Minimal Fields

Only the fields actually used in the markdown report are captured: key, component,
line, message, security category, and vulnerability probability. Additional fields
returned by the API are ignored to keep the model focused and reduce coupling to the
API response schema.

## Fields

| Field | C# Type | Description |
|:------|:--------|:------------|
| `Key` | `string` | Unique identifier for the hot-spot |
| `Component` | `string` | Fully-qualified component path including project key prefix |
| `Line` | `int?` | Source line number; `null` when the hot-spot has no line association |
| `Message` | `string` | Human-readable description of the hot-spot |
| `SecurityCategory` | `string` | Security category key (e.g., `sql-injection`, `xss`) |
| `VulnerabilityProbability` | `string` | Probability level returned by the API (e.g., `HIGH`, `MEDIUM`, `LOW`) |

## Satisfies Requirements

- `SonarMark-HotSpot-Record` — holds the data for one hot-spot fetched from the server
- `SonarMark-HotSpot-OptionalLine` — captures the optional line number field
- `SonarMark-HotSpot-VulnerabilityProbability` — captures the vulnerability probability field
