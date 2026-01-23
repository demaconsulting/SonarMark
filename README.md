# SonarMark

[![GitHub forks](https://img.shields.io/github/forks/demaconsulting/SonarMark?style=plastic)](https://github.com/demaconsulting/SonarMark/network/members)
[![GitHub stars](https://img.shields.io/github/stars/demaconsulting/SonarMark?style=plastic)](https://github.com/demaconsulting/SonarMark/stargazers)
[![GitHub contributors](https://img.shields.io/github/contributors/demaconsulting/SonarMark?style=plastic)](https://github.com/demaconsulting/SonarMark/graphs/contributors)
[![License](https://img.shields.io/github/license/demaconsulting/SonarMark?style=plastic)](https://github.com/demaconsulting/SonarMark/blob/main/LICENSE)
[![Build](https://img.shields.io/github/actions/workflow/status/demaconsulting/SonarMark/build_on_push.yaml)](https://github.com/demaconsulting/SonarMark/actions/workflows/build_on_push.yaml)
[![Quality Gate](https://sonarcloud.io/api/project_badges/measure?project=demaconsulting_SonarMark&metric=alert_status)](https://sonarcloud.io/dashboard?id=demaconsulting_SonarMark)
[![Security](https://sonarcloud.io/api/project_badges/measure?project=demaconsulting_SonarMark&metric=security_rating)](https://sonarcloud.io/dashboard?id=demaconsulting_SonarMark)
[![NuGet](https://img.shields.io/nuget/v/DemaConsulting.SonarMark?style=plastic)](https://www.nuget.org/packages/DemaConsulting.SonarMark)

Code Quality Reporting Tool for SonarQube/SonarCloud

## Overview

SonarMark is a .NET command-line tool that generates comprehensive markdown reports from SonarQube/SonarCloud
analysis results. It fetches quality gate status, issues, and security hot-spots directly from the
SonarQube/SonarCloud API, making it easy to integrate code quality reporting into your CI/CD pipelines and
documentation workflows.

## Features

- 📊 **Quality Gate Reports** - Retrieve and report quality gate status with detailed conditions
- 🐛 **Issue Analysis** - Fetch and categorize issues by type and severity
- 🔒 **Security Hot-Spots** - Identify and report security vulnerabilities requiring review
- 📝 **Markdown Output** - Generate human-readable markdown reports for easy sharing
- 🚀 **CI/CD Integration** - Enforce quality gates and fail builds on quality issues
- 🌐 **Multi-Platform** - Support for .NET 8, 9, and 10 across Windows, Linux, and macOS
- ✅ **Self-Validation** - Built-in tests to verify functionality without requiring a live server
- 🔗 **API Integration** - Direct integration with SonarQube and SonarCloud REST APIs

## Installation

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) 8.0, 9.0, or 10.0

### Global Installation

Install SonarMark as a global .NET tool for system-wide use:

```bash
dotnet tool install --global DemaConsulting.SonarMark
```

Verify the installation:

```bash
sonarmark --version
```

### Local Installation

Install SonarMark as a local tool in your project (recommended for team projects):

```bash
dotnet new tool-manifest  # if you don't have a tool manifest already
dotnet tool install DemaConsulting.SonarMark
```

Run the tool:

```bash
dotnet sonarmark --version
```

## Usage

### Basic Usage

Run the tool with the `--help` option to see available commands and options:

```bash
sonarmark --help
```

This will display:

```text
Usage: sonarmark [options]

Options:
  -v, --version              Display version information
  -?, -h, --help             Display this help message
  --silent                   Suppress console output
  --validate                 Run self-validation
  --results <file>           Write validation results to file (.trx or .xml)
  --enforce                  Return non-zero exit code if quality gate fails
  --log <file>               Write output to log file
  --server <url>             SonarQube/SonarCloud server URL
  --project-key <key>        SonarQube/SonarCloud project key
  --branch <name>            Branch name to query (default: main branch)
  --token <token>            Personal access token for SonarQube/SonarCloud
  --report <file>            Export quality results to markdown file
  --report-depth <depth>     Markdown header depth for report (default: 1)
```

### Quick Start Examples

**Generate a report for the main branch:**

```bash
sonarmark --server https://sonarcloud.io \
  --project-key my-org_my-project \
  --token $SONAR_TOKEN \
  --report quality-report.md
```

**Generate a report for a specific branch:**

```bash
sonarmark --server https://sonarcloud.io \
  --project-key my-org_my-project \
  --branch feature/new-feature \
  --token $SONAR_TOKEN \
  --report quality-report.md
```

**Enforce quality gate in CI/CD:**

```bash
sonarmark --server https://sonarcloud.io \
  --project-key my-org_my-project \
  --token $SONAR_TOKEN \
  --enforce
```

**Run self-validation:**

```bash
sonarmark --validate
```

**Run self-validation with test results output:**

```bash
sonarmark --validate --results validation-results.trx
```

### Self-Validation Tests

SonarMark includes built-in self-validation tests that verify the tool's functionality without requiring
a live SonarQube/SonarCloud server. These tests use mock data to validate core features and generate test
result files in TRX or JUnit format.

The self-validation suite includes the following tests:

| Test Name | Description |
| :-------- | :---------- |
| `SonarMark_QualityGateRetrieval` | Verifies fetching and processing quality gate status |
| `SonarMark_IssuesRetrieval` | Verifies fetching and processing code issues |
| `SonarMark_HotSpotsRetrieval` | Verifies fetching and processing security hot-spots |
| `SonarMark_MarkdownReportGeneration` | Verifies generating markdown reports with quality metrics |

These tests provide evidence of the tool's functionality and are particularly useful for:

- Verifying the installation is working correctly
- Running automated tests in CI/CD pipelines without requiring SonarQube access
- Generating test evidence for compliance and traceability requirements

For detailed usage instructions, command-line options, and examples, including tool update instructions, see the
[Usage Guide](https://github.com/demaconsulting/SonarMark/blob/main/docs/guide/guide.md).

## Report Format

The generated markdown report includes:

1. **Project Header** - Project name and dashboard link
2. **Quality Gate Status** - Overall pass/fail status (OK, ERROR, WARN, or NONE)
3. **Conditions** - Detailed quality gate conditions with metrics, comparators, thresholds, and actual values
4. **Issues** - Count and list of issues in compiler-style format with file, line, severity, type, rule, and message
5. **Security Hot-Spots** - Count and list of security vulnerabilities requiring review in compiler-style format

Example report structure:

```markdown
# Example Project Sonar Analysis

**Dashboard:** <https://sonarcloud.io/dashboard?id=my_project>

**Quality Gate Status:** ERROR

## Conditions

| Metric | Status | Comparator | Threshold | Actual |
|:-------------------------------|:-----:|:--:|--------:|-------:|
| Coverage on New Code | ERROR | LT | 80 | 65.5 |
| New Bugs | ERROR | GT | 0 | 3 |

## Issues

Found 2 issues

src/Program.cs(42): MAJOR CODE_SMELL [csharpsquid:S1234] Remove this unused variable
src/Helper.cs(15): MINOR CODE_SMELL [csharpsquid:S5678] Refactor this method

## Security Hot-Spots

Found 1 security hot-spot

src/Database.cs(88): HIGH [sql-injection] Make sure using this SQL query is safe
```

## Contributing

Contributions are welcome! We appreciate your interest in improving SonarMark.

Please see our [Contributing Guide](https://github.com/demaconsulting/SonarMark/blob/main/CONTRIBUTING.md) for
development setup, coding standards, and submission guidelines. Also review our
[Code of Conduct](https://github.com/demaconsulting/SonarMark/blob/main/CODE_OF_CONDUCT.md) for community guidelines.

For bug reports, feature requests, and questions, please use [GitHub Issues](https://github.com/demaconsulting/SonarMark/issues).

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/demaconsulting/SonarMark/blob/main/LICENSE)
file for details.

## Support

- 🐛 **Report Bugs**: [GitHub Issues](https://github.com/demaconsulting/SonarMark/issues)
- 💡 **Request Features**: [GitHub Issues](https://github.com/demaconsulting/SonarMark/issues)
- ❓ **Ask Questions**: [GitHub Discussions](https://github.com/demaconsulting/SonarMark/discussions)
- 📖 **Documentation**: [Usage Guide](https://github.com/demaconsulting/SonarMark/blob/main/docs/guide/guide.md)
- 🤝 **Contributing**: [Contributing Guide](https://github.com/demaconsulting/SonarMark/blob/main/CONTRIBUTING.md)

## Security

For security concerns and vulnerability reporting, please see our
[Security Policy](https://github.com/demaconsulting/SonarMark/blob/main/SECURITY.md).

## Acknowledgements

SonarMark is built with the following open-source projects:

- [.NET](https://dotnet.microsoft.com/) - Cross-platform framework for building applications
- [SonarQube](https://www.sonarqube.org/) - Continuous code quality inspection
- [SonarCloud](https://sonarcloud.io/) - Cloud-based code quality and security service
- [DemaConsulting.TestResults](https://github.com/demaconsulting/TestResults) - Test results parsing library
