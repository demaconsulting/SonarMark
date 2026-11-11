# Introduction

This guide describes how to install, configure, and use SonarMark.

## Purpose

SonarMark is a .NET command-line tool that generates comprehensive markdown reports from
SonarQube/SonarCloud analysis results. It fetches quality gate status, issues, and security
hot-spots directly from the SonarQube/SonarCloud API, making it straightforward to integrate
code quality reporting into CI/CD pipelines and documentation workflows.

The workflow below shows how SonarMark is used: analyze your project with SonarCloud or SonarQube, run
the `sonarmark` command locally or in a CI pipeline against that analysis, and use the resulting markdown
quality report and exit code as a CI quality gate, code quality documentation, and compliance evidence.

![SonarMark workflow: analyze your project with SonarCloud or SonarQube, run sonarmark with the server, project key,
branch, and token options, get a markdown quality report with the quality gate status, issues, and security hot-spots
plus a pass/fail exit code, then use them as a CI quality gate, code quality documents, compliance evidence, or in pull
request reviews](workflow.svg)

SonarMark follows the [Continuous Compliance][continuous-compliance] methodology, ensuring
compliance evidence is generated automatically on every CI run. Requirements are linked to
passing tests, and a trace matrix is auto-generated on each release.

## Scope

This guide covers:

- Installation and prerequisites for SonarMark
- Full command-line option reference
- Usage examples and common use cases
- Generated report format and structure
- Self-validation testing
- Best practices, troubleshooting, and exit codes

SonarMark requires .NET 10 or 11 and runs on Windows, Linux, and macOS. It connects to
SonarQube or SonarCloud over HTTPS.

## References

- [SonarQube Documentation][sonarqube-docs]
- [SonarCloud Documentation][sonarcloud-docs]
- [.NET SDK Downloads][dotnet-download]
- [SonarMark releases][sonarmark-releases]

[continuous-compliance]: https://github.com/demaconsulting/ContinuousCompliance
[dotnet-download]: https://dotnet.microsoft.com/download
[sonarqube-docs]: https://docs.sonarqube.org/latest/
[sonarcloud-docs]: https://docs.sonarcloud.io/
[sonarmark-releases]: https://github.com/demaconsulting/SonarMark/releases
