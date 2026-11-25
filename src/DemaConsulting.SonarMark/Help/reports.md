---
title: Markdown reports
summary: Write the quality results as a Markdown report, and set its heading depth.
aliases:
  - report
  - markdown
tags:
  - reference
order: 4
see-also:
  - sonar-server
  - quality-gates
examples:
  - name: report
    kind: snippet
    language: shell
    save-as: report.sh
    description: Write the quality results of a project as a Markdown report. Needs the server.
    content: '{{tool}} --server https://sonar.example.com --project-key my-project --report quality.md'
  - name: depth
    kind: snippet
    language: shell
    save-as: depth.sh
    description: Start the report at heading level 2 to embed it in a larger document. Needs the server.
    content: '{{tool}} --server https://sonar.example.com --project-key my-project --report quality.md --depth 2'
  - name: bad-depth
    kind: command
    language: shell
    description: A depth outside 1 to 6 is an error. Exits 1.
    content: '{{tool}} --depth 7'
    exit: 1
    expect: '--depth requires a depth between 1 and 6'
---
`--report <file>` writes the results that the tool read from the server
to a Markdown file ([[#report]]). An existing file is overwritten. The tool
prints `Quality report generated successfully.` when the file is written.

**Layout.** The report has:

- a heading at level `--depth` (default 1) with the text
  `<project name> Sonar Analysis`
- the line `**Dashboard:**` with the link to the project on the server
- the line `**Quality Gate Status:**` with the status, such as `OK` or
  `ERROR`
- a `Conditions` table of the quality gate conditions (metric, status,
  comparator, threshold, and actual value), only when the gate has any
- an `Issues` section that starts with `Found no issues`,
  `Found 1 issue`, or `Found <n> issues`, then one line per open or
  confirmed issue: `<file>(<line>): <severity> <type> [<rule>] <message>`
- a `Security Hot-Spots` section in the same form, one line per hot-spot:
  `<file>(<line>): <probability> [<category>] <message>`

The subheadings are one level deeper than the heading, at most level 6.
File paths are shown without the project key prefix, and without
`(<line>)` when the finding has no line.

**Depth.** `--depth` sets the level of the first heading, so the report
can be included in a larger document ([[#depth]]). A value outside 1 to 6
is an error ([[#bad-depth]]). `--report-depth` is an older name of
`--depth`.

The report is meant to be read, for example in build notes or a pull
request. The `[snippet]` examples need a live server; see [[sonar-server]].
