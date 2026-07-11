## SysML2Tools Verification

This document provides the verification evidence for the `SysML2Tools` OTS software item.
Requirements for this OTS item are defined in the SysML2Tools OTS Software Requirements document.

### Required Functionality

DemaConsulting.SysML2Tools validates the SysML2 architecture model under `docs/sysml2/` for syntax
and reference errors, and renders each view declared in `docs/sysml2/views/design-views.sysml` to
an SVG diagram embedded in the design documentation. Both behaviors run in the same CI pipeline
that produces the compiled Design document, so a successful pipeline run is evidence that
SysML2Tools executed without error.

### Verification Approach

SysML2Tools is verified by a combination of authored self-validation evidence and CI pipeline
evidence:

- **Self-validation evidence**: `dotnet sysml2tools --validate --results <trx-file>` runs the
  tool's built-in self-test suite, which exercises `lint` and `render --format svg` against known
  model fixtures and writes a TRX file consumed by `reqstream --enforce`
  (`SysML2Tools_LintSelfTest`, `SysML2Tools_RenderSvgSelfTest`).
- **Pipeline evidence**: `lint.ps1` runs `dotnet sysml2tools lint 'docs/sysml2/**/*.sysml'` against
  the actual SonarMark model and fails the build on any syntax or reference error. The build-docs
  job runs `dotnet sysml2tools render` to produce one SVG file per declared view under
  `docs/design/generated/`. Pandoc then compiles `docs/design/*.md`, which embed those SVG files by
  filename; if a declared view failed to render, the missing image reference would cause a broken
  image in the compiled Design HTML and PDF. WeasyPrint renders the result to PDF and FileAssert
  asserts its content. A CI build failure at either step is evidence that SysML2Tools did not
  produce the required model validation or diagrams against the real model.

### Test Scenarios

#### SysML2Tools_LintSelfTest

**Scenario**: SysML2Tools is invoked with `--validate`, which exercises `lint` against a known-good
and a known-bad model fixture as part of its built-in self-test suite.

**Expected**: Exits 0 with no reported syntax or reference errors for the valid fixture, and
correctly reports an error for the invalid fixture.

#### SysML2Tools_RenderSvgSelfTest

**Scenario**: SysML2Tools is invoked with `--validate`, which exercises `render --format svg`
against a known-good model fixture as part of its built-in self-test suite.

**Expected**: Exits 0 and produces a non-empty SVG file for the fixture's declared view.

### Acceptance Criteria

N/A - Acceptance criteria are managed at the system integration level. This OTS item is considered
verified when the integration test scenarios that exercise its functionality pass in the CI
pipeline.
