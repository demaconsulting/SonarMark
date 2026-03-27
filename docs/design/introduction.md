# Introduction

This document contains the software design specification for the SonarMark project.

## Purpose

SonarMark is a .NET command-line tool that generates comprehensive markdown reports from
SonarQube/SonarCloud analysis results. This design document describes the internal structure
of each software unit, explaining the design decisions and implementation approach for each
class in the codebase.

## Scope

This design document covers the software units that make up SonarMark, organized into the
following subsystems:

```text
SonarMark (system)
├── CLI (subsystem)
│   ├── Context           — argument parsing, output, log-file, enforce, results-file
│   └── Program           — entry point, dispatch, parameter validation, report writing
├── SonarQube Integration (subsystem)
│   ├── SonarQubeClient   — HTTP API client, fetches quality gate, issues, and hot-spots
│   ├── SonarHotSpot      — data record representing a SonarQube security hot-spot
│   └── SonarIssue        — data record representing a SonarQube issue
├── Report Generation (subsystem)
│   └── SonarQualityResult — aggregates results and renders the markdown report
└── Validation (subsystem)
    └── Validation        — self-validation runner, writes TRX and JUnit result files
```

## Audience

This document is intended for:

- Software developers working on SonarMark
- Quality assurance teams validating the implementation against requirements
- Reviewers performing formal code reviews
