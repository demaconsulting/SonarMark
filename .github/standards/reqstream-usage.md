# ReqStream Requirements Management Standards

This document defines DEMA Consulting standards for requirements management
using ReqStream within Continuous Compliance environments.

# Core Principles

ReqStream implements Continuous Compliance methodology for automated evidence
generation:

- **Requirements Traceability**: Every requirement MUST link to passing tests
- **Platform Evidence**: Source filters ensure correct testing environment
  validation
- **Quality Gate Enforcement**: CI/CD fails on requirements without test
  coverage
- **Audit Documentation**: Generated reports provide compliance evidence

# Requirements Organization

Organize requirements into separate files under `docs/reqstream/` for
independent review:

```text
requirements.yaml                   # Root file (includes only)
docs/reqstream/
  {project}-system.yaml             # System-level requirements
  platform-requirements.yaml        # Platform support requirements
  subsystem-{subsystem}.yaml        # Subsystem requirements
  unit-{unit}.yaml                  # Unit (class) requirements
  ots-{component}.yaml              # OTS software item requirements
```

# Requirements File Format

```yaml
sections:
  - title: Functional Requirements
    requirements:
      - id: Project-Component-Feature
        title: The system shall perform the required function.
        justification: |
          Business rationale explaining why this requirement exists.
          Include regulatory or standard references where applicable.
        tests:
          - TestMethodName
          - windows@PlatformSpecificTest  # Source filter for platform evidence
```

# OTS Software Requirements

Document third-party component requirements with specific section structure:

```yaml
sections:
  - title: OTS Software Requirements
    sections:
      - title: System.Text.Json
        requirements:
          - id: Project-SystemTextJson-ReadJson
            title: System.Text.Json shall be able to read JSON files.
            tests:
              - JsonReaderTests.TestReadValidJson
```

# Semantic IDs (MANDATORY)

Use meaningful IDs following `Project-Section-ShortDesc` pattern:

- **Good**: `TemplateTool-Core-DisplayHelp`
- **Bad**: `REQ-042` (requires lookup to understand)

# Requirement Best Practices

Requirements specify WHAT the system shall do, not HOW:

- Focus on externally observable characteristics and behavior
- Avoid implementation details, design constraints, or technology choices
- Each requirement must have clear, testable acceptance criteria

Include business rationale for each requirement:

- Business need or regulatory requirement
- Risk mitigation or quality improvement
- Standard or regulation references

# Source Filter Requirements (CRITICAL)

Platform-specific requirements MUST use source filters for compliance evidence:

```yaml
tests:
  - "windows@TestMethodName"    # Windows platform evidence only
  - "ubuntu@TestMethodName"     # Linux platform evidence only
  - "net8.0@TestMethodName"     # .NET 8 runtime evidence only
  - "TestMethodName"            # Any platform evidence acceptable
```

**WARNING**: Removing source filters invalidates platform-specific compliance
evidence.

# ReqStream Commands

Essential ReqStream commands for Continuous Compliance:

```bash
# Lint requirement files for issues (run before use)
dotnet reqstream \
  --requirements requirements.yaml \
  --lint

# Enforce requirements traceability (use in CI/CD)
dotnet reqstream \
  --requirements requirements.yaml \
  --tests "artifacts/**/*.trx" \
  --enforce

# Generate requirements report
dotnet reqstream \
  --requirements requirements.yaml \
  --report docs/requirements_doc/requirements.md

# Generate justifications report
dotnet reqstream \
  --requirements requirements.yaml \
  --justifications docs/requirements_doc/justifications.md

# Generate trace matrix
dotnet reqstream \
  --requirements requirements.yaml \
  --tests "artifacts/**/*.trx" \
  --matrix docs/requirements_report/trace_matrix.md
```

# Quality Checks

Before submitting requirements, verify:

- [ ] All requirements have semantic IDs (`Project-Section-Feature` pattern)
- [ ] Every requirement links to at least one passing test
- [ ] Platform-specific requirements use source filters (`platform@TestName`)
- [ ] Requirements specify observable behavior (WHAT), not implementation (HOW)
- [ ] Comprehensive justification explains business/regulatory need
- [ ] Files organized under `docs/reqstream/` following naming patterns
- [ ] Valid YAML syntax passes yamllint validation
- [ ] ReqStream enforcement passes: `dotnet reqstream --enforce`
- [ ] Test result formats compatible (TRX, JUnit XML)
