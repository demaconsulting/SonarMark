# ReviewMark Usage Standard

## Purpose

ReviewMark manages file review status enforcement and formal review processes. It tracks which files need
review, organizes them into review-sets, and generates review plans and reports.

## Key Commands

- **Lint Configuration**: `dotnet reviewmark --lint`
- **Elaborate Review-Set**: `dotnet reviewmark --elaborate [review-set]`
- **Generate Plan**: `dotnet reviewmark --plan docs/code_review_plan/plan.md`
- **Generate Report**: `dotnet reviewmark --report docs/code_review_report/report.md`

## Repository Structure

Required repository items for ReviewMark operation:

- `.reviewmark.yaml` - Configuration for review-sets, file-patterns, and review evidence-source.
- `docs/code_review_plan/` - Review planning artifacts
- `docs/code_review_report/` - Review status reports

# Review Definition Structure

Configure reviews in `.reviewmark.yaml` at repository root:

```yaml
# Patterns identifying all files that require review
needs-review:
  # Include source code (adjust file extensions for your repo)
  - "**/*.cs"           # C# source files
  - "**/*.cpp"          # C++ source files  
  - "**/*.hpp"          # C++ header files
  - "!**/bin/**"        # Generated source in build outputs
  - "!**/obj/**"        # Generated source in build intermediates

  # Include requirement files
  - "requirements.yaml"        # Root requirements file
  - "docs/reqstream/**/*.yaml" # Requirements files

  # Include critical documentation files
  - "README.md"                                 # Root level README
  - "docs/user_guide/**/*.md"                   # User guide
  - "docs/design/**/*.md"                       # Design documentation

# Source of review evidence
evidence-source:
  type: none
```

# Review-Set Organization

Organize review-sets using standard patterns to ensure comprehensive coverage
and consistent review processes:

## [System]-Architecture Review (one per system)

Reviews system architecture and operational validation:

- **Files**: System requirements (`docs/reqstream/{system-name}/{system-name}.yaml`), design introduction
  (`docs/design/introduction.md`), system design (`docs/design/{system-name}/{system-name}.md`),
  integration tests
- **Purpose**: Validates system operates as designed and meets overall requirements
- **Example**: `SomeSystem-Architecture`

## [System]-Design Review

Reviews architectural and design consistency:

- **Files**: System requirements, platform requirements, all design documents under `docs/design/`
- **Purpose**: Ensures design completeness and architectural coherence
- **Example**: `SomeSystem-Design`

## [System]-AllRequirements Review

Reviews requirements quality and traceability:

- **Files**: All requirement files including root `requirements.yaml` and all files under `docs/reqstream/{system-name}/`
- **Purpose**: Validates requirements structure, IDs, justifications, and test linkage
- **Example**: `SomeSystem-AllRequirements`

## [System]-[Subsystem] Review

Reviews subsystem architecture and interfaces:

- **Files**: Subsystem requirements, design documents, integration tests (usually no source code)
- **Purpose**: Validates subsystem behavior and interface compliance
- **File Path Pattern**:
  - Requirements: `docs/reqstream/{system-name}/{subsystem-name}/{subsystem-name}.yaml`
  - Design: `docs/design/{system-name}/{subsystem-name}/{subsystem-name}.md`
  - Tests: `test/{SystemName}.Tests/{SubsystemName}/{SubsystemName}*` or similar
- **Example**: `SomeSystem-Authentication`, `SomeSystem-DataLayer`

## [System]-[Subsystem]-[Unit] Review

Reviews individual software unit implementation:

- **Files**: Unit requirements, design documents, source code, unit tests
- **Purpose**: Validates unit meets requirements and is properly implemented
- **File Path Pattern**:
  - Requirements: `docs/reqstream/{system-name}/{subsystem-name}/{unit-name}.yaml` or `docs/reqstream/{system-name}/{unit-name}.yaml`
  - Design: `docs/design/{system-name}/{subsystem-name}/{unit-name}.md` or `docs/design/{system-name}/{unit-name}.md`
  - Source: `src/{SystemName}/{SubsystemName}/{UnitName}.cs`
  - Tests: `test/{SystemName}.Tests/{SubsystemName}/{UnitName}Tests.cs`
- **Example**: `SomeSystem-Authentication-PasswordValidator`, `SomeSystem-DataLayer-ConfigParser`

# Quality Checks

Before submitting ReviewMark configuration, verify:

- [ ] `.reviewmark.yaml` exists at repository root with proper structure
- [ ] `needs-review` patterns cover requirements, design, code, and tests with proper exclusions
- [ ] Each review-set has unique `id` and groups architecturally related files
- [ ] File patterns use correct glob syntax and match intended files
- [ ] File paths reflect current naming conventions (kebab-case design/requirements folders, PascalCase source folders)
- [ ] Evidence source properly configured (`none` for dev, `url` for production)
- [ ] Environment variables used for credentials (never hardcoded)
- [ ] Generated documents accessible for compliance auditing
- [ ] Review-set organization follows standard patterns ([System]-[Subsystem], [System]-Design, etc.)
