## SonarMark (Released Package)

### Purpose

A released version of SonarMark is consumed as a Shared Package by the SonarMark
`build-docs` CI job. The released version is installed from the `.config/dotnet-tools.json`
manifest using `dotnet tool restore` and is pinned to a specific version. Its internal
design is outside the scope of this document; only its advertised features are relevant here.

The released SonarMark is used in two ways within the `build-docs` job:

1. **Self-Validation** — to confirm the released binary is operational in the CI environment
2. **Report Generation** — to retrieve SonarCloud analysis results and produce the quality
   report that is included in the SonarMark documentation

### Classification

The released SonarMark is produced by the same program as the SonarMark being built. It is
a prior released version of the same tool consumed as a dependency by its own build process.
Per the software-items standard, a software package produced within the same program and
consumed as a dependency is classified as a Shared Package.

### Features Used

- **Self-Validation (`--validate`)** — runs SonarMark's four built-in self-validation
  scenarios (quality gate retrieval, issues retrieval, hot-spots retrieval, and markdown
  report generation) and reports a pass/fail outcome for each. Used in `build-docs` to
  confirm the released binary is functional before the report-generation step.

- **Report Generation (`--server`, `--project-key`, `--report`)** — retrieves project
  analysis data from the SonarQube/SonarCloud REST API and writes a structured markdown
  quality report. Used in `build-docs` to generate the SonarCloud quality report at
  `docs/code_quality/generated/sonar-quality.md`.

### Integration Pattern

The released SonarMark is listed as `demaconsulting.sonarmark` in
`.config/dotnet-tools.json` and is installed with `dotnet tool restore`. No additional
initialization or configuration is required; both features are invoked directly from
`build-docs` CI job steps using the `dotnet sonarmark` command.
