# Troubleshooting

## Authentication Issues

**Problem**: `401 Unauthorized` error.

**Solutions**:

- Verify your token is valid and not expired
- Ensure the token has appropriate permissions
- Check if the project exists and you have access to it
- For SonarCloud, verify you're using a user token, not a project analysis token

## Connection Issues

**Problem**: Cannot connect to SonarQube/SonarCloud server.

**Solutions**:

- Verify the server URL is correct
- Check network connectivity and firewall rules
- Ensure the server is accessible from your environment
- Verify SSL/TLS certificates are valid
- For self-hosted SonarQube, check if the server is running

## Project Not Found

**Problem**: `404 Not Found` or project doesn't exist error.

**Solutions**:

- Verify the project key is correct (case-sensitive)
- Ensure the project exists in SonarQube/SonarCloud
- Check if you have access to the project
- For branches, verify the branch exists in SonarQube/SonarCloud

## Branch Issues

**Problem**: Branch not found or incorrect data.

**Solutions**:

- Verify the branch name is correct (case-sensitive)
- Ensure the branch has been analyzed in SonarQube/SonarCloud
- Check if the branch is a long-lived or short-lived branch
- Use the exact branch name as shown in SonarQube/SonarCloud UI

## Quality Gate Failures

**Problem**: Quality gate fails unexpectedly with `--enforce`.

**Solutions**:

- Review the quality gate conditions in the console output
- Check the detailed report to see which conditions failed
- Verify quality gate configuration in SonarQube/SonarCloud
- Consider if the failure is expected (e.g., new issues introduced)

## Report Generation Issues

**Problem**: Report file is not generated or is empty.

**Solutions**:

- Check file permissions in the output directory
- Verify the output path is valid and accessible
- Ensure there's enough disk space
- Check the log output for specific error messages

## Validation Failures

**Problem**: Self-validation tests fail.

**Solutions**:

- Update to the latest version of SonarMark
- Check if there are any known issues in the GitHub repository
- Report the issue with full validation output if the problem persists

## Performance Issues

**Problem**: SonarMark takes too long to execute.

**Solutions**:

- Check network latency to the SonarQube/SonarCloud server
- Verify the server is responsive (not overloaded)
- Consider caching results if running frequently
- For large projects, be patient as data retrieval may take time

## Exit Codes

SonarMark uses the following exit codes:

- `0`: Success (or quality gate passed with `--enforce`)
- `1`: Error occurred or quality gate failed (with `--enforce`)

Use these exit codes in scripts for error handling:

```bash
#!/bin/bash
if sonarmark --server https://sonarcloud.io \
  --project-key my-org_my-project \
  --enforce; then
  echo "Quality gate passed!"
else
  echo "Quality gate failed!"
  exit 1
fi
```

## Additional Resources

- [GitHub Repository](https://github.com/demaconsulting/SonarMark)
- [Issue Tracker](https://github.com/demaconsulting/SonarMark/issues)
- [Security Policy](https://github.com/demaconsulting/SonarMark/blob/main/SECURITY.md)
- [SonarQube Documentation](https://docs.sonarqube.org/latest/)
- [SonarCloud Documentation](https://docs.sonarcloud.io/)
