## SonarMark (Released Package)

### Verification Approach

The released SonarMark is verified through its own built-in self-validation suite, which
is run as part of the `build-docs` CI job. When `build-docs` invokes
`dotnet sonarmark --validate --results artifacts/sonarmark-self-validation.trx`, the binary
executes its four internal validation scenarios and writes the outcomes to a TRX file. These
TRX results are collected by the CI pipeline and treated as test evidence.

The same self-validation evidence is also produced by the `integration-test` job, which
installs the newly built version of SonarMark and runs `sonarmark --validate` across nine
OS/runtime matrix combinations. Both the `build-docs` and `integration-test` TRX results
must pass for the build to succeed.

### Self-Validation Test Names

The four self-validation scenarios are baked into every SonarMark binary. Their names as
they appear in TRX output are:

- **`SonarMark_QualityGateRetrieval`** — confirms the tool can retrieve and process quality
  gate data from the SonarQube/SonarCloud API.
- **`SonarMark_IssuesRetrieval`** — confirms the tool can retrieve and process code issues
  from the SonarQube/SonarCloud API.
- **`SonarMark_HotSpotsRetrieval`** — confirms the tool can retrieve and process security
  hot-spots from the SonarQube/SonarCloud API.
- **`SonarMark_MarkdownReportGeneration`** — confirms the tool can render retrieved analysis
  data as a structured markdown report.

### Report Generation Evidence

Report generation is exercised transitively through the self-validation suite. The
`SonarMark_MarkdownReportGeneration` scenario directly exercises the same rendering code
path used when generating the SonarCloud quality report. This self-validation evidence is
sufficient to accept the report-generation feature as functional.

### Acceptance Criteria

The Shared Package integration is accepted when all four self-validation tests pass with
zero failures in every TRX file produced by the `build-docs` CI job.
