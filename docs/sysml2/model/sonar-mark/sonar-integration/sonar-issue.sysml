package SonarMark {
    private import SoftwareItemMetadata::*;

    part def SonarIssue {
        doc /* Data record representing a SonarQube issue. */

        metadata info : SoftwareInfo {
            description = "Data record representing a SonarQube issue.";
            sourceFiles = ("src/DemaConsulting.SonarMark/SonarIntegration/SonarIssue.cs");
            testFiles = ("test/DemaConsulting.SonarMark.Tests/SonarIntegration/SonarIssueTests.cs");
            designFiles = ("docs/design/sonar-mark/sonar-integration/sonar-issue.md");
            verificationFiles = ("docs/verification/sonar-mark/sonar-integration/sonar-issue.md");
            requirementFiles = ("docs/reqstream/sonar-mark/sonar-integration/sonar-issue.yaml");
        }
    }
}
