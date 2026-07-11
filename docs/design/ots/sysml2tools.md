## SysML2Tools OTS Design

DemaConsulting.SysML2Tools is a .NET dotnet global tool that lints a SysML2 architecture model
and renders its declared views to SVG diagrams consumed by the design documentation.

### Purpose

SysML2Tools provides the toolchain that keeps SonarMark's architecture model
(`docs/sysml2/model/**/*.sysml`) and its declared views (`docs/sysml2/views/design-views.sysml`)
consistent with the design documentation. It validates the model for syntax and reference errors,
and renders each declared view to an SVG diagram embedded directly in the corresponding design
document (for example `docs/design/introduction.md` embeds `SoftwareStructureView.svg`). AI agents
also query the model directly via the `sysml2tools-query` skill rather than parsing the rendered
diagrams or hand-maintained prose.

SysML2Tools is chosen because it is the reference implementation for the SysML v2 subset used by
this project's Continuous Compliance methodology, and it integrates directly with the same
dotnet-tool restore and CI pipeline used by the other pipeline tools.

### Features Used

- Model validation via `dotnet sysml2tools lint 'docs/sysml2/**/*.sysml'`, which reports syntax
  errors and unresolved references without producing output files.
- View rendering via
  `dotnet sysml2tools render --output docs/design/generated --format svg 'docs/sysml2/model/**/*.sysml' 'docs/sysml2/views/design-views.sysml'`,
  which renders every `view` declared in `design-views.sysml` to an SVG file named after the view
  (for example `SoftwareStructureView.svg`).
- Model querying via the `sysml2tools-query` skill, used by AI agents to answer structural and
  traceability questions about the model without parsing the raw SysML2 source or the rendered
  diagrams.

### Integration Pattern

SysML2Tools is installed as a .NET local tool defined in `.config/dotnet-tools.json` under the
package name `demaconsulting.sysml2tools.tool` and restored with `dotnet tool restore`. It operates
directly on the SysML2 source files under `docs/sysml2/`; no separate configuration file is
required.

It is used in two places in the pipeline:

- **Lint** (`lint.ps1`, all CI jobs and local pre-PR checks): `dotnet sysml2tools lint
  'docs/sysml2/**/*.sysml'` fails the build if the model contains syntax errors or unresolved
  references.
- **Render** (`build.yaml`, build-docs job): `dotnet sysml2tools render` renders each declared view
  to an SVG file in `docs/design/generated/`, immediately before Pandoc compiles the Design
  document. The rendered SVGs sit alongside the compiled `design.html`, so the design Markdown
  sources embed bare filenames (for example `![Software Structure](SoftwareStructureView.svg)`)
  that Pandoc and the browser resolve relative to that directory.

SysML2Tools reads only the local SysML2 model files and writes only local SVG output files; it
requires no external service or network access, and it has no transitive NuGet dependencies that
propagate to the main source project. It is a build-time tool only.
