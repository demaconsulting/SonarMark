# Introduction

This document contains the software design specification for the SonarMark project.

## Purpose

SonarMark is a .NET command-line tool that generates comprehensive markdown reports from
SonarQube/SonarCloud analysis results. This design document describes the internal structure
of each software unit, explaining the design decisions and implementation approach for each
class in the codebase.

## Scope

This design document covers the following software units:

- **Context** — command-line argument parsing and program output management
- **Program** — main entry point and top-level program logic
- **SonarHotSpot** — data record representing a SonarQube security hot-spot
- **SonarIssue** — data record representing a SonarQube issue
- **SonarQualityResult** — data record representing quality analysis results with markdown generation
- **SonarQubeClient** — HTTP client for the SonarQube/SonarCloud API
- **Validation** — self-validation functionality for the tool

## Audience

This document is intended for:

- Software developers working on SonarMark
- Quality assurance teams validating the implementation against requirements
- Reviewers performing formal code reviews
