package SonarMark {
    import OtsDependencies::*;
    import SharedDependencies::*;

    doc
    /*
     * SonarMark is a .NET command-line tool that generates comprehensive markdown reports
     * from SonarQube/SonarCloud analysis results.
     */

    part def SonarMarkSystem {
        doc /* The SonarMark system as a whole. */

        comment testRef /* Test: test/DemaConsulting.SonarMark.Tests/IntegrationTests.cs */
        comment designRef /* Design: docs/design/sonar-mark.md */
        comment verificationRef /* Verification: docs/verification/sonar-mark.md */
        comment reqRef /* Requirements: docs/reqstream/sonar-mark/sonar-mark.yaml */

        part program : Program;
        part cli : CliSubsystem;
        part sonarIntegration : SonarIntegrationSubsystem;
        part reportGeneration : ReportGenerationSubsystem;
        part selfTest : SelfTestSubsystem;

        part testResults : TestResults;

        part sonarMarkSharedPackage : SonarMarkSharedPackage;
    }
}
