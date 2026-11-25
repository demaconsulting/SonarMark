---
title: Getting started
summary: Install the tool, check that it works, and write a first report.
aliases:
  - start
  - quickstart
  - examples
tags:
  - tutorial
order: 1
see-also:
  - command-line
  - sonar-server
  - reports
examples:
  - name: version
    kind: command
    language: shell
    description: Print the version and exit.
    content: '{{tool}} --version'
  - name: validate
    kind: command
    language: shell
    description: Run the built-in self-tests. Exits 0 when every test passes.
    content: '{{tool}} --validate'
    expect: 'Failed: 0'
  - name: first-report
    kind: snippet
    language: shell
    save-as: first-report.sh
    description: Read a SonarCloud project and write its quality report. Needs the server.
    content: '{{tool}} --server https://sonarcloud.io --project-key my-org_my-project --report quality.md'
  - name: topic
    kind: command
    language: shell
    description: Print a help topic, here the list of options.
    content: '{{tool}} help command-line'
    expect: '--server'
---
{{tool}} {{version}} is a .NET tool.
It reads the analysis results of a project from a SonarQube or SonarCloud
server and turns them into a Markdown report. It reads only from the
server that `--server` names, and writes only the files that options
name, apart from a temporary folder that `--validate` uses.

1. **Install it.** Install the tool from NuGet, globally or in a local tool
   manifest:

   ```bash
   dotnet tool install -g DemaConsulting.SonarMark
   ```

2. **Check it.** The command that [[#version]] prints shows the version.
   The command from [[#validate]] runs the built-in self-tests without a
   server and exits 0 when all pass; see [[self-validation]].
3. **Write a report.** Set the `SONAR_TOKEN` environment variable to a
   token of the server, then run a command like [[#first-report]] with
   your project key. It prints the quality gate status and the numbers of
   issues and hot-spots, and writes the report. [[sonar-server]] explains
   the server and the token, and [[reports]] describes the report.
4. **Gate the build.** Add `--enforce` to exit 1 when the quality gate
   fails; see [[quality-gates]].

Print any help topic with `{{tool}} help <topic>`, as in the command that
[[#topic]] prints. Add `--search <words>` to find a topic, and `--json` to
get any help answer as JSON.
