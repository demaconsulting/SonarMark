# SonarQualityResult

## Overview

`SonarQualityResult` is an immutable record that aggregates the full quality
analysis result for a SonarQube/SonarCloud project: the quality gate status,
the list of gate conditions, all issues, and all security hot-spots. It also
contains the `ToMarkdown` method that converts the result into a formatted
markdown report.

## Design Decisions

### Record with ToMarkdown Method

`SonarQualityResult` is a record for immutability and concise construction, but
also carries the `ToMarkdown` behavior method. This co-locates the data and its
primary transformation, making the class self-contained and easy to test in
isolation without requiring a separate formatter class.

### Configurable Depth

`ToMarkdown` accepts a `depth` parameter (defaulting to 1) that controls the
markdown heading level. This allows callers to embed the report as a sub-section
of a larger document without heading-level conflicts.

### Friendly Names for Metrics

The `ToMarkdown` implementation maps internal metric key names (e.g.,
`new_reliability_rating`) to human-readable labels (e.g., `Reliability Rating`).
This improves report readability without requiring the API to return display names.

### Compiler-Style Issue Output

Issues and hot-spots are rendered in compiler-style format
(`component:line: severity: message`) so that they can be read alongside build
output and parsed by tools that understand that format.

## Satisfies Requirements

- `SonarMark-Server-QualityGate` — `QualityGateStatus` and `Conditions` hold the quality gate data
- `SonarMark-Server-Issues` — `Issues` holds the list of fetched issues
- `SonarMark-Server-HotSpots` — `HotSpots` holds the list of fetched hot-spots
- `SonarMark-Report-Markdown` — `ToMarkdown` generates the markdown report content
- `SonarMark-Report-Depth` — the `depth` parameter controls heading levels
- `SonarMark-Report-QualityGate` — quality gate status and conditions are included in the report
- `SonarMark-Report-Issues` — issues are categorized and rendered in the report
- `SonarMark-Report-HotSpots` — hot-spots are rendered in the report
