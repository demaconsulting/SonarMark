package SonarMark {
    part def ReportGenerationSubsystem {
        doc /* Aggregates SonarQube data into a structured markdown quality report. */

        comment testRef /* Test: test/DemaConsulting.SonarMark.Tests/ReportGeneration/ReportGenerationTests.cs */
        comment designRef /* Design: docs/design/sonar-mark/report-generation.md */
        comment verificationRef /* Verification: docs/verification/sonar-mark/report-generation.md */
        comment reqRef /* Requirements: docs/reqstream/sonar-mark/report-generation/report-generation.yaml */

        part sonarQualityResult : SonarQualityResult;
    }
}
