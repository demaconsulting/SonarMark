# Introduction

This document describes how each software item in SonarMark is verified.

## Purpose

This document describes how each software item in SonarMark is verified — local items (systems, subsystems,
and units). For each item it names the test scenarios that verify its requirements. A reviewer should be
able to confirm coverage completeness without reading test code.

## Scope

Local items covered by this document:

- **SonarMark**: system, subsystem, and unit verification including the Program entry point, Cli subsystem,
  SonarIntegration subsystem, ReportGeneration subsystem, and SelfTest subsystem.

OTS items covered by this document:

- **DemaConsulting.TestResults**: integration verification.
- **SysML2Tools**: integration verification.

Shared Packages covered by this document:

- **SonarMark (released)**: integration and usage verification.

Out of scope: test infrastructure, build pipeline scripts, and the test project itself
(`test/DemaConsulting.SonarMark.Tests/`).

## Companion Artifact Structure

Local items have parallel artifacts in:

- Requirements: `docs/reqstream/sonar-mark/sonar-mark.yaml`,
  `docs/reqstream/sonar-mark[/{subsystem-name}...]/{item}.yaml`
- Design: `docs/design/sonar-mark.md`,
  `docs/design/sonar-mark[/{subsystem-name}...]/{item}.md`
- Verification: `docs/verification/sonar-mark.md`,
  `docs/verification/sonar-mark[/{subsystem-name}...]/{item}.md`
- Source: `src/DemaConsulting.SonarMark[/{SubsystemName}...]/{Item}.cs`
- Tests: `test/DemaConsulting.SonarMark.Tests[/{SubsystemName}...]/{Item}Tests.cs`

OTS items have integration/usage verification documentation parallel to system folders:

- Requirements: `docs/reqstream/ots/{ots-name}.yaml`
- Design: `docs/design/ots/{ots-name}.md`
- Verification: `docs/verification/ots/{ots-name}.md`

Shared Packages have integration/usage verification documentation parallel to system and OTS folders:

- Requirements: `docs/reqstream/shared/{package-name}.yaml`
- Design: `docs/design/shared/{package-name}.md`
- Verification: `docs/verification/shared/{package-name}.md`

Review-sets: defined in `.reviewmark.yaml`

## References

- [SonarQube Web API Documentation](https://next.sonarqube.com/sonarqube/web_api)
- [SonarCloud Web API Documentation](https://sonarcloud.io/web_api)
