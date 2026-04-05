# ReviewMark Usage Standard

## Purpose

ReviewMark manages file review status enforcement and formal review processes. It tracks which files need
review, organizes them into review-sets, and generates review plans and reports.

## Key Commands

- **Lint Configuration**: `dotnet reviewmark --lint`
- **Elaborate Review-Set**: `dotnet reviewmark --elaborate {review-set}`
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

# Review-Set Design Principles

When constructing review-sets, follow these principles to maintain manageable scope and effective compliance evidence:

- **Hierarchical Scope**: Higher-level reviews exclude lower-level implementation details, relying instead on design
  documents to describe what components they use. System reviews exclude subsystem/unit details, subsystem reviews
  exclude unit source code, only unit reviews include actual implementation.
- **Single Focus**: Each review-set proves one specific compliance question (user promises, system architecture,
  design consistency, etc.)
- **Context Management**: Keep file counts manageable to prevent context overflow while maintaining complete coverage
  through the hierarchy

# Review-Set Organization

Organize review-sets using these standard patterns to ensure comprehensive coverage
while keeping each review manageable in scope:

**Note**: File path patterns shown below use C# naming conventions (PascalCase, `.cs` extensions).
Other languages should adapt these patterns to their conventions (e.g., C++ might use
`snake_case` with `.cpp`/`.hpp` extensions).

## `Purpose` Review (only one per repository)

Reviews user-facing capabilities and system promises:

- **Purpose**: Proves that the systems provide the capabilities the user is being told about
- **Title**: "Review that Advertised Features Match System Design"
- **Scope**: Excludes subsystem and unit files, relying on system-level design documents
  to describe what subsystems and units they use
- **File Path Patterns**:
  - README: `README.md`
  - User guide: `docs/user_guide/**/*.md`
  - System requirements: `docs/reqstream/{system-name}/{system-name}.yaml`
  - Design introduction: `docs/design/introduction.md`
  - System design: `docs/design/{system-name}/{system-name}.md`

## `{System}-Architecture` Review (one per system)

Reviews system architecture and operational validation:

- **Purpose**: Proves that the system is designed and tested to satisfy its requirements
- **Title**: "Review that {System} Architecture Satisfies Requirements"
- **Scope**: Excludes subsystem and unit files, relying on system-level design to describe
  what subsystems and units it uses
- **File Path Patterns**:
  - System requirements: `docs/reqstream/{system-name}/{system-name}.yaml`
  - Design introduction: `docs/design/introduction.md`
  - System design: `docs/design/{system-name}/{system-name}.md`
  - System integration tests: `test/{SystemName}.Tests/{SystemName}Tests.cs`

## `{System}-Design` Review (one per system)

Reviews architectural and design consistency:

- **Purpose**: Proves the system design is consistent and complete
- **Title**: "Review that {System} Design is Consistent and Complete"
- **Scope**: Only brings in top-level requirements and relies on brevity of design documentation
- **File Path Patterns**:
  - System requirements: `docs/reqstream/{system-name}/{system-name}.yaml`
  - Platform requirements: `docs/reqstream/{system-name}/platform-requirements.yaml`
  - Design introduction: `docs/design/introduction.md`
  - System design files: `docs/design/{system-name}/**/*.md`

## `{System}-AllRequirements` Review (one per system)

Reviews requirements quality and traceability:

- **Purpose**: Proves the requirements are consistent and complete
- **Title**: "Review that All {System} Requirements are Complete"
- **Scope**: Only brings in requirements files to keep review manageable
- **File Path Patterns**:
  - Root requirements: `requirements.yaml`
  - System requirements: `docs/reqstream/{system-name}/**/*.yaml`
  - OTS requirements: `docs/reqstream/ots/**/*.yaml` (if applicable)

## `{System}-{Subsystem}` Review (one per subsystem)

Reviews subsystem architecture and interfaces:

- **Purpose**: Proves that the subsystem is designed and tested to satisfy its requirements
- **Title**: "Review that {System} {Subsystem} Satisfies Subsystem Requirements"
- **Scope**: Excludes units under the subsystem, relying on subsystem design to describe
  what units it uses
- **File Path Patterns**:
  - Requirements: `docs/reqstream/{system-name}/{subsystem-name}/{subsystem-name}.yaml`
  - Design: `docs/design/{system-name}/{subsystem-name}/{subsystem-name}.md`
  - Tests: `test/{SystemName}.Tests/{SubsystemName}/{SubsystemName}Tests.cs`

## `{System}-{Subsystem}-{Unit}` Review (one per unit)

Reviews individual software unit implementation:

- **Purpose**: Proves the unit is designed, implemented, and tested to satisfy its requirements
- **Title**: "Review that {System} {Subsystem} {Unit} Implementation is Correct"
- **Scope**: Complete unit review including all artifacts
- **File Path Patterns**:
  - Requirements: `docs/reqstream/{system-name}/{subsystem-name}/{unit-name}.yaml` or
    `docs/reqstream/{system-name}/{unit-name}.yaml`
  - Design: `docs/design/{system-name}/{subsystem-name}/{unit-name}.md` or
    `docs/design/{system-name}/{unit-name}.md`
  - Source: `src/{SystemName}/{SubsystemName}/{UnitName}.cs` or `src/{SystemName}/{UnitName}.cs`
  - Tests: `test/{SystemName}.Tests/{SubsystemName}/{UnitName}Tests.cs` or
    `test/{SystemName}.Tests/{UnitName}Tests.cs`

# Quality Checks

Before submitting ReviewMark configuration, verify:

- [ ] `.reviewmark.yaml` exists at repository root with proper structure
- [ ] Review-set organization follows the standard hierarchy patterns
- [ ] Purpose review-set includes README.md, user guide, system requirements, design introduction, and system design files
- [ ] System-level reviews follow hierarchical scope principle (exclude subsystem/unit details)
- [ ] Subsystem reviews follow hierarchical scope principle (exclude unit source code)
- [ ] Only unit reviews include actual source code files
- [ ] Each review-set focuses on a single compliance question (single focus principle)
- [ ] File patterns use correct glob syntax and match intended files
- [ ] Review-set file counts remain manageable (context management principle)
- [ ] Evidence source properly configured (`none` for dev, `url` for production)
