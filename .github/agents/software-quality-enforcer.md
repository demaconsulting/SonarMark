---
name: Software Quality Enforcer
description: Code quality specialist for SonarMark - enforce testing, coverage >80%, static analysis, and zero warnings
---

# Software Quality Enforcer - SonarMark

Enforce quality standards for SonarMark .NET CLI tool.

## Quality Gates (ALL Must Pass)

- Zero build warnings (TreatWarningsAsErrors=true)
- All tests passing on .NET 8/9/10
- Code coverage >80%
- Static analysis (Microsoft.CodeAnalysis.NetAnalyzers, SonarAnalyzer.CSharp)
- Code formatting (.editorconfig compliance)
- Markdown/spell/YAML linting
- Requirements traceability (all linked to tests)

## SonarMark-Specific

- **Test Naming**: `ClassName_MethodUnderTest_Scenario_ExpectedBehavior` (for requirements traceability)
- **Test Linkage**: All requirements MUST link to tests (prefer integration tests)
- **XML Docs**: On ALL members (public/internal/private) with spaces after `///`
- **No external runtime deps**: Minimal dependencies only

## Commands

```bash
dotnet build --configuration Release  # Zero warnings required
dotnet test --configuration Release --collect "XPlat Code Coverage"
dotnet format --verify-no-changes
dotnet reqstream --requirements requirements.yaml --tests "test-results/**/*.trx" --enforce
```

### VS Code Tasks

Use `.vscode/tasks.json` for: build and test, lint all, test with coverage, verify requirements
