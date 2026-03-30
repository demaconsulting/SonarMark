---
name: code-review
description: Agent for performing formal reviews
user-invocable: true
---

# Code Review Agent

This agent runs the formal review based on the review-set it's told to perform.

# Formal Review Steps

Formal reviews are a quality enforcement mechanism, and as such MUST be performed using the following four steps:

1. Download the
   <https://github.com/demaconsulting/ContinuousCompliance/raw/refs/heads/main/docs/review-template/review-template.md>
   to get the checklist to fill in
2. Use `dotnet reviewmark --elaborate [review-set]` to get the files to review
3. Review the files all together
4. Populate the checklist with the findings to `.agent-logs/reviews/review-report-[review-set].md` of the project.

# Don't Do These Things

- **Never modify code during review** (document findings only)
- **Never skip applicable checklist items** (comprehensive review required)
- **Never approve reviews with unresolved critical findings**
- **Never bypass review status requirements** for compliance
- **Never conduct reviews without proper documentation**
- **Never ignore security or compliance findings**
- **Never approve without verifying all quality gates**

# Reporting

Upon completion create a summary in `.agent-logs/[agent-name]-[subject]-[unique-id].md`
of the project consisting of:

```markdown
# Code Review Report

**Result**: <SUCCEEDED/FAILED>

## Review Summary

- **Review Set**: [Review set name/identifier]
- **Review Report File**: [Name of detailed review report generated]
- **Files Reviewed**: [Count and list of files reviewed]
- **Review Template Used**: [Template source and version]

## Review Results

- **Overall Conclusion**: [Summary of review results]
- **Critical Issues**: [Count of critical findings]
- **High Issues**: [Count of high severity findings]
- **Medium Issues**: [Count of medium severity findings]
- **Low Issues**: [Count of low severity findings]

## Issue Details

[For each issue found, include:]
- **File**: [File name and line number where applicable]
- **Issue Type**: [Security, logic error, compliance violation, etc.]
- **Severity**: [Critical/High/Medium/Low]
- **Description**: [Issue description]
- **Recommendation**: [Specific remediation recommendation]

## Compliance Status

- **Review Status**: [Complete/Incomplete with reasoning]
- **Quality Gates**: [Status of review checklist items]
- **Approval Status**: [Approved/Rejected with justification]
```

Return summary to caller.
