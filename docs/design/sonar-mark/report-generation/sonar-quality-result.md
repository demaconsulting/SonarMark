### SonarQualityResult

![ReportGeneration Structure](ReportGenerationView.svg)

#### Purpose

`SonarQualityResult` is an immutable record that aggregates the full quality analysis result
for a SonarQube/SonarCloud project and renders it as a formatted markdown report via
`ToMarkdown`. It is the single hand-off point between the SonarIntegration and
ReportGeneration subsystems.

#### Data Model

**ServerUrl**: `string` — base URL of the SonarQube/SonarCloud server; used to construct
the dashboard link in the report header.

**ProjectKey**: `string` — project key; used in the dashboard URL and to strip the project
key prefix from component paths in the rendered output.

**ProjectName**: `string` — human-readable project name fetched from the API; used as the
report heading.

**QualityGateStatus**: `string` — overall quality gate status from the API (`OK`, `WARN`,
`ERROR`, or `NONE`).

**Conditions**: `IReadOnlyList<SonarQualityCondition>` — quality gate conditions; empty when
no conditions are defined. Each `SonarQualityCondition` record holds `Metric` (metric key),
`Comparator` (e.g., `LT`, `GT`), `ErrorThreshold` (`string?`), `ActualValue` (`string?`),
and `Status` (e.g., `OK`, `WARN`, `ERROR`).

**MetricNames**: `IReadOnlyDictionary<string, string>` — maps internal metric keys (e.g.,
`new_coverage`) to human-readable display names (e.g., `Coverage on New Code`); used in the
conditions table to improve readability.

**Issues**: `IReadOnlyList<SonarIssue>` — code quality issues; empty when none are found.

**HotSpots**: `IReadOnlyList<SonarHotSpot>` — security hot-spots; empty when none are found.

**SonarQualityCondition**: An immutable data record modelling a single quality gate condition as
returned by the SonarQube/SonarCloud API, with five individually-accessible fields:

- `Metric` (`string`) — the internal metric key (e.g., `new_coverage`, `new_bugs`).
- `Comparator` (`string`) — the comparison operator (e.g., `LT`, `GT`).
- `ErrorThreshold` (`string?`) — the configured error threshold value; nullable when not set.
- `ActualValue` (`string?`) — the actual measured value from the analysis; nullable when unavailable.
- `Status` (`string`) — the per-condition gate status (`OK`, `WARN`, `ERROR`).

`SonarQualityCondition` records are carried in the `Conditions` list, rendered as rows in the
conditions table by `AppendConditionsSection`, and individually tested for rendering correctness.

#### Key Methods

**ToMarkdown**: Converts the quality result to a markdown string.

- *Parameters*: `int depth` — heading depth for the report title (1–6).
- *Returns*: `string` — complete GitHub-flavored markdown report.
- *Preconditions*: `depth` must be in the range 1–6.
- *Postconditions*: Returned string contains all four sections (header, conditions, issues,
  hot-spots); the conditions section is omitted when `Conditions` is empty.

Computes the heading prefix (`#` × depth) and sub-heading prefix (`#` × min(depth+1, 6)),
then delegates to `AppendHeader`, `AppendConditionsSection`, `AppendIssuesSection`, and
`AppendHotSpotsSection`. Issues and hot-spots sections always appear with a count line;
individual items are listed only when the count is greater than zero. Component paths are
cleaned by stripping the `{ProjectKey}:` prefix so consumers see relative paths. Metric keys
in the conditions table are resolved to display names via `MetricNames`; the raw key is used
as a fallback when a key is absent.

#### Error Handling

`ToMarkdown` throws `ArgumentOutOfRangeException` when `depth` is outside 1–6. No other
error conditions exist; the record is fully populated before `ToMarkdown` is called.

#### Dependencies

- **SonarIssue** — items in the `Issues` list, rendered in `AppendIssuesSection`.
- **SonarHotSpot** — items in the `HotSpots` list, rendered in `AppendHotSpotsSection`.
- **SonarQualityCondition** — items in the `Conditions` list, rendered in
  `AppendConditionsSection`.

#### Callers

- **SonarQubeClient.GetQualityResultByBranchAsync** — constructs and returns the
  `SonarQualityResult` instance.
- **Program.ProcessSonarAnalysis** — calls `ToMarkdown` and writes the result to disk.
- **Validation.RunMarkdownReportGenerationTest** — exercises the full pipeline including
  `ToMarkdown` to validate the rendered output.
