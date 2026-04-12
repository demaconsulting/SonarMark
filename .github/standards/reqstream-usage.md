---
name: ReqStream Usage
description: Follow these standards when managing requirements with ReqStream.
globs: ["requirements.yaml", "docs/reqstream/**/*.yaml"]
---

# ReqStream Requirements Management Standards

This document defines DEMA Consulting standards for requirements management
using ReqStream within Continuous Compliance environments.

## Required Standards

Read these standards first before applying this standard:

- **`software-items.md`** - Software categorization (System/Subsystem/Unit/OTS)

# Core Principles

ReqStream implements Continuous Compliance methodology for automated evidence
generation:

- **Requirements Traceability**: Every requirement MUST link to passing tests
- **Platform Evidence**: Source filters ensure correct testing environment
  validation
- **Quality Gate Enforcement**: CI/CD fails on requirements without test
  coverage
- **Audit Documentation**: Generated reports provide compliance evidence

# Software Items Integration (CRITICAL)

Before creating requirements files, agents MUST:

1. **Read `.github/standards/software-items.md`** to understand System/Subsystem/Unit/OTS classifications
2. **Apply proper categorization** when organizing requirements files
3. **Mirror source code structure** in requirements folder organization

# Requirements Organization

Organize requirements into separate files under `docs/reqstream/` mirroring
the source code structure because reviewers need clear navigation from
requirements to design to implementation:

```text
requirements.yaml                   # Root file (includes only)
docs/reqstream/
├── {system-name}/                  # System-level requirements folder (one per system)
│   ├── {system-name}.yaml          # System-level requirements
│   ├── platform-requirements.yaml  # Platform support requirements
│   ├── {subsystem-name}/           # Subsystem requirements (kebab-case folders)
│   │   ├── {subsystem-name}.yaml   # Requirements for this subsystem
│   │   └── {unit-name}.yaml        # Requirements for units within this subsystem
│   └── {unit-name}.yaml            # Requirements for top-level units (outside subsystems)
└── ots/                            # OTS software items folder
    └── {ots-name}.yaml             # Requirements for OTS components
```

The folder structure MUST mirror the source code organization to maintain
consistency with design documentation and enable automated tooling.

# Requirement Hierarchies and Links

Requirements link downward only - higher-level requirements reference lower-level
ones they decompose into:

- **System requirements** → may link to subsystem or unit requirements
- **Subsystem requirements** → may link to unit requirements within that subsystem
- **Unit requirements** → should NOT link upward to parent requirements

This prevents circular dependencies and ensures clear hierarchical relationships
for compliance auditing.

# Test Linkage Hierarchy

Requirements MUST link to tests at their own level to maintain proper test scope:

- **System requirements** → link ONLY to system-level integration tests
- **Subsystem requirements** → link ONLY to subsystem-level tests
- **Unit requirements** → link ONLY to unit-level tests

Lower-level tests validate implementation details, while higher-level requirements
are validated through integration behavior at their architectural level.

# Requirements File Format

```yaml
sections:
  - title: Functional Requirements
    requirements:
      - id: System-Subsystem-Feature
        title: The system shall perform the required function.
        justification: |
          Business rationale explaining why this requirement exists.
          Include regulatory or standard references where applicable.
        children:  # Downward links to decomposed requirements (optional)
          - ChildSystem-Feature-Behavior
        tests:     # Links to test methods (required)
          - TestMethodName
          - windows@PlatformSpecificTest  # Source filter for platform evidence
```

Requirements specify WHAT the system shall do, not HOW, because implementation
details belong in design documentation while requirements focus on externally
observable behavior with clear, testable acceptance criteria.

# OTS Software Requirements

Document third-party component requirements in the `docs/reqstream/ots/` folder
with nested sections because auditors need clear separation between in-house
and external component evidence:

```yaml
sections:
  - title: OTS Software Requirements
    sections:
      - title: System.Text.Json
        requirements:
          - id: TemplateTool-SystemTextJson-ReadJson
            title: System.Text.Json shall be able to read JSON files.
            tests:
              - JsonReaderTests.TestReadValidJson
```

# Semantic IDs (MANDATORY)

Use meaningful IDs following `System-Section-ShortDesc` pattern because
auditors need to understand requirements without cross-referencing:

- **Good**: `TemplateTool-Core-DisplayHelp`
- **Bad**: `REQ-042` (requires lookup to understand)

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

- [ ] All requirements have semantic IDs (`System-Section-Feature` pattern)
- [ ] Every requirement links to at least one passing test
- [ ] Platform-specific requirements use source filters (`platform@TestName`)
- [ ] Requirements specify observable behavior (WHAT), not implementation (HOW)
- [ ] Comprehensive justification explains business/regulatory need
- [ ] Files organized under `docs/reqstream/` following folder structure patterns
- [ ] Subsystem folders use kebab-case naming matching source code
- [ ] OTS requirements placed in `ots/` subfolder
- [ ] Every software unit has requirements file, design doc, and tests
- [ ] Valid YAML syntax passes yamllint validation
- [ ] ReqStream enforcement passes: `dotnet reqstream --enforce`
- [ ] Test result formats compatible (TRX, JUnit XML)
