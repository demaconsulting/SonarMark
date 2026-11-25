---
title: SonarQube and SonarCloud servers
summary: Name the server, project, and branch to read, and give the tool a token.
aliases:
  - server
  - sonarqube
  - sonarcloud
  - token
tags:
  - reference
order: 3
see-also:
  - reports
  - troubleshooting
examples:
  - name: sonarcloud
    kind: snippet
    language: shell
    save-as: sonarcloud.sh
    description: Read the main branch of a SonarCloud project. Needs the server.
    content: '{{tool}} --server https://sonarcloud.io --project-key my-org_my-project'
  - name: branch
    kind: snippet
    language: shell
    save-as: branch.sh
    description: Read one branch of a project on your own SonarQube server. Needs the server.
    content: '{{tool}} --server https://sonar.example.com --project-key my-project --branch develop'
  - name: missing-key
    kind: command
    language: shell
    description: A server without a project key is an error, found before any request. Exits 1.
    content: '{{tool}} --server https://sonar.example.com'
    exit: 1
    expect: 'Error: --project-key parameter is required'
---
{{tool}} reads the results of one project from a SonarQube server or from
SonarCloud. Every run other than `--help`, `--version`, and `--validate`
needs `--server` and `--project-key`; without them the tool stops before
it sends any request ([[#missing-key]]).

**Server.** `--server` takes the full address with its scheme, such as
`https://sonarcloud.io` ([[#sonarcloud]]) or the address of your own
SonarQube server. A trailing `/` is ignored.

**Project and branch.** `--project-key` is the key that the server shows
for the project. `--branch` reads one branch ([[#branch]]); without it the
server reports its main branch.

**Token.** The token comes from `--token`, or else from the `SONAR_TOKEN`
environment variable; `--token` wins. Prefer the variable, for example
set from a CI secret, so that the token stays out of command lines and
logs. {{tool}} never prints it. Use the token of a user who can browse the
project. Without a token the requests are anonymous, which works only
when the server allows anonymous access to the project.

**What is read.** The tool only reads: the project name, the quality gate
status and its conditions, the metric names, the open and confirmed
issues, and the security hot-spots. It changes nothing on the server.

The `[snippet]` examples need a live server and a token. Replace the
server, project key, and branch with your own.
