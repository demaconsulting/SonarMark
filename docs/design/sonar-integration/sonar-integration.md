# SonarIntegration Subsystem

## Overview

The SonarIntegration subsystem is responsible for communicating with SonarQube/SonarCloud
servers via their REST API, fetching quality gate status, issues, and security hot-spots.

## Units

| Unit | Source File | Purpose |
|:-----|:-----------|:--------|
| SonarQubeClient | `SonarIntegration/SonarQubeClient.cs` | HTTP API client, fetches quality gate, issues, and hot-spots |
| SonarHotSpot | `SonarIntegration/SonarHotSpot.cs` | Data record representing a SonarQube security hot-spot |
| SonarIssue | `SonarIntegration/SonarIssue.cs` | Data record representing a SonarQube issue |

## Interfaces

`SonarQubeClient.GetQualityResultByBranchAsync(server, projectKey, branch)` is the
primary interface returning a `SonarQualityResult` record.

## Interactions

1. `Program.ProcessSonarAnalysis` creates a `SonarQubeClient` (or uses injected factory)
2. `SonarQubeClient` makes HTTP calls to five SonarQube API endpoints
3. Results are returned as `SonarQualityResult` containing `SonarHotSpot` and `SonarIssue` lists
