---
name: ReqStream Usage
description: Follow these standards when managing requirements with ReqStream.
globs: ["requirements.yaml", "docs/reqstream/**/*.yaml"]
---

# Required Standards

Read these standards first before applying this standard:

- **`requirements-principles.md`** - Requirements principles and unidirectionality
- **`software-items.md`** - Software categorization (System/Subsystem/Unit/OTS)

# Requirements Organization

Organize requirements under `docs/reqstream/` mirroring the source code structure
because ReqStream discovers files via the includes chain in `requirements.yaml`
and organizes report output by this hierarchy:

```text
requirements.yaml                   # Root file (includes only)
docs/reqstream/
├── {system-name}/                  # System-level requirements folder (one per system)
│   ├── {system-name}.yaml          # System-level requirements
│   ├── platform-requirements.yaml  # Platform support requirements
│   ├── {subsystem-name}/           # Subsystem (kebab-case); may nest recursively
│   │   ├── {subsystem-name}.yaml   # Requirements for this subsystem
│   │   ├── {child-subsystem}/      # Child subsystem (same structure as parent)
│   │   └── {unit-name}.yaml        # Requirements for units within this subsystem
│   └── {unit-name}.yaml            # Requirements for top-level units (outside subsystems)
└── ots/                            # OTS items appear as a distinct section in reports
    └── {ots-name}.yaml             # Requirements for OTS components
```

# Requirements File Format

```yaml
sections:
  - title: Functional Requirements
    requirements:
      - id: System-Component-Feature      # Used as-is in all reports - make it readable
        title: The system shall perform the required function.
        justification: |
          Business rationale and any regulatory references.
          # ReqStream extracts this field into the justifications report (--justifications)
        children:                         # ReqStream validates this decomposition chain
          - ChildSystem-Feature-Behavior  # Downward links only (see requirements-principles.md)
        tests:                            # ReqStream matches these by method name in test results
          - TestMethodName
          - windows@PlatformSpecificTest  # Only test runs on Windows count as evidence
```

# OTS Software Requirements

Use nested sections in `docs/reqstream/ots/` because ReqStream renders the `ots/`
subtree as a distinct section in generated reports, separate from in-house
system requirements:

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

Use the `System-Component-Feature` pattern because ReqStream uses IDs as-is in
all generated reports and the trace matrix - opaque IDs make those outputs
unreadable without a separate lookup:

- **System-level**: `TemplateTool-Core-DisplayHelp`
- **Subsystem-level**: `TemplateTool-Parser-ParseYaml`
- **Unit-level**: `TemplateTool-Validator-CheckFormat`
- **Bad**: `REQ-042` (meaningless in report output)

# Source Filter Requirements (CRITICAL)

Platform-specific requirements MUST use source filters because without them
ReqStream accepts test results from any platform as evidence - a Windows-only
requirement would incorrectly pass on Linux:

```yaml
tests:
  - "windows@TestMethodName"    # Only Windows test runs count as evidence
  - "ubuntu@TestMethodName"     # Only Linux test runs count as evidence
  - "net8.0@TestMethodName"     # Only .NET 8 runs count as evidence
  - "TestMethodName"            # Any platform acceptable
```

**WARNING**: Removing source filters invalidates platform-specific compliance
evidence.

# ReqStream Commands

```bash
# Validate YAML syntax and requirement IDs before generating any reports
dotnet reqstream --requirements requirements.yaml --lint

# Generate requirements document for compliance record
dotnet reqstream --requirements requirements.yaml \
  --report docs/requirements_doc/requirements.md

# Generate justifications document for compliance record
dotnet reqstream --requirements requirements.yaml \
  --justifications docs/requirements_doc/justifications.md

# Generate trace matrix proving each requirement is covered by passing tests
dotnet reqstream --requirements requirements.yaml \
  --tests "artifacts/**/*.trx" \
  --matrix docs/requirements_report/trace_matrix.md
```

# Quality Checks

Before submitting requirements, verify:

- [ ] All requirements have semantic IDs (`System-Section-Feature` pattern)
- [ ] Every requirement links to at least one passing test
- [ ] Platform-specific requirements use source filters (`platform@TestName`)
- [ ] Comprehensive justification explains business/regulatory need
- [ ] Files organized under `docs/reqstream/` following folder structure patterns
- [ ] Subsystem folders use kebab-case naming matching source code
- [ ] OTS requirements placed in `ots/` subfolder
- [ ] Valid YAML syntax passes yamllint validation
- [ ] Test result formats compatible (TRX, JUnit XML)
