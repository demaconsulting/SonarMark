# SonarMark Requirements

## Command-Line Interface

### CLI-001

**The tool shall provide a command-line interface.**

A command-line interface is essential for automation and integration into CI/CD pipelines.
It allows users to incorporate SonarMark into their build processes without manual interaction,
enabling automated quality checks and reporting as part of the development workflow.


### CLI-002

**The tool shall display version information when requested.**

Version information helps users verify which version of the tool they are running, which is
critical for troubleshooting, ensuring compatibility, and tracking which features are available.
This is a standard expectation for command-line tools.


### CLI-003

**The tool shall display help information when requested.**

Help information provides users with usage instructions and available options without needing
to consult external documentation. This improves usability and reduces the learning curve,
especially for new users or those who need a quick reference.


### CLI-004

**The tool shall support silent mode to suppress console output.**

Silent mode is important for automated environments where console output may interfere with
log parsing or where minimal output is preferred. It allows the tool to run without cluttering
build logs while still performing its analysis and reporting functions.


### CLI-005

**The tool shall support writing output to a log file.**

Log file output enables persistent recording of tool execution for later review and audit trails.
This is particularly valuable for debugging issues, maintaining compliance records, and analyzing
trends in code quality over time without relying on transient console output.


### CLI-006

**The tool shall support enforcing quality gate checks.**

Quality gate enforcement allows the tool to fail builds when quality standards are not met,
preventing low-quality code from progressing through the pipeline. This is a critical feature
for maintaining code quality standards and preventing technical debt accumulation.


## SonarQube/SonarCloud Integration

### SONAR-001

**The tool shall connect to SonarQube/SonarCloud servers.**

Connection to SonarQube/SonarCloud servers is the core functionality of the tool, enabling
retrieval of code quality metrics, analysis results, and quality gate status. Without this
capability, the tool cannot fulfill its primary purpose of integrating Sonar analysis into
development workflows.


### SONAR-002

**The tool shall authenticate with SonarQube/SonarCloud using tokens.**

Token-based authentication is the secure and recommended method for accessing SonarQube/SonarCloud
APIs. It provides better security than password-based authentication and is essential for
automated environments where interactive authentication is not possible.


### SONAR-003

**The tool shall fetch quality gate status from SonarQube/SonarCloud.**

Quality gate status indicates whether code meets the defined quality standards and is fundamental
to the tool's enforcement capabilities. This information is essential for making pass/fail
decisions in CI/CD pipelines and providing visibility into overall code quality.


### SONAR-004

**The tool shall fetch issues from SonarQube/SonarCloud.**

Issues represent code quality problems identified by Sonar analysis. Fetching and reporting
these issues allows developers to understand what needs to be fixed and provides the detailed
information needed to improve code quality beyond just pass/fail status.


### SONAR-005

**The tool shall fetch security hot-spots from SonarQube/SonarCloud.**

Security hot-spots highlight code that requires security review, helping teams identify and
address potential security vulnerabilities. This is critical for maintaining secure applications
and meeting security compliance requirements.


### SONAR-006

**The tool shall support filtering by project key.**

Project key filtering is essential for identifying which specific project to analyze when
multiple projects exist on a Sonar server. This parameter is mandatory for the tool to
retrieve the correct analysis results for the intended codebase.


### SONAR-007

**The tool shall support filtering by branch.**

Branch filtering enables analysis of specific branches in version control, which is crucial
for multi-branch development workflows. This allows teams to check quality gates for feature
branches, pull requests, and release branches independently.


## Report Generation

### RPT-001

**The tool shall generate markdown reports.**

Markdown reports provide human-readable documentation of code quality that can be easily
integrated into documentation systems, pull request comments, and version control. Markdown
is widely supported and readable both as plain text and rendered, making it ideal for
developer communication and documentation.


### RPT-002

**The tool shall support configurable report depth.**

Configurable report depth allows users to control the level of detail in generated reports,
accommodating different use cases from high-level summaries to detailed analysis. This
flexibility ensures reports can be integrated into various document structures without
conflicting heading levels.


### RPT-003

**The tool shall include quality gate status in reports.**

Quality gate status is the most important metric in the report, providing a clear pass/fail
indication of code quality. Including this information with condition details helps teams
understand whether their code meets quality standards and which metrics need improvement.


### RPT-004

**The tool shall categorize and report issues by type and severity.**

Categorizing issues by type and severity helps teams prioritize remediation efforts and
understand the nature of quality problems in their codebase. This organization makes it
easier to address the most critical issues first and track specific categories of problems.


### RPT-005

**The tool shall report security hot-spots requiring review.**

Security hot-spots require manual review to determine if they represent actual vulnerabilities.
Reporting these separately from standard issues ensures they receive appropriate security-focused
attention and helps teams maintain secure coding practices.


## Validation and Testing

### VAL-001

**The tool shall support self-validation mode.**

Self-validation mode allows the tool to verify its own functionality against a live Sonar
server, ensuring it works correctly and can retrieve and process data as expected. This
capability is essential for testing the tool itself and providing confidence in its operation
before using it in production pipelines.


### VAL-002

**The tool shall write validation results to test result files.**

Writing validation results to test result files allows integration with standard testing
frameworks and CI/CD tools. This enables validation outcomes to be tracked, reported, and
integrated into build pipelines alongside other test results.


### VAL-003

**The tool shall support TRX format for test results.**

TRX (Test Results XML) is the native test results format for Microsoft testing tools and
Visual Studio. Supporting this format ensures compatibility with .NET development workflows
and enables seamless integration with Azure DevOps and other Microsoft-based CI/CD systems.


### VAL-004

**The tool shall support JUnit format for test results.**

JUnit format is a widely-supported standard for test results across many platforms and tools,
including Jenkins, GitLab CI, and GitHub Actions. Supporting this format ensures broad
compatibility with diverse CI/CD environments beyond Microsoft ecosystems.


## Quality Enforcement

### ENF-001

**The tool shall support enforcement mode to fail on quality gate failures.**

Enforcement mode is critical for maintaining code quality standards by preventing builds
from succeeding when quality gates fail. This capability enables teams to establish and
maintain quality policies that must be met before code can be merged or deployed.


### ENF-002

**The tool shall return non-zero exit code when quality gate fails in enforcement mode.**

Returning a non-zero exit code on quality gate failure is the standard mechanism for
indicating failure in command-line tools and CI/CD systems. This allows build systems
to automatically detect and halt builds when quality standards are not met.


## Platform Support

### PLT-001

**The tool shall run on Windows operating systems.**

Windows is a major development platform, especially for .NET development. Supporting Windows
ensures the tool can be used in Windows-based development environments and CI/CD systems,
which are common in enterprise .NET development shops.


### PLT-002

**The tool shall run on Linux operating systems.**

Linux is the dominant platform for cloud-based CI/CD systems and containerized environments.
Supporting Linux ensures the tool can be used in modern DevOps pipelines, Docker containers,
and popular CI platforms like GitHub Actions, GitLab CI, and Jenkins.


### PLT-003

**The tool shall support .NET 8.0 runtime.**

.NET 8.0 is a Long-Term Support (LTS) release with support until November 2026. Supporting
this version ensures compatibility with enterprise environments that standardize on LTS
releases for stability and long-term support guarantees.


### PLT-004

**The tool shall support .NET 9.0 runtime.**

.NET 9.0 is a Standard Term Support (STS) release providing access to the latest features
and performance improvements. Supporting this version allows users who adopt newer .NET
versions to use the tool without compatibility concerns.


### PLT-005

**The tool shall support .NET 10.0 runtime.**

.NET 10.0 is the next Long-Term Support (LTS) release scheduled for November 2025. Supporting
this version ensures the tool remains compatible with the latest LTS release, providing users
with a future-proof solution and access to the newest platform capabilities.


