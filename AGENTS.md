# Agent Quick Reference

Comprehensive guidance for AI agents working on repositories following Continuous Compliance practices.

## Standards Application (ALL Agents Must Follow)

Before performing any work, agents must read and apply the relevant standards from `.github/standards/`:

- **`csharp-language.md`** - For C# code development (literate programming, XML docs, dependency injection)
- **`csharp-testing.md`** - For C# test development (AAA pattern, naming, MSTest anti-patterns)
- **`design-documentation.md`** - For design documentation (software structure diagrams, system.md, subsystem organization)
- **`reqstream-usage.md`** - For requirements management (traceability, semantic IDs, source filters)
- **`reviewmark-usage.md`** - For file review management (review-sets, file patterns, enforcement)
- **`software-items.md`** - For software categorization (system/subsystem/unit/OTS classification)
- **`technical-documentation.md`** - For documentation creation and maintenance (structure, Pandoc, README best practices)

Load only the standards relevant to your specific task scope and apply their
quality checks and guidelines throughout your work.

## Agent Delegation Guidelines

The default agent should handle simple, straightforward tasks directly.
Delegate to specialized agents only for specific scenarios:

- **Light development work** (small fixes, simple features) → Call developer agent
- **Light quality checking** (linting, basic validation) → Call quality agent
- **Formal feature implementation** (complex, multi-step) → Call the `implementation` agent
- **Formal bug resolution** (complex debugging, systematic fixes) → Call the `implementation` agent
- **Formal reviews** (compliance verification, detailed analysis) → Call code-review agent
- **Template consistency** (downstream repository alignment) → Call repo-consistency agent

## Available Specialized Agents

- **code-review** - Agent for performing formal reviews using standardized
  review processes
- **developer** - General-purpose software development agent that applies
  appropriate standards based on the work being performed
- **implementation** - Orchestrator agent that manages quality implementations
  through a formal state machine workflow
- **quality** - Quality assurance agent that grades developer work against DEMA
  Consulting standards and Continuous Compliance practices
- **repo-consistency** - Ensures downstream repositories remain consistent with
  the TemplateDotNetTool template patterns and best practices

## Quality Gate Enforcement (ALL Agents Must Verify)

Configuration files and scripts are self-documenting with their design intent and
modification policies in header comments.

1. **Linting Standards**: `./lint.sh` (Unix) or `lint.bat` (Windows) - comprehensive linting suite
2. **Build Quality**: Zero warnings (`TreatWarningsAsErrors=true`)
3. **Static Analysis**: SonarQube/CodeQL passing with no blockers
4. **Requirements Traceability**: `dotnet reqstream --enforce` passing
5. **Test Coverage**: All requirements linked to passing tests
6. **Documentation Currency**: All docs current and generated
7. **File Review Status**: All reviewable files have current reviews

## Continuous Compliance Overview

This repository follows the DEMA Consulting Continuous Compliance
<https://github.com/demaconsulting/ContinuousCompliance> approach, which enforces quality and
compliance gates on every CI/CD run instead of as a last-mile activity.

### Core Principles

- **Requirements Traceability**: Every requirement MUST link to passing tests
- **Quality Gates**: All quality checks must pass before merge
- **Documentation Currency**: All docs auto-generated and kept current
- **Automated Evidence**: Full audit trail generated with every build

## Required Compliance Tools

### Linting Tools (ALL Must Pass)

- **markdownlint-cli2**: Markdown style and formatting enforcement
- **cspell**: Spell-checking across all text files (use `.cspell.yaml` for technical terms)
- **yamllint**: YAML structure and formatting validation
- **Language-specific linters**: Based on repository technology stack

### Quality Analysis

- **SonarQube/SonarCloud**: Code quality and security analysis
- **CodeQL**: Security vulnerability scanning (produces SARIF output)
- **Static analyzers**: Microsoft.CodeAnalysis.NetAnalyzers, SonarAnalyzer.CSharp, etc.

### Requirements & Compliance

- **ReqStream**: Requirements traceability enforcement (`dotnet reqstream --enforce`)
- **ReviewMark**: File review status enforcement
- **BuildMark**: Tool version documentation
- **VersionMark**: Version tracking across CI/CD jobs

## Project Structure Template

- `docs/` - Documentation and compliance artifacts
  - `design/` - Detailed design documents
    - `introduction.md` - System/Subsystem/Unit breakdown for this repository
  - `reqstream/` - Subsystem requirements YAML files (included by root requirements.yaml)
  - Auto-generated reports (requirements, justifications, trace matrix)
- `src/{ProjectName}/` - Source code projects
- `test/{ProjectName}.Tests/` - Test projects
- `.github/workflows/` - CI/CD pipeline definitions (build.yaml, build_on_push.yaml, release.yaml)
- Configuration files: `.editorconfig`, `.clang-format`, `nuget.config`, `.reviewmark.yaml`, etc.

## Key Configuration Files

### Essential Files (Repository-Specific)

- **`lint.sh` / `lint.bat`** - Cross-platform comprehensive linting scripts
- **`.editorconfig`** - Code formatting rules
- **`.clang-format`** - C/C++ formatting (if applicable)
- **`.cspell.yaml`** - Spell-check configuration and technical term dictionary
- **`.markdownlint-cli2.yaml`** - Markdown linting rules
- **`.yamllint.yaml`** - YAML linting configuration
- **`nuget.config`** - NuGet package sources (if .NET)
- **`package.json`** - Node.js dependencies for linting tools

### Compliance Files

- **`requirements.yaml`** - Root requirements file with includes
- **`.reviewmark.yaml`** - File review definitions and tracking
- CI/CD pipeline files with quality gate enforcement

## Continuous Compliance Workflow

### CI/CD Pipeline Stages (Standard)

1. **Lint**: `./lint.sh` or `lint.bat` - comprehensive linting suite
2. **Build**: Compile with warnings as errors
3. **Analyze**: SonarQube/SonarCloud, CodeQL security scanning
4. **Test**: Execute all tests, generate coverage reports
5. **Validate**: Tool self-validation tests
6. **Document**: Generate requirements reports, trace matrix, build notes
7. **Enforce**: Requirements traceability, file review status
8. **Publish**: Generate final documentation (Pandoc → PDF)

### Quality Gate Enforcement

All stages must pass before merge. Pipeline fails immediately on:

- Any linting errors
- Build warnings or errors
- Security vulnerabilities (CodeQL)
- Requirements without test coverage
- Outdated file reviews
- Missing documentation

## Continuous Compliance Requirements

This repository follows continuous compliance practices from DEMA Consulting
Continuous Compliance <https://github.com/demaconsulting/ContinuousCompliance>.

### Core Requirements Traceability Rules

- **ALL requirements MUST be linked to tests** - Enforced in CI via `dotnet reqstream --enforce`
- **NOT all tests need requirement links** - Tests may exist for corner cases, design validation, failure scenarios
- **Source filters are critical** - Platform/framework requirements need specific test evidence

For detailed requirements format, test linkage patterns, and ReqStream
integration, call the developer agent with requirements management context.

## Agent Report Files

Upon completion, create a report file at `.agent-logs/{agent-name}-{subject}-{unique-id}.md` that includes:

- A concise summary of the work performed
- Any important decisions made and their rationale
- Follow-up items, open questions, or TODOs

Store agent logs in the `.agent-logs/` folder so they are ignored via `.gitignore` and excluded from linting and commits.
