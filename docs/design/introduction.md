# Introduction

This document contains the software design specification for the SonarMark system. SonarMark
is a single .NET CLI tool organized into one system with four subsystems — Cli, SonarIntegration,
ReportGeneration, and SelfTest — plus a top-level unit, Program, that serves as the entry point.

## Purpose

This document defines the design for each software item in SonarMark — full architectural and
detailed design for all local items (system, subsystems, and units). A reviewer should be able
to understand how each item satisfies its requirements without reading source code.

## Scope

Local items:

- **SonarMark**: system, subsystem, and unit design.

Out of scope: test projects, build pipeline scripts, and CI configuration.

## Software Structure

```text
SonarMark (System)
├── Program (Unit)
├── Cli (Subsystem)
│   └── Context (Unit)
├── SonarIntegration (Subsystem)
│   ├── SonarQubeClient (Unit)
│   ├── SonarHotSpot (Unit)
│   └── SonarIssue (Unit)
├── ReportGeneration (Subsystem)
│   ├── SonarQualityResult (Unit)
│   └── SonarQualityCondition (Unit)
└── SelfTest (Subsystem)
    └── Validation (Unit)
```

## Folder Layout

```text
src/DemaConsulting.SonarMark/
├── Program.cs                      — entry point, dispatch, parameter validation, report writing
├── Cli/
│   └── Context.cs                  — argument parsing (ArgumentParser internal class), output, log-file, enforce, results-file
├── SonarIntegration/
│   ├── SonarQubeClient.cs          — HTTP API client, fetches quality gate, issues, and hot-spots
│   ├── SonarHotSpot.cs             — data record representing a SonarQube security hot-spot
│   └── SonarIssue.cs               — data record representing a SonarQube issue
├── ReportGeneration/
│   └── SonarQualityResult.cs       — aggregates results and renders the markdown report; contains SonarQualityCondition data record
└── SelfTest/
    └── Validation.cs               — self-validation runner, writes TRX and JUnit result files

test/DemaConsulting.SonarMark.Tests/
└── (integration and unit tests)
```

## Companion Artifact Structure

Each local software item has corresponding artifacts in parallel directory trees:

- Requirements: `docs/reqstream/sonar-mark/sonar-mark.yaml`,
  `docs/reqstream/sonar-mark[/{subsystem-name}...]/{item}.yaml`
- Design: `docs/design/sonar-mark.md`,
  `docs/design/sonar-mark[/{subsystem-name}...]/{item}.md`
- Verification: `docs/verification/sonar-mark.md`,
  `docs/verification/sonar-mark[/{subsystem-name}...]/{item}.md`
- Source: `src/DemaConsulting.SonarMark[/{SubsystemName}...]/{Item}.cs`
- Tests: `test/DemaConsulting.SonarMark.Tests[/{SubsystemName}...]/{Item}Tests.cs`

Review-sets: defined in `.reviewmark.yaml`

## References

- [SonarQube Web API Documentation](https://next.sonarqube.com/sonarqube/web_api)
- [SonarCloud Web API Documentation](https://sonarcloud.io/web_api)
