---
title: Errors and exit codes
summary: What the exit codes mean, and how to fix the common error messages.
aliases:
  - errors
  - exit-codes
tags:
  - troubleshooting
order: 7
see-also:
  - command-line
  - sonar-server
  - quality-gates
examples:
  - name: no-server
    kind: command
    language: shell
    description: A run without a server. Exits 1.
    content: '{{tool}} --project-key my-project'
    exit: 1
    expect: 'Error: --server parameter is required'
  - name: no-scheme
    kind: command
    language: shell
    description: A server address without https://, found before any request. Exits 1.
    content: '{{tool}} --server sonar.example.com --project-key my-project'
    exit: 1
    expect: 'Failed to get quality results'
  - name: bad-log
    kind: command
    language: shell
    description: A log file in a folder that does not exist. Exits 1.
    content: '{{tool}} --silent --log missing/output.log --version'
    exit: 1
    expect: 'Failed to open log file'
  - name: unknown-topic
    kind: command
    language: shell
    description: A help topic that does not exist. Exits 2.
    content: '{{tool}} help no-such-topic'
    exit: 2
    expect: "Unknown help card 'no-such-topic'"
---
**Exit codes.**

| Code | Meaning |
| --- | --- |
| 0 | Success. |
| 1 | An error, a failed self-test, or a failed gate with `--enforce`. |
| 2 | A wrong `help` argument or an unknown help topic. |

Errors are printed to standard error as `Error: ...` and set the exit
code. Errors found during a run are hidden by `--silent` and kept in the
`--log` file. Errors in the arguments, and a log file that cannot be
opened, are found before the run starts: they are always printed and
never logged ([[#bad-log]]).

**Common messages.**

- `--server parameter is required`, `--project-key parameter is required`:
  add the option; see [[sonar-server]].
- `HTTP 401 error from server`: the token is missing or wrong.
- `HTTP 403` or `HTTP 404 error from server`: usually the token is
  missing or its user cannot see the project, or the project key or
  branch is wrong.
- `An invalid request URI was provided`: add `https://` to `--server`
  ([[#no-scheme]]).
- `Invalid ... response: missing ...`: the address is usually not that of
  a SonarQube or SonarCloud server.
- `Quality gate failed`: `--enforce` found the gate status `ERROR`; see
  [[quality-gates]].
- `Failed to write report`: the folder of `--report` does not exist or
  the file is locked.
- `Failed to open log file`: the folder of `--log` does not exist or the
  file is locked.
- `Unsupported argument`: check the spelling against [[command-line]].
  `help` must come first.
- `requires a ... argument`: the option before it needs a value.
- `Unknown help card`: the topic does not exist ([[#unknown-topic]]).

**Unexpected errors.** When the server cannot be reached, or answers with
something that is not JSON, the tool prints `Unexpected error: ...`
followed by a stack trace, and exits with a code other than 0, 1, or 2.
Check the address, the network, and any proxy.
