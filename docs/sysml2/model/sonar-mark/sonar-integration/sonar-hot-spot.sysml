package SonarMark {
    private import SoftwareItemMetadata::*;

    part def SonarHotSpot {
        doc /* Data record representing a SonarQube security hot-spot. */

        metadata info : SoftwareInfo {
            description = "Data record representing a SonarQube security hot-spot.";
            sourceFiles = ("src/DemaConsulting.SonarMark/SonarIntegration/SonarHotSpot.cs");
            testFiles = ("test/DemaConsulting.SonarMark.Tests/SonarIntegration/SonarHotSpotTests.cs");
            designFiles = ("docs/design/sonar-mark/sonar-integration/sonar-hot-spot.md");
            verificationFiles = ("docs/verification/sonar-mark/sonar-integration/sonar-hot-spot.md");
            requirementFiles = ("docs/reqstream/sonar-mark/sonar-integration/sonar-hot-spot.yaml");
        }
    }
}
