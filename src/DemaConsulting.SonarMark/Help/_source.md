---
name: sonarmark
schema: 2
title: SonarMark
summary: Built-in help of SonarMark, the SonarQube and SonarCloud report tool.
---
{{tool}} reads the quality gate, issues, and security hot-spots of a project from a SonarQube or SonarCloud
server, writes them as a Markdown report, and can fail a build when the quality gate fails.

Every operation is an option. The only command is `{{tool}} help`, which must be the first argument and takes topic
names, such as `{{tool}} help command-line`.
