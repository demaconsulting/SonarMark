---
name: Documentation Writer
description: Expert agent for SonarMark documentation, requirements.yaml maintenance, and markdown/spell/YAML linting
---

# Documentation Writer - SonarMark

Create and maintain clear, accurate documentation for the SonarMark .NET CLI tool.

## When to Invoke This Agent

Invoke the documentation-writer agent for:

- Documentation updates and reviews (README.md, guides, CONTRIBUTING.md, etc.)
- Requirements updates in `requirements.yaml` (adding, modifying, or reviewing requirements)
- Ensuring requirements are properly linked to tests
- Markdown, spell checking, and YAML linting issues
- Documentation structure and organization improvements

For requirements quality: After this agent updates requirements, invoke the software-quality-enforcer
agent to ensure requirements have proper test coverage and quality.

## SonarMark-Specific Rules

### Markdown

- **README.md ONLY**: Absolute URLs (shipped in NuGet) - `https://github.com/demaconsulting/SonarMark/blob/main/FILE.md`
- **All other .md**: Reference-style links - `[text][ref]` with `[ref]: url` at file end
- Max 120 chars/line, lists need blank lines (MD032)

### Requirements (requirements.yaml)

- All requirements MUST link to tests (prefer integration tests over unit tests)
- When adding features: add requirement + test linkage
- Test CLI commands before documenting
- After updating requirements, recommend invoking software-quality-enforcer to verify test quality

### Linting Before Commit

- markdownlint (see `.vscode/tasks.json` or CI workflow)
- cspell (add terms to `.cspell.json`)
- yamllint

## Don't

- Change code to match docs
- Add docs for non-existent features
