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

## SonarQualityCondition

`SonarQualityCondition` is an immutable positional record declared in the same
file as `SonarQualityResult`. It represents one quality gate condition returned
by the `/api/qualitygates/project_status` endpoint.

| Parameter | C# Type | Description |
| :---------- | :-------- | :------------ |
| `Metric` | `string` | Metric key as returned by the API (e.g., `new_coverage`, `new_bugs`) |
| `Comparator` | `string` | Comparison operator (e.g., `LT`, `GT`) |
| `ErrorThreshold` | `string?` | Threshold value that triggers an error; `null` when not set |
| `ActualValue` | `string?` | Actual metric value from the analysis; `null` when not available |
| `Status` | `string` | Condition status returned by the API (e.g., `OK`, `WARN`, `ERROR`) |

## Satisfies Requirements

- `SonarMark-Report-QualityGate` — quality gate status and conditions are included in the report
- `SonarMark-Report-Issues` — issues are categorized and rendered in the report
- `SonarMark-Report-HotSpots` — hot-spots are rendered in the report
- `SonarMark-Report-DepthValidation` — `ToMarkdown` validates depth is in the range 1–6
