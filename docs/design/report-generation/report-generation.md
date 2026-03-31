# ReportGeneration Subsystem

## Overview

The ReportGeneration subsystem aggregates quality analysis results and renders them
as a structured GitHub-flavoured markdown report.

## Units

| Unit | Source File | Purpose |
|:-----|:-----------|:--------|
| SonarQualityResult | `ReportGeneration/SonarQualityResult.cs` | Aggregates results and renders the markdown report |

## Interfaces

`SonarQualityResult.ToMarkdown(depth)` generates the full markdown report with
configurable heading depth (1-6).

## Interactions

1. `SonarQubeClient.GetQualityResultByBranchAsync` returns a populated `SonarQualityResult`
2. `Program.ProcessSonarAnalysis` calls `SonarQualityResult.ToMarkdown(context.ReportDepth)`
3. The resulting markdown is written to the file specified by `--report`
