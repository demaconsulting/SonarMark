# SonarMark

Tool to create a code quality report from a SonarQube/SonarCloud analysis

## Overview

SonarMark is a .NET command-line tool that generates markdown reports from SonarQube/SonarCloud analysis
results. It fetches quality gate status, issues, and security hot-spots directly from the SonarQube/SonarCloud
API and generates comprehensive markdown reports.

## Installation

```bash
dotnet tool install --global DemaConsulting.SonarMark
```

## Usage

```bash
sonarmark --server <url> --project-key <key> [options]
```

### Required Arguments

- `--server <url>` - SonarQube/SonarCloud server URL (e.g., `https://sonarcloud.io`)
- `--project-key <key>` - Project key in SonarQube/SonarCloud

### Optional Arguments

- `--branch <name>` - Branch name to query (defaults to main branch if not specified)
- `--token <token>` - Personal access token for authentication
- `--report <file>` - Export quality results to markdown file
- `--report-depth <depth>` - Markdown header depth for report (default: 1)
- `--enforce` - Return non-zero exit code if quality gate fails
- `--silent` - Suppress console output
- `--log <file>` - Write output to log file
- `-v, --version` - Display version information
- `-?, -h, --help` - Display help message

### Examples

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

**Enforce quality gate and fail on errors:**

```bash
sonarmark --server https://sonarcloud.io \
  --project-key my-org_my-project \
  --token $SONAR_TOKEN \
  --enforce
```

## Report Contents

The generated markdown report includes:

1. **Quality Gate Status** - Overall pass/fail status
2. **Conditions** - Quality gate conditions with thresholds and actual values
3. **Issues** - Open and confirmed issues grouped by type and severity
4. **Security Hot-Spots** - Security vulnerabilities requiring review

## Authentication

For private projects or SonarCloud, you'll need a personal access token:

1. Generate a token in SonarQube/SonarCloud user settings
2. Pass it using the `--token` parameter or set the `SONAR_TOKEN` environment variable

## Exit Codes

- `0` - Success
- `1` - Error occurred or quality gate failed (with `--enforce`)

## License

This project is licensed under the MIT License - see the LICENSE file for details.
