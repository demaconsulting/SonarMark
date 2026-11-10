package SonarMark {
    private import SoftwareItemMetadata::*;

    part def SonarIntegrationSubsystem {
        doc /* SonarQube/SonarCloud REST API integration and data-model definitions. */

        metadata info : SoftwareInfo {
            description = "SonarQube/SonarCloud REST API integration and data-model definitions.";
            testFiles = ("test/DemaConsulting.SonarMark.Tests/SonarIntegration/SonarIntegrationTests.cs");
            designFiles = ("docs/design/sonar-mark/sonar-integration.md");
            verificationFiles = ("docs/verification/sonar-mark/sonar-integration.md");
            requirementFiles = ("docs/reqstream/sonar-mark/sonar-integration/sonar-integration.yaml");
        }

        part sonarQubeClient : SonarQubeClient;
        part sonarHotSpot : SonarHotSpot;
        part sonarIssue : SonarIssue;
    }
}
