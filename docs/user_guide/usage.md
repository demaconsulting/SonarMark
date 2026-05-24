# Usage

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

# Passing a token from an environment variable
export SONAR_TOKEN=squ_abc123...
sonarmark --server https://sonarcloud.io \
  --project-key my-org_my-project \
  --token $SONAR_TOKEN
```

### Report Generation

#### `--report <file>`

Export quality results to a markdown file. The file will contain quality gate status, conditions,
issues, and security hot-spots.

```bash
sonarmark --server https://sonarcloud.io \
  --project-key my-org_my-project \
  --report quality-report.md
```

#### `--depth <depth>`

Set the markdown header depth for the report. Default is 1. Use this when embedding the report in
larger documents.

> **Note:** `--report-depth` is a deprecated alias for `--depth` and is kept for backwards
> compatibility.

```bash
# Use level 2 headers (##) instead of level 1 (#)
sonarmark --server https://sonarcloud.io \
  --project-key my-org_my-project \
  --report quality-report.md \
  --depth 2
```

### Quality Enforcement

#### `--enforce`

Return a non-zero exit code if the quality gate fails. Essential for CI/CD pipelines to fail
builds on quality issues.

```bash
sonarmark --server https://sonarcloud.io \
  --project-key my-org_my-project \
  --enforce
```

### Validation Options

#### `--validate`

Run built-in self-validation tests. These tests verify SonarMark functionality without requiring
access to a real SonarQube/SonarCloud server.

```bash
sonarmark --validate
```

#### `--results <file>`

Write validation results to a file. Supports TRX (`.trx`) and JUnit XML (`.xml`) formats.
Requires `--validate`.

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

Generate timestamped quality reports on a schedule:

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

The generated markdown report includes the following sections.

### Project Header

The report begins with the project name and a link to the SonarQube/SonarCloud dashboard:

```markdown
# Example Project Sonar Analysis

**Dashboard:** <https://sonarcloud.io/dashboard?id=my_project>
```

### Quality Gate Status

Shows whether the project passed or failed the quality gate. Possible values are OK, ERROR, WARN,
or NONE:

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

If no issues are found the section reads `Found no issues`.

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
- **Security Category**: The type of security issue in brackets (e.g., sql-injection,
  weak-cryptography)
- **Message**: Description of the security concern

If no security hot-spots are found the section reads `Found no security hot-spots`.

## Self-Validation

Self-validation produces a report demonstrating that SonarMark is functioning correctly. This is
useful in regulated industries where tool validation evidence is required.

### Running Validation

To perform self-validation:

```bash
sonarmark --validate
```

To save validation results to a file:

```bash
sonarmark --validate --results results.trx
```

The results file format is determined by the file extension: `.trx` for TRX (MSTest) format,
or `.xml` for JUnit format.

### Validation Report

The validation report contains the tool version, machine name, operating system version, .NET
runtime version, timestamp, and test results.

Example validation report:

```text
# DEMA Consulting SonarMark

| Information         | Value                                              |
| :------------------ | :------------------------------------------------- |
| SonarMark Version   | 1.0.0                                              |
| Machine Name        | BUILD-SERVER                                       |
| OS Version          | Ubuntu 22.04.3 LTS                                 |
| DotNet Runtime      | .NET 10.0.0                                        |
| Time Stamp          | 2024-01-15 10:30:00 UTC                            |

✓ SonarMark_QualityGateRetrieval - Passed
✓ SonarMark_IssuesRetrieval - Passed
✓ SonarMark_HotSpotsRetrieval - Passed
✓ SonarMark_MarkdownReportGeneration - Passed

Total Tests: 4
Passed: 4
Failed: 0
```

### Validation Tests

Each test proves specific functionality works correctly:

- **`SonarMark_QualityGateRetrieval`** - Verifies fetching and processing quality gate status from
  SonarQube/SonarCloud.
- **`SonarMark_IssuesRetrieval`** - Verifies fetching and processing code issues with severity
  classification.
- **`SonarMark_HotSpotsRetrieval`** - Verifies fetching and processing security hot-spots and
  vulnerabilities.
- **`SonarMark_MarkdownReportGeneration`** - Verifies generating markdown reports with quality
  metrics and findings.

## Best Practices

### Authentication

- **Store tokens securely**: Use environment variables or secret management systems
- **Rotate tokens regularly**: Follow your organization's security policies
- **Use read-only tokens**: SonarMark only needs read access to the API
- **Don't commit tokens**: Never commit tokens to version control

### CI/CD

- **Use enforcement mode**: Always use `--enforce` in CI/CD to fail builds on quality gate failures
- **Archive reports**: Save quality reports as build artifacts for historical tracking
- **Set timeouts**: Configure reasonable timeouts for API calls in CI/CD environments
- **Handle failures gracefully**: Use appropriate error handling in your CI/CD scripts

### Reports

- **Use meaningful filenames**: Include timestamps, branch names, or build numbers in report filenames
- **Adjust header depth**: Use `--depth` when embedding reports in larger documents
- **Combine with logging**: Use `--log` to capture detailed execution information

### Performance

- **Cache dependencies**: Use local tool installation to speed up execution in CI/CD
- **Minimize API calls**: Only fetch data when needed (e.g., don't generate reports if not required)
- **Use silent mode**: Suppress unnecessary output in automated scripts with `--silent`
