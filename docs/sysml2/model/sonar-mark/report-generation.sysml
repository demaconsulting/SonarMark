package SonarMark {
    private import SoftwareItemMetadata::*;

    part def ReportGenerationSubsystem {
        doc /* Aggregates SonarQube data into a structured markdown quality report. */

        metadata info : SoftwareInfo {
            testFiles = ("test/DemaConsulting.SonarMark.Tests/ReportGeneration/ReportGenerationTests.cs");
            designFiles = ("docs/design/sonar-mark/report-generation.md");
            verificationFiles = ("docs/verification/sonar-mark/report-generation.md");
            requirementFiles = ("docs/reqstream/sonar-mark/report-generation/report-generation.yaml");
        }

        part sonarQualityResult : SonarQualityResult;
    }
}
