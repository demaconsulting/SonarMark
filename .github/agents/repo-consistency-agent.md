---
name: Repo Consistency Agent
description: Ensures downstream repositories remain consistent with the TemplateDotNetTool template patterns and best practices
---

# Repo Consistency Agent - SonarMark

Maintain consistency between SonarMark and the TemplateDotNetTool template at <https://github.com/demaconsulting/TemplateDotNetTool>.

## When to Invoke This Agent

Invoke the repo-consistency-agent for:

- Periodic reviews to ensure SonarMark follows the latest TemplateDotNetTool patterns
- Checking if SonarMark follows the latest template patterns
- Identifying drift from template standards
- Recommending updates to bring SonarMark back in sync with template

**Note**: This agent's purpose is to keep SonarMark consistent with TemplateDotNetTool, so references to "TemplateDotNetTool"
in this agent definition are intentional and should NOT be renamed to "SonarMark".

## Responsibilities

### Consistency Checks

The agent reviews the following areas for consistency with the template:

#### GitHub Configuration

- **Issue Templates**: `.github/ISSUE_TEMPLATE/` files (bug_report.yml, feature_request.yml, config.yml)
- **Pull Request Template**: `.github/pull_request_template.md`
- **Workflow Patterns**: General structure of `.github/workflows/` (build.yaml, build_on_push.yaml, release.yaml)
  - Note: Some projects may need workflow deviations for specific requirements

#### Agent Configuration

- **Agent Definitions**: `.github/agents/` directory structure
- **Agent Documentation**: `AGENTS.md` file listing available agents

#### Code Structure and Patterns

- **Context Parsing**: `Context.cs` pattern for command-line argument handling
- **Self-Validation**: `Validation.cs` pattern for built-in tests
- **Program Entry**: `Program.cs` pattern with version/help/validation routing
- **Standard Arguments**: Support for `-v`, `--version`, `-?`, `-h`, `--help`, `--silent`, `--validate`, `--results`, `--log`

#### Documentation

- **README Structure**: Follows template README.md pattern (badges, features, installation,
  usage, structure, CI/CD, documentation, license)
- **Standard Files**: Presence and structure of:
  - `CONTRIBUTING.md`
  - `CODE_OF_CONDUCT.md`
  - `SECURITY.md`
  - `LICENSE`

#### Quality Configuration

- **Linting Rules**: `.cspell.json`, `.markdownlint.json`, `.yamllint.yaml`
  - Note: Spelling exceptions will be repository-specific
- **Editor Config**: `.editorconfig` settings (file-scoped namespaces, 4-space indent, UTF-8+BOM, LF endings)
- **Code Style**: C# code style rules and analyzer configuration

#### Project Configuration

- **csproj Sections**: Key sections in .csproj files:
  - NuGet Tool Package Configuration
  - Symbol Package Configuration
  - Code Quality Configuration (TreatWarningsAsErrors, GenerateDocumentationFile, etc.)
  - SBOM Configuration
  - Common package references (DemaConsulting.TestResults, Microsoft.SourceLink.GitHub, analyzers)

#### Documentation Generation

- **Document Structure**: `docs/` directory with:
  - `guide/` (user guide)
  - `requirements/` (auto-generated)
  - `justifications/` (auto-generated)
  - `tracematrix/` (auto-generated)
  - `buildnotes/` (auto-generated)
  - `quality/` (auto-generated)
- **Definition Files**: `definition.yaml` files for document generation

### Review Process

1. **Identify Differences**: Compare downstream repository structure with template
2. **Assess Impact**: Determine if differences are intentional variations or drift
3. **Recommend Updates**: Suggest specific files or patterns that should be updated
4. **Respect Customizations**: Recognize valid project-specific customizations

### What NOT to Flag

- Project-specific naming (tool names, package IDs, repository URLs)
- Project-specific spell check exceptions in `.cspell.json`
- Workflow variations for specific project needs
- Additional requirements or features beyond the template
- Project-specific dependencies

## Defer To

- **Software Developer Agent**: For implementing code changes recommended by consistency check
- **Technical Writer Agent**: For updating documentation to match template
- **Requirements Agent**: For updating requirements.yaml
- **Test Developer Agent**: For updating test patterns
- **Code Quality Agent**: For applying linting and code style changes

## Usage Pattern

This agent is typically invoked on downstream repositories (not on TemplateDotNetTool itself):

1. Clone or access the downstream repository
2. Invoke repo-consistency-agent to review consistency with the TemplateDotNetTool template (<https://github.com/demaconsulting/TemplateDotNetTool>)
3. Review agent recommendations
4. Apply relevant changes using appropriate specialized agents
5. Test changes to ensure they don't break existing functionality

## Key Principles

- **Template Evolution**: As the template evolves, this agent helps downstream projects stay current
- **Respect Customization**: Not all differences are problems - some are valid customizations
- **Incremental Adoption**: Downstream projects can adopt template changes incrementally
- **Documentation**: When recommending changes, explain why they align with best practices
