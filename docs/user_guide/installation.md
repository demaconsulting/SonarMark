# Installation

## Continuous Compliance

SonarMark follows the [Continuous Compliance][continuous-compliance] methodology, ensuring
compliance evidence is generated automatically on every CI run.

Key practices include:

- **Requirements Traceability**: Every requirement is linked to passing tests, and a trace matrix is
  auto-generated on each release
- **Linting Enforcement**: markdownlint, cspell, and yamllint are enforced before any build proceeds
- **Automated Audit Documentation**: Each release ships with generated requirements, justifications,
  trace matrix, and quality reports
- **CodeQL and SonarCloud**: Security and quality analyses run on every build

## Prerequisites

- [.NET SDK][dotnet-download] 8.0, 9.0, or 10.0

## Global Installation

Install SonarMark as a global .NET tool for system-wide access:

```bash
dotnet tool install --global DemaConsulting.SonarMark
```

Verify the installation:

```bash
sonarmark --version
```

## Local Installation

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

## Update

To update to the latest version:

```bash
# Global installation
dotnet tool update --global DemaConsulting.SonarMark

# Local installation
dotnet tool update DemaConsulting.SonarMark
```

[continuous-compliance]: https://github.com/demaconsulting/ContinuousCompliance
[dotnet-download]: https://dotnet.microsoft.com/download
