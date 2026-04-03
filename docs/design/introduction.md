# Introduction

This document contains the software design specification for the SonarMark project.

## Purpose

SonarMark is a .NET command-line tool that generates comprehensive markdown reports from
SonarQube/SonarCloud analysis results. This design document describes the internal structure
of each software unit, explaining the design decisions and implementation approach for each
class in the codebase.

## Scope

This design document covers the software units that make up SonarMark.

## Audience

This document is intended for:

- Software developers working on SonarMark
- Quality assurance teams validating the implementation against requirements
- Reviewers performing formal code reviews

## Software Structure

The following tree shows how the SonarMark software items are organized across the
system, subsystem, and unit levels:

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
│   └── SonarQualityResult (Unit)
└── SelfTest (Subsystem)
    └── Validation (Unit)
```

Each unit is described in detail in its own chapter within this document.

## Folder Layout

The source code folder structure mirrors the top-level subsystem breakdown above, giving
reviewers an explicit navigation aid from design to code:

```text
src/DemaConsulting.SonarMark/
├── Program.cs                      — entry point, dispatch, parameter validation, report writing
├── Cli/
│   └── Context.cs                  — argument parsing, output, log-file, enforce, results-file
├── SonarIntegration/
│   ├── SonarQubeClient.cs          — HTTP API client, fetches quality gate, issues, and hot-spots
│   ├── SonarHotSpot.cs             — data record representing a SonarQube security hot-spot
│   └── SonarIssue.cs               — data record representing a SonarQube issue
├── ReportGeneration/
│   └── SonarQualityResult.cs       — aggregates results and renders the markdown report
└── SelfTest/
    └── Validation.cs               — self-validation runner, writes TRX and JUnit result files
```

The test project mirrors the same layout under `test/DemaConsulting.SonarMark.Tests/`.

The design documentation mirrors the same structure under `docs/design/sonar-mark/`:

```text
docs/design/sonar-mark/
├── sonar-mark.md                   — system-level design documentation
├── program.md                      — entry point unit design
├── cli/
│   ├── cli.md                      — CLI subsystem overview
│   └── context.md                  — Context unit design
├── sonar-integration/
│   ├── sonar-integration.md        — SonarQube integration subsystem overview
│   ├── sonar-qube-client.md        — SonarQubeClient unit design
│   ├── sonar-hot-spot.md           — SonarHotSpot unit design
│   └── sonar-issue.md              — SonarIssue unit design
├── report-generation/
│   ├── report-generation.md        — report generation subsystem overview
│   └── sonar-quality-result.md     — SonarQualityResult unit design
└── self-test/
    ├── self-test.md                — self-test subsystem overview
    └── validation.md               — Validation unit design
```
