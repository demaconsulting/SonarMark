# Agent Quick Reference

Comprehensive guidance for AI agents working on repositories following Continuous Compliance practices.

# Project Structure

The following is the basic folder structure of the project. Agents should use this information when searching for
existing files and to know where to make new files.

```text
├── docs/
│   ├── build_notes/
│   ├── code_quality/
│   ├── code_review_plan/
│   ├── code_review_report/
│   ├── design/
│   ├── requirements_doc/
│   ├── requirements_report/
│   └── reqstream/
├── src/
│   └── <project>/
└── test/
    └── <test-project>/
```

# Key Configuration Files

- **`.config/dotnet-tools.json`** - Local tool manifest for Continuous Compliance tools
- **`.editorconfig`** - Code formatting rules
- **`.clang-format`** - C/C++ formatting (if applicable)
- **`.cspell.yaml`** - Spell-check configuration and technical term dictionary
- **`.markdownlint-cli2.yaml`** - Markdown linting rules
- **`.yamllint.yaml`** - YAML linting configuration
- **`.reviewmark.yaml`** - File review definitions and tracking
- **`nuget.config`** - NuGet package sources (if .NET)
- **`package.json`** - Node.js dependencies for linting tools
- **`requirements.yaml`** - Root requirements file with includes
- **`pip-requirements.txt`** - Python dependencies for yamllint
- **`lint.sh` / `lint.bat`** - Cross-platform comprehensive linting scripts

# Standards Application (ALL Agents Must Follow)

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

# Agent Delegation Guidelines

The default agent should handle simple, straightforward tasks directly.
Delegate to specialized agents only for specific scenarios:

- **Light development work** (small fixes, simple features) → Call the developer agent
- **Light quality checking** (linting, basic validation) → Call the quality agent
- **Formal feature implementation** (complex, multi-step) → Call the implementation agent
- **Formal bug resolution** (complex debugging, systematic fixes) → Call the implementation agent
- **Formal reviews** (compliance verification, detailed analysis) → Call the code-review agent
- **Template consistency** (downstream repository alignment) → Call the repo-consistency agent

## Available Specialized Agents

- **developer** - General-purpose software development agent that applies appropriate
  standards based on the work being performed
- **code-review** - Agent for performing formal reviews using standardized review processes
- **implementation** - Orchestrator agent that manages quality implementations
  through a formal state machine workflow
- **quality** - Quality assurance agent that grades developer work against DEMA
  Consulting standards and Continuous Compliance practices
- **repo-consistency** - Ensures downstream repositories remain consistent with
  the TemplateDotNetTool template patterns and best practices

# Linting (Required Before Quality Gates)

1. **Markdown Auto-fix**: `npx markdownlint-cli2 --fix **/*.md` (fixes most markdown issues except line length)
2. **Dotnet Auto-fix**: `dotnet format` (reformats .NET languages)
3. **Run full check**: `lint.bat` (Windows) or `lint.sh` (Unix)  
4. **Fix remaining**: Address line length, spelling, YAML syntax manually
5. **Verify clean**: Re-run until 0 errors before quality validation

## Linting Tools (ALL Must Pass)

- **markdownlint-cli2**: Markdown style and formatting enforcement
- **cspell**: Spell-checking across all text files (use `.cspell.yaml` for technical terms)
- **yamllint**: YAML structure and formatting validation
- **Language-specific linters**: Based on repository technology stack

# Quality Gate Enforcement (ALL Agents Must Verify)

Configuration files and scripts are self-documenting with their design intent and
modification policies in header comments.

1. **Build Quality**: Zero warnings (`TreatWarningsAsErrors=true`)
2. **Static Analysis**: SonarQube/CodeQL passing with no blockers
3. **Requirements Traceability**: `dotnet reqstream --enforce` passing
4. **Test Coverage**: All requirements linked to passing tests
5. **Documentation Currency**: All docs current and generated
6. **File Review Status**: All reviewable files have current reviews

# Continuous Compliance Overview

This repository follows the DEMA Consulting Continuous Compliance
<https://github.com/demaconsulting/ContinuousCompliance> approach, which enforces quality and
compliance gates on every CI/CD run instead of as a last-mile activity.

## Core Principles

- **Requirements Traceability**: Every requirement MUST link to passing tests
- **Quality Gates**: All quality checks must pass before merge
- **Documentation Currency**: All docs auto-generated and kept current
- **Automated Evidence**: Full audit trail generated with every build

## Requirements & Compliance

- **ReqStream**: Requirements traceability enforcement (`dotnet reqstream --enforce`)
- **ReviewMark**: File review status enforcement
- **BuildMark**: Tool version documentation
- **VersionMark**: Version tracking across CI/CD jobs
