# GitHub Copilot Agents

This document provides comprehensive guidance for GitHub Copilot agents working on the SonarMark project.

## Overview

GitHub Copilot agents are AI-powered assistants that help with various development tasks. This document will be
updated as agents are configured for this repository.

## Project Overview

SonarMark is a .NET command-line tool for creating code quality reports from SonarQube/SonarCloud analysis results.
It provides functionality to generate markdown reports that summarize code quality metrics, issues, and violations.

### Technology Stack

- **Language**: C# 12
- **Framework**: .NET 8.0, 9.0, and 10.0
- **Testing Framework**: MSTest
- **Build System**: dotnet CLI
- **Package Manager**: NuGet

## Project Structure

```text
SonarMark/
├── .config/                      # Dotnet tools configuration
│   └── dotnet-tools.json         # Local tool manifest
├── .github/                      # GitHub Actions workflows
│   ├── agents/                   # Agent configurations
│   ├── ISSUE_TEMPLATE/           # Issue templates
│   └── workflows/
│       ├── build.yaml            # Reusable build workflow
│       └── build_on_push.yaml    # Main CI/CD pipeline
├── docs/                         # Documentation
│   ├── guide/                    # User guide
│   ├── requirements/             # Requirements documentation
│   └── tracematrix/              # Trace matrix documentation
├── src/                          # Source code
│   └── DemaConsulting.SonarMark/ # Main application project
├── test/                         # Test projects
│   └── DemaConsulting.SonarMark.Tests/ # Test project
├── .cspell.json                  # Spell checking configuration
├── .editorconfig                 # Code style configuration
├── .markdownlint.json            # Markdown linting rules
├── .yamllint.yaml                # YAML linting rules
├── AGENTS.md                     # This file
├── CODE_OF_CONDUCT.md            # Code of Conduct
├── LICENSE                       # MIT License
├── README.md                     # Project documentation
├── requirements.yaml             # Requirements definitions
└── SECURITY.md                   # Security policy
```

### Critical Files

- **`.editorconfig`**: Defines code style rules, naming conventions, and formatting standards
- **`.cspell.json`**: Contains spell-checking configuration and custom dictionary
- **`.markdownlint.json`**: Markdown linting rules
- **`.yamllint.yaml`**: YAML linting rules
- **`DemaConsulting.SonarMark.sln`**: Solution file containing all projects
- **`requirements.yaml`**: Requirements definitions with test mappings

## Testing Guidelines

- **Test Framework**: MSTest v4 (Microsoft.VisualStudio.TestTools.UnitTesting)
- **Test File Naming**: `[Component]Tests.cs` (e.g., `ProgramTests.cs`)
- **Test Method Naming**: `ClassName_MethodUnderTest_Scenario_ExpectedBehavior` format
  - Example: `Program_Main_NoArguments_ReturnsSuccess` clearly indicates testing the `Program.Main` method
  - This pattern makes test intent clear for requirements traceability
- **Requirements Traceability**: All requirements in `requirements.yaml` should be linked to tests
  - Integration tests are preferred for requirements evidence
  - Unit tests may be used if no integration test provides appropriate evidence
  - Integration tests use Test Source Linking format (`filepart@testname`) for platform/runtime requirements
  - Example: `windows-latest@IntegrationTest_VersionFlag_OutputsVersion` links a Windows-specific test
  - Example: `dotnet8.x@IntegrationTest_ReportGeneration_CreatesMarkdownFile` links a .NET 8 test
- **MSTest v4 APIs**: Use modern assertions:
  - `Assert.HasCount(collection, expectedCount)` instead of `Assert.AreEqual(count, collection.Count)`
  - `Assert.IsEmpty(collection)` instead of `Assert.AreEqual(0, collection.Count)`
  - `Assert.DoesNotContain(item, collection)` for negative checks
- **Console Testing**: Save and restore `Console.Out` in tests that modify console output:

  ```csharp
  var originalOut = Console.Out;
  try { /* test code */ }
  finally { Console.SetOut(originalOut); }
  ```

- **All tests must pass** before merging changes
- **No warnings allowed** in test builds

## Requirements Management

- **Requirements Tool**: DemaConsulting.ReqStream is used for requirements management
- **Requirements File**: `requirements.yaml` defines all project requirements
- **Trace Matrix**: Requirements are traced to tests using the ReqStream tool
- **Documentation**: Requirements and trace matrix PDFs are generated as part of the build process
- **Enforcement**: The `--enforce` flag ensures all requirements have test coverage
- **Test Naming**: Tests must use the naming pattern that clearly indicates what they test for traceability

## Code Style and Conventions

### Naming Conventions

- **Interfaces**: Must begin with `I` (e.g., `IReportGenerator`)
- **Classes, Structs, Enums**: PascalCase
- **Methods, Properties**: PascalCase
- **Parameters, Local Variables**: camelCase

### Code Organization

- **Namespace Declarations**: Use file-scoped namespaces (C# 10+)
- **Using Directives**: Sort system directives first
- **Braces**: Required for all control statements
- **Indentation**: 4 spaces for C#, 2 spaces for YAML/JSON/XML
- **Encoding**: UTF-8 with BOM, LF line endings with final newline

### Documentation and Comments

- **Copyright Headers**: All source files must include the MIT license header
- **XML Documentation**: Use triple-slash comments (`///`) for all public, internal, and private members
  - **IMPORTANT**: Summary blocks must be indented with spaces after `///`

  ```csharp
  /// <summary>
  ///     This is the correct indentation format for summary blocks.
  /// </summary>
  ```

- **Error Handling Patterns**:
  - Argument parsing: Throw `ArgumentException` with descriptive messages
  - Runtime errors during execution: Use `InvalidOperationException`
  - Write methods (WriteLine/WriteError) are for output AFTER successful parsing
- **Code Reusability**: Create properties or methods to avoid code duplication

## Quality Standards

- **Static Analysis**: Built-in .NET analyzers enforce code style, naming rules, and nullable reference types
- **Documentation**:
  - README.md uses absolute URLs (included in NuGet package)
  - Other markdown files use link references: `[text][ref]` with `[ref]: url` at end
- **Linting**:
  - **Markdown**: Must pass markdownlint (max line length: 120 chars)
    - Lists must be surrounded by blank lines (MD032)
    - Run locally: Check CI workflow for markdownlint-cli2-action usage
  - **Spell Check**: Must pass cspell (custom dictionary in `.cspell.json`)
    - Add project-specific terms to the custom dictionary if needed
  - **YAML**: Must pass yamllint (2-space indentation, max line length: 120 chars)
  - **All linting must pass locally before committing** - CI will reject changes with linting errors

## CI/CD Pipelines

The project uses GitHub Actions workflows in `.github/workflows/`:

- **build_on_push.yaml**: Runs quality checks, builds on Windows and Linux
- **build.yaml**: Reusable workflow for restore, build, test, and package

Build commands:

```bash
dotnet tool restore    # Restore dotnet tools
dotnet restore         # Restore dependencies
dotnet build --no-restore --configuration Release
dotnet test --no-build --configuration Release --verbosity normal
dotnet pack --no-build --configuration Release
```

## Pre-Finalization Quality Checks

Before completing any task, you **MUST** perform these checks in order and ensure they all pass:

1. **Build and Test**: Run `dotnet build --configuration Release && dotnet test --configuration Release` - all tests
   must pass with zero warnings
2. **Code Review**: Use `code_review` tool and address all valid concerns
3. **Security Scanning**: Use `codeql_checker` tool after code review - must report zero vulnerabilities
4. **Linting**: Run all linters locally and fix any issues before pushing changes:
   - **Markdown**: Run markdownlint on all changed `.md` files - must pass with zero errors
   - **Spell Check**: Run cspell on all changed files - must pass with zero errors
   - **YAML**: Run yamllint on all changed `.yaml` or `.yml` files - must pass with zero errors
   - These linters run in CI and will fail the build if not passing

## Project-Specific Guidelines

### Key Decisions from Recent Reviews

These patterns emerged from code review feedback and should be followed:

1. **XmlDoc Formatting**: Always indent summary content with spaces after `///`
2. **Error Handling**: Throw exceptions during parsing; use Write methods only after successful parsing
3. **Code Reuse**: Extract repeated code into properties or methods
4. **Console Testing**: Always save/restore `Console.Out` in try/finally blocks
5. **Modern Test APIs**: Prefer MSTest v4 assertions (HasCount, IsEmpty, DoesNotContain)

### What NOT to Do

- Don't delete or modify working code unless fixing a security vulnerability
- Don't remove or modify existing tests unless directly related to your changes
- Don't commit build artifacts (`bin/`, `obj/`, `node_modules/`)
- Don't use force push (`git reset`, `git rebase`)
- Don't create temporary files in the repository (use `/tmp` instead)
- Don't make changes to `.github/agents/` files (for other agents only)

## Available Agents

The following custom agents are configured for this repository. Each agent file contains detailed guidelines,
responsibilities, and project-specific conventions.

- **Documentation Writer** (`.github/agents/documentation-writer.md`) - Expert agent for creating, updating, and
  maintaining project documentation
- **Project Maintainer** (`.github/agents/project-maintainer.md`) - Expert agent for overall project management,
  dependency updates, CI/CD maintenance, and release coordination
