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

OTS items:

- **DemaConsulting.TestResults**: integration and usage design.
- **SysML2Tools**: integration and usage design.

Shared Packages:

- **SonarMark (released)**: integration and usage design.

Out of scope: test projects, build pipeline scripts, and CI configuration.

## Software Structure

The software structure is modeled in SysML2 under `docs/sysml2/` and rendered to the
diagram below by SysML2Tools as part of the build pipeline. AI agents should query the
SysML2 model directly (see the `sysml2tools-query` skill) rather than parsing this
diagram or the prose below.

![Software Structure](SoftwareStructureView.svg)

## Folder Layout

- **src/** - source files and projects
  - **DemaConsulting.SonarMark/** - main CLI application
    - **Cli/** - command-line interface subsystem
    - **SonarIntegration/** - SonarQube/SonarCloud API integration subsystem
    - **ReportGeneration/** - markdown report generation subsystem
    - **SelfTest/** - self-validation subsystem
- **test/** - test projects
  - **DemaConsulting.SonarMark.Tests/** - integration and unit tests

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

OTS items have integration/usage design documentation parallel to system folders:

- Requirements: `docs/reqstream/ots/{ots-name}.yaml`
- Design: `docs/design/ots/{ots-name}.md`
- Verification: `docs/verification/ots/{ots-name}.md`

Shared Packages have integration/usage design documentation parallel to system and OTS folders:

- Requirements: `docs/reqstream/shared/{package-name}.yaml`
- Design: `docs/design/shared/{package-name}.md`
- Verification: `docs/verification/shared/{package-name}.md`

Review-sets: defined in `.reviewmark.yaml`

## References

- [SonarQube Web API Documentation](https://next.sonarqube.com/sonarqube/web_api)
- [SonarCloud Web API Documentation](https://sonarcloud.io/web_api)
