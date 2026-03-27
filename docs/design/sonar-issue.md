# SonarIssue

## Overview

`SonarIssue` is an immutable record type that represents a single code quality
issue returned by the SonarQube/SonarCloud API. It holds the fields needed to
render a compiler-style issue entry in the markdown report.

## Design Decisions

### Record Type

`SonarIssue` is declared as a `record` (positional record) for the same reasons
as `SonarHotSpot`: value-based equality, concise syntax, and suitability as a
data-transfer object.

### Severity and Rule Fields

The `Severity` and `Rule` fields are included because the markdown report groups
and labels issues by severity. The `Component` and `Line` fields provide the
source location needed for compiler-style output.

## Satisfies Requirements

- `SonarMark-Server-Issues` — holds the data for one issue fetched from the server
- `SonarMark-Report-Issues` — provides the fields rendered in the issues section of the report
