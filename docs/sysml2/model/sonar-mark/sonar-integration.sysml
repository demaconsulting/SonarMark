package SonarMark_SonarIntegrationSubsystem {
    private import SoftwareItemMetadata::*;
    public import SonarMark_SonarQubeClient::*;
    public import SonarMark_SonarHotSpot::*;
    public import SonarMark_SonarIssue::*;

    part def SonarIntegrationSubsystem {
        doc /* SonarQube/SonarCloud REST API integration and data-model definitions. */

        metadata info : SoftwareInfo {
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
