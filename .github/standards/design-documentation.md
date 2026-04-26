---
name: Design Documentation
description: Follow these standards when creating design documentation.
globs: ["docs/design/**/*.md"]
---

# Design Documentation Standards

This document defines standards for design documentation within Continuous
Compliance environments, extending the general technical documentation
standards with specific requirements for software design artifacts.

## Required Standards

Read these standards first before applying this standard:

- **`technical-documentation.md`** - General technical documentation standards
- **`software-items.md`** - Software categorization (System/Subsystem/Unit/OTS)

# Core Principles

Design documentation serves as the bridge between requirements and
implementation, providing detailed technical specifications that enable:

- **Formal Code Review**: Reviewers can verify implementation matches design
- **Compliance Evidence**: Auditors can trace requirements through design to code
- **Maintenance Support**: Developers can understand system structure and interactions
- **Quality Assurance**: Testing teams can validate against detailed specifications

# Required Structure and Documents

Design documentation must be organized under `docs/design/` with folder structure
mirroring source code organization because reviewers need clear navigation from
design to implementation:

```text
docs/design/
├── introduction.md              # Design overview with software structure
└── {system-name}/               # System-level design folder (one per system)
    ├── {system-name}.md         # System-level design documentation
    ├── {subsystem-name}/        # Subsystem (kebab-case); may nest recursively
    │   ├── {subsystem-name}.md  # Subsystem overview and design
    │   ├── {child-subsystem}/   # Child subsystem (same structure as parent)
    │   └── {unit-name}.md       # Unit-level design documents
    └── {unit-name}.md           # Top-level unit design documents (if not in subsystem)
```

## introduction.md (MANDATORY)

The `introduction.md` file serves as the design entry point and MUST include
these sections because auditors need clear scope boundaries and architectural
overview:

### Purpose Section

Clear statement of the design document's purpose, audience, and regulatory
or compliance drivers.

### Scope Section

Define what software items are covered and what is explicitly excluded.
Design documentation must NOT include test projects, test classes, or test
infrastructure because design documentation documents the architecture of
shipping product code, not ancillary content used to validate it.

### Software Structure Section (MANDATORY)

Include a text-based tree diagram showing the software organization across
System, Subsystem, and Unit levels. Agents MUST read `software-items.md`
to understand these classifications before creating this section.

Example format:

```text
Project1Name (System)
├── ComponentA (Subsystem)
│   ├── SubComponentP (Subsystem)
│   │   └── ClassW (Unit)
│   ├── ClassX (Unit)
│   └── ClassY (Unit)
├── ComponentB (Subsystem)
│   └── ClassZ (Unit)
└── UtilityClass (Unit)

Project2Name (System)
└── HelperClass (Unit)
```

### Folder Layout Section (MANDATORY)

Include a text-based tree diagram showing how the source code folders
mirror the software structure, with file paths and brief descriptions.

Example format:

```text
src/Project1Name/
├── ComponentA/
│   ├── SubComponentP/
│   │   └── ClassW.cs           - Specialized processing engine
│   ├── ClassX.cs               - Core business logic handler
│   └── ClassY.cs               - Data validation service
├── ComponentB/
│   └── ClassZ.cs               - Integration interface
└── UtilityClass.cs             - Common utility functions

src/Project2Name/
└── HelperClass.cs              - Helper functions
```

### Companion Artifact Structure (RECOMMENDED)

Include a brief note explaining that each software item has parallel artifacts
across the repository, so agents and reviewers can navigate from any one
artifact to all related files:

Example format:

```text
Each software item in the structure above has corresponding artifacts in
parallel directory trees:

- Requirements: `docs/reqstream/{system}/.../{item}.yaml` (kebab-case)
- Design docs: `docs/design/{system}/.../{item}.md` (kebab-case)
- Source code: `src/{System}/.../{Item}.{ext}` (cased per language - see `software-items.md`)
- Tests: `test/{System}.Tests/.../{Item}Tests.{ext}` (cased per language - see `software-items.md`)
- Review-sets: defined in `.reviewmark.yaml`
```

## System Design Documentation (MANDATORY)

For each system identified in the repository:

- Create a kebab-case folder matching the system name
- Include `{system-name}.md` with system-level design documentation such as:
  - System architecture and major components
  - External interfaces and dependencies
  - Data flow and control flow
  - System-wide design constraints and decisions
  - Integration patterns and communication protocols

## Subsystem and Unit Design Documents

For each subsystem identified in the software structure:

- Create a kebab-case folder matching the subsystem name (enables automated tooling)
- Include `{subsystem-name}.md` with subsystem overview and design
- Include unit design documents for ALL units within the subsystem

For every unit identified in the software structure:

- Document data models, algorithms, and key methods
- Describe interactions with other units
- Include sufficient detail for formal code review
- Place in appropriate subsystem folder or at design root level

# Software Items Integration (CRITICAL)

Read `software-items.md` before creating design documentation - correct
System/Subsystem/Unit categorization is required for software structure
diagrams and folder layout.

# Writing Guidelines

Design documentation must be technical and specific because it serves as the
implementation specification for formal code review:

- **Implementation Detail**: Provide sufficient detail for code review and implementation
- **Architectural Clarity**: Clearly define component boundaries and interfaces
- **Traceability**: Link to requirements where applicable using ReqStream patterns

# Mermaid Diagram Integration

Use Mermaid diagrams to supplement text descriptions (diagrams must not replace text content).

# Quality Checks

Before submitting design documentation, verify:

- [ ] `introduction.md` includes both Software Structure and Folder Layout sections
- [ ] Software structure correctly categorizes items as System/Subsystem/Unit per `software-items.md`
- [ ] Folder layout mirrors software structure organization
- [ ] Design documents provide sufficient detail for code review
- [ ] System documentation provides comprehensive system-level design
- [ ] Subsystem documentation folders use kebab-case names while mirroring source subsystem names and structure
- [ ] All documents follow technical documentation formatting standards
- [ ] Content is current with implementation and requirements
- [ ] Documents are integrated into ReviewMark review-sets for formal review
