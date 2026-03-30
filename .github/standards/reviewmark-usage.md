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
  - "**/*.cs"                    # All C# source and test files
  - "**/*.md"                    # Requirements and design documentation
  - "docs/reqstream/**/*.yaml"   # Requirements files only

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
      - "src/Auth/PasswordValidator.cs"
      - "docs/reqstream/auth-passwordvalidator-class.yaml"
      - "test/Auth/PasswordValidatorTests.cs"
      - "docs/design/password-validation.md"

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

- **Files**: System-level requirements, design introduction, system design documents, integration tests
- **Purpose**: Validates system operates as designed and meets overall requirements
- **Example**: `TemplateTool-System`

## [Product]-Design Review

Reviews architectural and design consistency:

- **Files**: System-level requirements, platform requirements, all design documents
- **Purpose**: Ensures design completeness and architectural coherence
- **Example**: `MyProduct-Design`

## [Product]-AllRequirements Review

Reviews requirements quality and traceability:

- **Files**: All requirement files including root `requirements.yaml`
- **Purpose**: Validates requirements structure, IDs, justifications, and test linkage
- **Example**: `MyProduct-AllRequirements`

## [Product]-[Unit] Review

Reviews individual software unit implementation:

- **Files**: Unit requirements, design documents, source code, unit tests
- **Purpose**: Validates unit meets requirements and is properly implemented
- **Example**: `MyProduct-PasswordValidator`, `MyProduct-ConfigParser`

## [Product]-[Subsystem] Review

Reviews subsystem architecture and interfaces:

- **Files**: Subsystem requirements, design documents, integration tests (usually no source code)
- **Purpose**: Validates subsystem behavior and interface compliance
- **Example**: `MyProduct-Authentication`, `MyProduct-DataLayer`

# ReviewMark Commands

Essential ReviewMark commands for Continuous Compliance:

```bash
# Lint review configuration for issues (run before use)
dotnet reviewmark \
  --lint

# Generate review plan (shows coverage)
dotnet reviewmark \
  --plan docs/code_review_plan/plan.md

# Generate review report (shows status)
dotnet reviewmark \
  --report docs/code_review_report/report.md

# Enforce review compliance (use in CI/CD)
dotnet reviewmark \
  --plan docs/code_review_plan/plan.md \
  --report docs/code_review_report/report.md \
  --enforce
```

# File Pattern Best Practices

Use "include-then-exclude" approach for `needs-review` patterns because it
ensures comprehensive coverage while removing unwanted files:

## Include-Then-Exclude Strategy

1. **Start broad**: Include all files of potential interest with generous patterns
2. **Exclude overreach**: Use `!` patterns to remove build output, generated files, and temporary files
3. **Test patterns**: Verify patterns match intended files using `dotnet reviewmark --elaborate`

## Pattern Guidelines

- **Be generous with includes**: Better to include too much initially than miss important files
- **Be specific with excludes**: Target exact paths and patterns that should never be reviewed
- **Order matters**: Patterns are processed sequentially, excludes override earlier includes

# Quality Checks

Before submitting ReviewMark configuration, verify:

- [ ] `.reviewmark.yaml` exists at repository root with proper structure
- [ ] `needs-review` patterns cover requirements, design, code, and tests with proper exclusions
- [ ] Each review-set has unique `id` and groups architecturally related files
- [ ] File patterns use correct glob syntax and match intended files
- [ ] Evidence source properly configured (`none` for dev, `url` for production)
- [ ] Environment variables used for credentials (never hardcoded)
- [ ] ReviewMark enforcement configured: `dotnet reviewmark --enforce`
- [ ] Generated documents accessible for compliance auditing
- [ ] Review-set organization follows standard patterns ([Product]-[Unit], [Product]-Design, etc.)
