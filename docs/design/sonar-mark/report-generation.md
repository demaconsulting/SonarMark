## ReportGeneration

### Overview

The ReportGeneration subsystem aggregates the quality analysis results returned by the
SonarIntegration subsystem and renders them as a structured GitHub-flavoured markdown report.
It contains a single unit, `SonarQualityResult`, which holds the quality gate status,
conditions, metric name dictionary, issues list, and hot-spots list, and exposes a
`ToMarkdown` method to convert the aggregated data into a markdown string.

### Interfaces

**SonarQualityResult.ToMarkdown**: Renders the full markdown report.

- *Type*: In-process .NET public API.
- *Role*: Provider — called by `Program.ProcessSonarAnalysis`.
- *Contract*: Accepts `int depth` (1–6). Returns a `string` containing the complete
  GitHub-flavoured markdown report including a project header with dashboard link, quality
  gate status, a conditions table, an issues list, and a security hot-spots list.
- *Constraints*: Throws `ArgumentOutOfRangeException` when `depth` is outside the range 1–6.

> **Cross-subsystem dependency**: `SonarQualityResult` consumes the `SonarIssue` and
> `SonarHotSpot` record types defined in the `SonarIntegration` subsystem. These types are
> passed in at construction time and are referenced only as data records; no methods on those
> types are called during rendering.

### Design

1. `SonarQubeClient.GetQualityResultByBranchAsync` returns a `SonarQualityResult` populated
   with the quality gate status, conditions, metric names, issues, and hot-spots.
2. `Program.ProcessSonarAnalysis` calls `SonarQualityResult.ToMarkdown(context.Depth)` to
   render the markdown.
3. `Program` writes the returned string to the file specified by `--report` using
   `File.WriteAllText`.
