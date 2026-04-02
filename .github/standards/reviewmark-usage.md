# ReviewMark File Review Standards

This document defines DEMA Consulting standards for managing file reviews using
ReviewMark within Continuous Compliance environments.

# Core Purpose

ReviewMark automates file review tracking using cryptographic fingerprints to
ensure:

- Every file requiring review is covered by a current, valid review
- Reviews become stale when files change, triggering re-review
- Complete audit trail of review coverage for regulatory compliance

# Review Definition Structure

Configure reviews in `.reviewmark.yaml` at repository root:

```yaml
# Patterns identifying all files that require review
needs-review:
  # Include core development artifacts
  - "requirements.yaml"          # Root requirements file
  - "docs/reqstream/**/*.yaml"   # Requirements files
  - "docs/design/**/*.md"        # Design documentation
  - "**/*.cs"                    # All C# source and test files

  # Exclude build output and generated content
  - "!**/obj/**"                 # Exclude build output
  - "!**/bin/**"                 # Exclude binary output
  - "!**/generated/**"           # Exclude auto-generated files

# Source of review evidence
evidence-source:
  type: none

# Named review-sets grouping related files
reviews:
  - id: MyProduct-PasswordValidator
    title: Password Validator Unit Review
    paths:
      - "docs/reqstream/authentication/password-validator.yaml"
      - "docs/design/authentication/password-validator.md"
      - "src/{ProjectName}/Authentication/PasswordValidator.cs"
      - "test/{ProjectName}.Tests/Authentication/PasswordValidatorTests.cs"

  - id: MyProduct-AllRequirements
    title: All Requirements Review
    paths:
      - "requirements.yaml"
      - "docs/reqstream/**/*.yaml"
```

# Review-Set Organization

Organize review-sets using standard patterns to ensure comprehensive coverage
and consistent review processes:

## [Project]-System Review

Reviews system integration and operational validation:

- **Files**: System requirements (`docs/reqstream/system.yaml`), design introduction
  (`docs/design/introduction.md`), system design (`docs/design/system.md`),
  integration tests
- **Purpose**: Validates system operates as designed and meets overall requirements
- **Example**: `TemplateTool-System`

## [Product]-Design Review

Reviews architectural and design consistency:

- **Files**: System requirements, platform requirements, all design documents under `docs/design/`
- **Purpose**: Ensures design completeness and architectural coherence
- **Example**: `MyProduct-Design`

## [Product]-AllRequirements Review

Reviews requirements quality and traceability:

- **Files**: All requirement files including root `requirements.yaml` and all files under `docs/reqstream/`
- **Purpose**: Validates requirements structure, IDs, justifications, and test linkage
- **Example**: `MyProduct-AllRequirements`

## [Product]-[Unit] Review

Reviews individual software unit implementation:

- **Files**: Unit requirements, design documents, source code, unit tests
- **Purpose**: Validates unit meets requirements and is properly implemented
- **File Path Pattern**:
  - Requirements: `docs/reqstream/{subsystem-name}/{unit-name}.yaml` or `docs/reqstream/{unit-name}.yaml`
  - Design: `docs/design/{subsystem-name}/{unit-name}.md` or `docs/design/{unit-name}.md`
  - Source: `src/{ProjectName}/{SubsystemName}/{UnitName}.cs`
  - Tests: `test/{ProjectName}.Tests/{SubsystemName}/{UnitName}Tests.cs`
- **Example**: `MyProduct-PasswordValidator`, `MyProduct-ConfigParser`

## [Product]-[Subsystem] Review

Reviews subsystem architecture and interfaces:

- **Files**: Subsystem requirements, design documents, integration tests (usually no source code)
- **Purpose**: Validates subsystem behavior and interface compliance
- **File Path Pattern**:
  - Requirements: `docs/reqstream/{subsystem-name}/{subsystem-name}.yaml`
  - Design: `docs/design/{subsystem-name}/{subsystem-name}.md`
  - Tests: `test/{ProjectName}.Tests/{SubsystemName}Integration/` or similar
- **Example**: `MyProduct-Authentication`, `MyProduct-DataLayer`

## [Product]-OTS Review

Reviews OTS (Off-The-Shelf) software integration:

- **Files**: OTS requirements and integration test evidence
- **Purpose**: Validates OTS components meet integration requirements
- **File Path Pattern**:
  - Requirements: `docs/reqstream/ots/{ots-name}.yaml`
  - Tests: Integration tests proving OTS functionality
- **Example**: `MyProduct-SystemTextJson`, `MyProduct-EntityFramework`

# File Pattern Best Practices

Use "include-then-exclude" approach for `needs-review` patterns because it
ensures comprehensive coverage while removing unwanted files:

1. **Start broad**: Include all files of potential interest with generous patterns
2. **Exclude overreach**: Use `!` patterns to remove build output, generated files, and temporary files
3. **Test patterns**: Verify patterns match intended files using `dotnet reviewmark --elaborate`

**Order matters**: Patterns are processed sequentially, excludes override earlier includes.

# ReviewMark Commands

Essential ReviewMark commands for Continuous Compliance:

```bash
# Lint review configuration for issues (run before use)
dotnet reviewmark --lint

# Generate review plan and report (use in CI/CD)
dotnet reviewmark \
  --plan docs/code_review_plan/plan.md \
  --report docs/code_review_report/report.md \
  --enforce
```

# Quality Checks

Before submitting ReviewMark configuration, verify:

- [ ] `.reviewmark.yaml` exists at repository root with proper structure
- [ ] `needs-review` patterns cover requirements, design, code, and tests with proper exclusions
- [ ] Each review-set has unique `id` and groups architecturally related files
- [ ] File patterns use correct glob syntax and match intended files
- [ ] File paths reflect current naming conventions (kebab-case design/requirements folders, PascalCase source folders)
- [ ] Evidence source properly configured (`none` for dev, `url` for production)
- [ ] Environment variables used for credentials (never hardcoded)
- [ ] ReviewMark enforcement configured: `dotnet reviewmark --enforce`
- [ ] Generated documents accessible for compliance auditing
- [ ] Review-set organization follows standard patterns ([Product]-[Unit], [Product]-Design, etc.)
