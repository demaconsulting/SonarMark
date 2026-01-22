# SonarMark

[![GitHub forks](https://img.shields.io/github/forks/demaconsulting/SonarMark)](https://github.com/demaconsulting/SonarMark/network/members)
[![GitHub stars](https://img.shields.io/github/stars/demaconsulting/SonarMark)](https://github.com/demaconsulting/SonarMark/stargazers)
[![GitHub contributors](https://img.shields.io/github/contributors/demaconsulting/SonarMark)](https://github.com/demaconsulting/SonarMark/graphs/contributors)
[![License](https://img.shields.io/github/license/demaconsulting/SonarMark)](https://github.com/demaconsulting/SonarMark/blob/main/LICENSE)
[![Build](https://img.shields.io/github/actions/workflow/status/demaconsulting/SonarMark/build_on_push.yaml)](https://github.com/demaconsulting/SonarMark/actions/workflows/build_on_push.yaml)
[![Quality Gate](https://sonarcloud.io/api/project_badges/measure?project=demaconsulting_SonarMark&metric=alert_status)](https://sonarcloud.io/dashboard?id=demaconsulting_SonarMark)
[![Security](https://sonarcloud.io/api/project_badges/measure?project=demaconsulting_SonarMark&metric=security_rating)](https://sonarcloud.io/dashboard?id=demaconsulting_SonarMark)
[![NuGet](https://img.shields.io/nuget/v/DemaConsulting.SonarMark)](https://www.nuget.org/packages/DemaConsulting.SonarMark)

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

### Update

To update to the latest version:

```bash
# For global tools
dotnet tool update --global DemaConsulting.SonarMark

# For local tools
dotnet tool update DemaConsulting.SonarMark
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

For detailed usage instructions, command-line options, and examples, see the
[Usage Guide](https://github.com/demaconsulting/SonarMark/blob/main/docs/guide/guide.md).

## Report Format

The generated markdown report includes:

1. **Quality Gate Status** - Overall pass/fail status
2. **Quality Gate Conditions** - Detailed conditions with thresholds and actual values
3. **Issues** - Open and confirmed issues grouped by type (bugs, code smells, vulnerabilities) and severity
4. **Security Hot-Spots** - Security vulnerabilities requiring review

Example report structure:

```markdown
# Quality Gate Status

**Status**: PASSED

## Quality Gate Conditions

| Condition | Status | Actual | Threshold |
|-----------|--------|--------|-----------|
| Coverage | OK | 85.2% | > 80% |
| Duplications | OK | 2.1% | < 3% |

## Issues

### Bugs
- **Major**: 2
- **Minor**: 5

### Code Smells
- **Major**: 15
- **Minor**: 32
```

## Self-Validation

SonarMark includes built-in self-validation tests to verify functionality without requiring access to a real
SonarQube/SonarCloud server. The validation uses mock data to test core features.

Run validation with:

```bash
sonarmark --validate
```

Optionally save results to TRX or JUnit XML format:

```bash
# Save as TRX format
sonarmark --validate --results validation-results.trx

# Save as JUnit XML format
sonarmark --validate --results validation-results.xml
```

The validation tests include:

- Quality gate status retrieval and parsing
- Issues fetching and parsing
- Security hot-spots fetching and parsing
- Markdown report generation

## Development

### Development Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) 8.0, 9.0, or 10.0
- Git

### Building

Clone the repository and build the solution:

```bash
git clone https://github.com/demaconsulting/SonarMark.git
cd SonarMark
dotnet restore
dotnet build --configuration Release
```

### Testing

Run all tests:

```bash
dotnet test --configuration Release
```

### Packaging

Create NuGet packages:

```bash
dotnet pack --configuration Release --no-build
```

The packages will be created in `src/DemaConsulting.SonarMark/bin/Release/`.

## Contributing

Contributions are welcome! We appreciate your interest in improving SonarMark.

Please see our [Code of Conduct](https://github.com/demaconsulting/SonarMark/blob/main/CODE_OF_CONDUCT.md) for
community guidelines. For bug reports, feature requests, and questions, please use
[GitHub Issues](https://github.com/demaconsulting/SonarMark/issues).

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/demaconsulting/SonarMark/blob/main/LICENSE)
file for details.

## Support

- 🐛 **Report Bugs**: [GitHub Issues](https://github.com/demaconsulting/SonarMark/issues)
- 💡 **Request Features**: [GitHub Issues](https://github.com/demaconsulting/SonarMark/issues)
- ❓ **Ask Questions**: [GitHub Discussions](https://github.com/demaconsulting/SonarMark/discussions)
- 📖 **Documentation**: [Usage Guide](https://github.com/demaconsulting/SonarMark/blob/main/docs/guide/guide.md)

## Security

For security concerns and vulnerability reporting, please see our
[Security Policy](https://github.com/demaconsulting/SonarMark/blob/main/SECURITY.md).

## Acknowledgements

SonarMark is built with the following open-source projects:

- [.NET](https://dotnet.microsoft.com/) - Cross-platform framework for building applications
- [SonarQube](https://www.sonarqube.org/) - Continuous code quality inspection
- [SonarCloud](https://sonarcloud.io/) - Cloud-based code quality and security service
- [DemaConsulting.TestResults](https://github.com/demaconsulting/TestResults) - Test results parsing library
