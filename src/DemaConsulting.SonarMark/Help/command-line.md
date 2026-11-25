---
title: Command-line options
summary: Every option of the tool, and the order in which a run handles them.
aliases:
  - options
  - flags
tags:
  - reference
order: 2
see-also:
  - sonar-server
  - troubleshooting
examples:
  - name: usage
    kind: command
    language: shell
    description: Print the short usage text. It ends with a pointer to this help.
    content: '{{tool}} --help'
    expect: "Run '{{tool}} help' for topics and examples."
  - name: silent-log
    kind: command
    language: shell
    description: Hide the console output and keep it in a log file.
    content: '{{tool}} --silent --log output.log --version'
  - name: unknown-option
    kind: command
    language: shell
    description: An unknown argument is an error, printed even with --silent. Exits 1.
    content: '{{tool}} --silent --unknown'
    exit: 1
    expect: "Error: Unsupported argument '--unknown'"
---
Options start with `--` (or a single character after `-`) and may be
given in any order.

| Option | Meaning |
| --- | --- |
| `-v`, `--version` | Print only the version. |
| `-?`, `-h`, `--help` | Print the short usage text. |
| `--silent` | Hide console output. Errors still set exit code 1. |
| `--log <file>` | Also write the output to a file. |
| `--server <url>` | The server to read from; see [[sonar-server]]. |
| `--project-key <key>` | The key of the project on the server. |
| `--branch <name>` | The branch to read; default the main branch. |
| `--token <token>` | The token; default `SONAR_TOKEN`. |
| `--report <file>` | Write a Markdown report; see [[reports]]. |
| `--depth <n>` | First heading level of a report, 1 to 6; default 1. |
| `--enforce` | Exit 1 when the gate fails; see [[quality-gates]]. |
| `--validate` | Run the self-tests; see [[self-validation]]. |
| `--results <file>` | Write the self-test results as `.trx` or `.xml`. |

`--report-depth` is an older name of `--depth`, and `--result` of
`--results`; both still work.

**Order.** `--version`, when all arguments are valid, prints only the
version and skips everything else. Otherwise the tool prints a banner, then handles the first of `--help`,
`--validate`, or reading the server. The command that [[#silent-log]]
prints writes the version to the log file only.

**Errors.** An unknown argument, or an option without its value, prints
`Error: ...` and exits 1 ([[#unknown-option]]). A value that is itself an
option, as in `--server --enforce`, is an error too. These errors are
found before `--silent` and `--log` take effect, so they are always
printed and never logged.

**Help.** `help` is a command, not an option, and must be the first
argument. `--silent` and `--log` do not apply to it, and an unknown topic
exits 2.
