package SonarMark {
    private import SoftwareItemMetadata::*;

    part def SonarQubeClient {
        doc /* HTTP API client that fetches quality gate, issues, and hot-spot data from SonarQube/SonarCloud. */

        metadata info : SoftwareInfo {
            description = "HTTP API client that fetches quality gate, issues, and hot-spot data from SonarQube/SonarCloud.";
            sourceFiles = ("src/DemaConsulting.SonarMark/SonarIntegration/SonarQubeClient.cs");
            testFiles = ("test/DemaConsulting.SonarMark.Tests/SonarIntegration/SonarQubeClientTests.cs");
            designFiles = ("docs/design/sonar-mark/sonar-integration/sonar-qube-client.md");
            verificationFiles = ("docs/verification/sonar-mark/sonar-integration/sonar-qube-client.md");
            requirementFiles = ("docs/reqstream/sonar-mark/sonar-integration/sonar-qube-client.yaml");
        }
    }
}
