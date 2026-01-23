# SonarMark Usage Guide

This guide provides comprehensive documentation for using SonarMark to generate code quality reports from
SonarQube/SonarCloud analysis results.

## Introduction

SonarMark is a .NET command-line tool that fetches quality gate status, issues, and security hot-spots from
SonarQube/SonarCloud and generates comprehensive markdown reports. It's designed to integrate seamlessly into CI/CD
pipelines for automated quality reporting.

### Key Features

- **Quality Gate Reporting**: Fetch and report quality gate status with detailed conditions
- **Issue Analysis**: Retrieve and categorize issues by type and severity
- **Security Hot-Spots**: Identify and report security vulnerabilities
- **Markdown Output**: Generate human-readable markdown reports
- **CI/CD Integration**: Support for enforcement mode to fail builds on quality gate failures
- **Multi-Platform**: Works on Windows, Linux, and macOS with .NET 8, 9, or 10

## Installation

### Prerequisites

- [.NET SDK][dotnet-download] 8.0, 9.0, or 10.0

### Global Installation

Install SonarMark as a global .NET tool for system-wide access:

```bash
dotnet tool install --global DemaConsulting.SonarMark
```

Verify the installation:

```bash
sonarmark --version
```

### Local Installation

For team projects, install SonarMark as a local tool to ensure version consistency:

```bash
# Create tool manifest if it doesn't exist
dotnet new tool-manifest

# Install the tool
dotnet tool install DemaConsulting.SonarMark
```

Run the locally installed tool:

```bash
dotnet sonarmark --version
```

### Update

To update to the latest version:

```bash
# Global installation
dotnet tool update --global DemaConsulting.SonarMark

# Local installation
dotnet tool update DemaConsulting.SonarMark
```

## Getting Started

### Basic Usage

The most basic usage requires specifying the SonarQube/SonarCloud server URL and project key:

```bash
sonarmark --server https://sonarcloud.io --project-key my-org_my-project
```

### With Authentication

For private projects, provide an authentication token:

```bash
sonarmark --server https://sonarcloud.io \
  --project-key my-org_my-project \
  --token $SONAR_TOKEN
```

### Generating a Report

To generate a markdown report file:

```bash
sonarmark --server https://sonarcloud.io \
  --project-key my-org_my-project \
  --token $SONAR_TOKEN \
  --report quality-report.md
```

## Command-Line Options

### Display Options

#### `--version`, `-v`

Display version information and exit.

```bash
sonarmark --version
```

#### `--help`, `-h`, `-?`

Display help message with all available options.

```bash
sonarmark --help
```

### Output Control

#### `--silent`

Suppress console output. Useful in automated scripts where only the exit code matters.

```bash
sonarmark --server https://sonarcloud.io \
  --project-key my-org_my-project \
  --silent
```

#### `--log <file>`

Write all output to a log file in addition to console output.

```bash
sonarmark --server https://sonarcloud.io \
  --project-key my-org_my-project \
  --log analysis.log
```

### SonarQube/SonarCloud Connection

#### `--server <url>` (Required)

SonarQube or SonarCloud server URL.

Examples:

```bash
# SonarCloud
--server https://sonarcloud.io

# Self-hosted SonarQube
--server https://sonar.example.com
```

#### `--project-key <key>` (Required)

Project key in SonarQube/SonarCloud. This is the unique identifier for your project.

```bash
--project-key my-organization_my-project
```

#### `--branch <name>`

Branch name to query. If not specified, uses the main branch configured in SonarQube/SonarCloud.

```bash
# Analyze a specific branch
sonarmark --server https://sonarcloud.io \
  --project-key my-org_my-project \
  --branch feature/new-feature
```

#### `--token <token>`

Personal access token for authentication. Can also be provided via the `SONAR_TOKEN` environment variable.

```bash
# Using command-line argument
sonarmark --server https://sonarcloud.io \
  --project-key my-org_my-project \
  --token squ_abc123...

# Using environment variable
export SONAR_TOKEN=squ_abc123...
sonarmark --server https://sonarcloud.io \
  --project-key my-org_my-project \
  --token $SONAR_TOKEN
```

### Report Generation

#### `--report <file>`

Export quality results to a markdown file. The file will contain quality gate status, conditions, issues, and
security hot-spots.

```bash
sonarmark --server https://sonarcloud.io \
  --project-key my-org_my-project \
  --report quality-report.md
```

#### `--report-depth <depth>`

Set the markdown header depth for the report. Default is 1. Use this when embedding the report in larger documents.

```bash
# Use level 2 headers (##) instead of level 1 (#)
sonarmark --server https://sonarcloud.io \
  --project-key my-org_my-project \
  --report quality-report.md \
  --report-depth 2
```

### Quality Enforcement

#### `--enforce`

Return a non-zero exit code if the quality gate fails. Essential for CI/CD pipelines to fail builds on quality issues.

```bash
sonarmark --server https://sonarcloud.io \
  --project-key my-org_my-project \
  --enforce
```

### Self-Validation

#### `--validate`

Run built-in self-validation tests. These tests verify SonarMark functionality without requiring access to a real
SonarQube/SonarCloud server.

```bash
sonarmark --validate
```

#### `--results <file>`

Write validation results to a file. Supports TRX (`.trx`) and JUnit XML (`.xml`) formats. Requires `--validate`.

```bash
# TRX format
sonarmark --validate --results validation-results.trx

# JUnit XML format
sonarmark --validate --results validation-results.xml
```

## Common Use Cases

### CI/CD Integration

Integrate SonarMark into your CI/CD pipeline to automatically check code quality:

```yaml
# GitHub Actions example
- name: Check Code Quality
  run: |
    sonarmark \
      --server https://sonarcloud.io \
      --project-key ${{ secrets.SONAR_PROJECT_KEY }} \
      --token ${{ secrets.SONAR_TOKEN }} \
      --report quality-report.md \
      --enforce

- name: Upload Quality Report
  uses: actions/upload-artifact@v4
  if: always()
  with:
    name: quality-report
    path: quality-report.md
```

### Branch Analysis

Analyze specific branches during pull request reviews:

```bash
# Analyze pull request branch
sonarmark --server https://sonarcloud.io \
  --project-key my-org_my-project \
  --branch PR-123 \
  --token $SONAR_TOKEN \
  --report pr-quality.md
```

### Quality Gate Monitoring

Monitor quality gate status without generating a full report:

```bash
# Just check quality gate status
sonarmark --server https://sonarcloud.io \
  --project-key my-org_my-project \
  --token $SONAR_TOKEN \
  --enforce \
  --silent
```

### Automated Reporting

Generate daily/weekly quality reports:

```bash
#!/bin/bash
# Generate timestamped quality report
TIMESTAMP=$(date +%Y%m%d-%H%M%S)
sonarmark --server https://sonarcloud.io \
  --project-key my-org_my-project \
  --token $SONAR_TOKEN \
  --report "quality-report-${TIMESTAMP}.md" \
  --log "analysis-${TIMESTAMP}.log"
```

## Report Format

The generated markdown report includes the following sections:

### Project Header

The report begins with the project name and a link to the SonarQube/SonarCloud dashboard:

```markdown
# Example Project Sonar Analysis

**Dashboard:** <https://sonarcloud.io/dashboard?id=my_project>
```

### Quality Gate Status

Shows whether the project passed or failed the quality gate. Possible values are OK, ERROR, WARN, or NONE:

```markdown
**Quality Gate Status:** OK
```

or

```markdown
**Quality Gate Status:** ERROR
```

### Conditions

If quality gate conditions exist, they are displayed in a table with the following columns:

- **Metric**: The friendly name of the metric being measured (e.g., "Coverage on New Code")
- **Status**: The condition status (OK, ERROR, or WARN)
- **Comparator**: The comparison operator (LT for less than, GT for greater than)
- **Threshold**: The threshold value that was set
- **Actual**: The actual measured value

```markdown
## Conditions

| Metric | Status | Comparator | Threshold | Actual |
|:-------------------------------|:-----:|:--:|--------:|-------:|
| Coverage on New Code | ERROR | LT | 80 | 65.5 |
| New Bugs | ERROR | GT | 0 | 3 |
| Duplications | OK | LT | 3 | 2.1 |
```

### Issues

The issues section shows a count of issues found and lists each issue in compiler-style format:

```markdown
## Issues

Found 3 issues

src/Program.cs(42): MAJOR CODE_SMELL [csharpsquid:S1234] Remove this unused variable
src/Helper.cs(15): MINOR CODE_SMELL [csharpsquid:S5678] Refactor this method to reduce complexity
src/Service.cs(88): MAJOR BUG [csharpsquid:S9012] Fix this potential null reference
```

Each issue line includes:

- **File path and line number**: `src/Program.cs(42)` or just `src/Program.cs` if no line number
- **Severity**: BLOCKER, CRITICAL, MAJOR, MINOR, or INFO
- **Type**: BUG, VULNERABILITY, or CODE_SMELL
- **Rule**: The SonarQube rule identifier in brackets
- **Message**: Description of the issue

If no issues are found:

```markdown
## Issues

Found no issues
```

### Security Hot-Spots

The security hot-spots section shows a count and lists each hot-spot in compiler-style format:

```markdown
## Security Hot-Spots

Found 2 security hot-spots

src/Database.cs(88): HIGH [sql-injection] Make sure using this SQL query is safe
src/Auth.cs(42): MEDIUM [weak-cryptography] Use a stronger encryption algorithm
```

Each hot-spot line includes:

- **File path and line number**: `src/Database.cs(88)` or just `src/Database.cs` if no line number
- **Vulnerability Probability**: HIGH, MEDIUM, or LOW
- **Security Category**: The type of security issue in brackets (e.g., sql-injection, weak-cryptography)
- **Message**: Description of the security concern

If no security hot-spots are found:

```markdown
## Security Hot-Spots

Found no security hot-spots
```

## Running Self-Validation

SonarMark includes built-in self-validation tests to verify functionality without requiring access to a real
SonarQube/SonarCloud server. The validation uses mock data to test core features.

### Running Validation

```bash
sonarmark --validate
```

### Validation Tests

The self-validation suite includes the following tests that verify core functionality:

| Test Name | Description |
| :-------- | :---------- |
| `SonarMark_QualityGateRetrieval` | Verifies fetching and processing quality gate status from SonarQube/SonarCloud |
| `SonarMark_IssuesRetrieval` | Verifies fetching and processing code issues with severity classification |
| `SonarMark_HotSpotsRetrieval` | Verifies fetching and processing security hot-spots and vulnerabilities |
| `SonarMark_MarkdownReportGeneration` | Verifies generating markdown reports with quality metrics and findings |

These tests provide evidence of the tool's functionality and are particularly useful for:

- Verifying the installation is working correctly on different platforms and .NET versions
- Running automated tests in CI/CD pipelines without requiring SonarQube access
- Generating test evidence for compliance and traceability requirements
- Validating tool functionality before deployment

**Note**: The test names with the `SonarMark_` prefix are designed for clear identification in test
result files (TRX/JUnit) when integrating with larger projects or test frameworks.

### Validation Output

Example output:

```text
SonarMark version 1.0.0
Copyright (c) DEMA Consulting

# DEMA Consulting SonarMark
## Self-Validation Tests

[PASS] Quality Gate Status Retrieval
[PASS] Issues Retrieval
[PASS] Hot-Spots Retrieval
[PASS] Markdown Report Generation

Total Tests: 4
Passed: 4
Failed: 0
```

### Saving Validation Results

Save results in TRX or JUnit XML format for integration with test reporting tools:

```bash
# TRX format (for Azure DevOps, Visual Studio)
sonarmark --validate --results validation-results.trx

# JUnit XML format (for Jenkins, GitLab CI)
sonarmark --validate --results validation-results.xml
```

## Best Practices

### Authentication

- **Store tokens securely**: Use environment variables or secret management systems
- **Rotate tokens regularly**: Follow your organization's security policies
- **Use read-only tokens**: SonarMark only needs read access to the API
- **Don't commit tokens**: Never commit tokens to version control

### CI/CD Best Practices

- **Use enforcement mode**: Always use `--enforce` in CI/CD to fail builds on quality gate failures
- **Archive reports**: Save quality reports as build artifacts for historical tracking
- **Set timeouts**: Configure reasonable timeouts for API calls in CI/CD environments
- **Handle failures gracefully**: Use appropriate error handling in your CI/CD scripts

### Report Best Practices

- **Use meaningful filenames**: Include timestamps, branch names, or build numbers in report filenames
- **Adjust header depth**: Use `--report-depth` when embedding reports in larger documents
- **Combine with logging**: Use `--log` to capture detailed execution information

### Performance

- **Cache dependencies**: Use local tool installation to speed up execution in CI/CD
- **Minimize API calls**: Only fetch data when needed (e.g., don't generate reports if not required)
- **Use silent mode**: Suppress unnecessary output in automated scripts with `--silent`

## Troubleshooting

### Authentication Issues

**Problem**: `401 Unauthorized` error

**Solutions**:

- Verify your token is valid and not expired
- Ensure the token has appropriate permissions
- Check if the project exists and you have access to it
- For SonarCloud, verify you're using a user token, not a project analysis token

### Connection Issues

**Problem**: Cannot connect to SonarQube/SonarCloud server

**Solutions**:

- Verify the server URL is correct
- Check network connectivity and firewall rules
- Ensure the server is accessible from your environment
- Verify SSL/TLS certificates are valid
- For self-hosted SonarQube, check if the server is running

### Project Not Found

**Problem**: `404 Not Found` or project doesn't exist error

**Solutions**:

- Verify the project key is correct (case-sensitive)
- Ensure the project exists in SonarQube/SonarCloud
- Check if you have access to the project
- For branches, verify the branch exists in SonarQube/SonarCloud

### Branch Issues

**Problem**: Branch not found or incorrect data

**Solutions**:

- Verify the branch name is correct (case-sensitive)
- Ensure the branch has been analyzed in SonarQube/SonarCloud
- Check if the branch is a long-lived or short-lived branch
- Use the exact branch name as shown in SonarQube/SonarCloud UI

### Quality Gate Failures

**Problem**: Quality gate fails unexpectedly with `--enforce`

**Solutions**:

- Review the quality gate conditions in the console output
- Check the detailed report to see which conditions failed
- Verify quality gate configuration in SonarQube/SonarCloud
- Consider if the failure is expected (e.g., new issues introduced)

### Report Generation Issues

**Problem**: Report file is not generated or is empty

**Solutions**:

- Check file permissions in the output directory
- Verify the output path is valid and accessible
- Ensure there's enough disk space
- Check the log output for specific error messages

### Validation Failures

**Problem**: Self-validation tests fail

**Solutions**:

- Update to the latest version of SonarMark
- Check if there are any known issues in the GitHub repository
- Report the issue with full validation output if problem persists

### Performance Issues

**Problem**: SonarMark takes too long to execute

**Solutions**:

- Check network latency to the SonarQube/SonarCloud server
- Verify the server is responsive (not overloaded)
- Consider caching results if running frequently
- For large projects, be patient as data retrieval may take time

### Exit Codes

SonarMark uses the following exit codes:

- `0`: Success (or quality gate passed with `--enforce`)
- `1`: Error occurred or quality gate failed (with `--enforce`)

Use these exit codes in scripts for error handling:

```bash
#!/bin/bash
if sonarmark --server https://sonarcloud.io \
  --project-key my-org_my-project \
  --enforce; then
  echo "Quality gate passed!"
else
  echo "Quality gate failed!"
  exit 1
fi
```

## Additional Resources

- [GitHub Repository][github]
- [Issue Tracker][issues]
- [Security Policy][security]
- [SonarQube Documentation][sonarqube-docs]
- [SonarCloud Documentation][sonarcloud-docs]

[dotnet-download]: https://dotnet.microsoft.com/download
[github]: https://github.com/demaconsulting/SonarMark
[issues]: https://github.com/demaconsulting/SonarMark/issues
[security]: https://github.com/demaconsulting/SonarMark/blob/main/SECURITY.md
[sonarqube-docs]: https://docs.sonarqube.org/latest/
[sonarcloud-docs]: https://docs.sonarcloud.io/
