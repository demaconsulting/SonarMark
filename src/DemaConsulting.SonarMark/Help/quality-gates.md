---
title: Quality gates
summary: Fail a build when the quality gate of the project fails.
aliases:
  - enforce
  - quality-gate
  - gate
tags:
  - reference
order: 5
see-also:
  - reports
  - troubleshooting
examples:
  - name: enforce
    kind: snippet
    language: shell
    save-as: enforce.sh
    description: Exit 1 when the quality gate fails. Needs the server.
    content: '{{tool}} --server https://sonar.example.com --project-key my-project --enforce'
  - name: enforce-report
    kind: snippet
    language: shell
    save-as: enforce-report.sh
    description: Write the report and fail the build when the gate fails. Needs the server.
    content: '{{tool}} --server https://sonar.example.com --project-key my-project --enforce --report quality.md'
---
The server computes the quality gate of the project from its analysis.
{{tool}} reads the status of the gate and prints it as
`Quality Gate Status: <status>`.

**Enforce.** With `--enforce`, a status of `ERROR` prints
`Error: Quality gate failed` and the run exits 1 ([[#enforce]]). Any other
status, such as `OK` or `NONE`, passes. Without `--enforce` the run exits
0 whatever the status.

**Report.** The report of `--report` is still written when the gate fails
([[#enforce-report]]), so a failed build keeps the reasons: the
`Conditions` table of the report shows each condition of the gate with
its threshold and actual value; see [[reports]].

**Silent runs.** With `--silent` the exit code still reports a failed
gate, and `--log` keeps the error message.

**In a pipeline.** Run {{tool}} after the analysis has finished on the
server. Publish the report as a build artifact or in the job summary.

Both examples need a live server and a token; see [[sonar-server]].
