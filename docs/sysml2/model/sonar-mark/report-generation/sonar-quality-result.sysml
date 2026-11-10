package SonarMark {
    private import SoftwareItemMetadata::*;

    part def SonarQualityResult {
        doc /* Aggregates quality gate, issues, and hot-spots data and renders the markdown report. */

        metadata info : SoftwareInfo {
            description = "Aggregates quality gate, issues, and hot-spots data and renders the markdown report.";
            sourceFiles = ("src/DemaConsulting.SonarMark/ReportGeneration/SonarQualityResult.cs");
            testFiles = ("test/DemaConsulting.SonarMark.Tests/ReportGeneration/SonarQualityResultTests.cs");
            designFiles = ("docs/design/sonar-mark/report-generation/sonar-quality-result.md");
            verificationFiles = ("docs/verification/sonar-mark/report-generation/sonar-quality-result.md");
            requirementFiles = ("docs/reqstream/sonar-mark/report-generation/sonar-quality-result.yaml");
        }
    }
}
