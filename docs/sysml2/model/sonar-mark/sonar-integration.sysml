package SonarMark {
    part def SonarIntegrationSubsystem {
        doc /* SonarQube/SonarCloud REST API integration and data-model definitions. */

        comment testRef /* Test: test/DemaConsulting.SonarMark.Tests/SonarIntegration/SonarIntegrationTests.cs */
        comment designRef /* Design: docs/design/sonar-mark/sonar-integration.md */
        comment verificationRef /* Verification: docs/verification/sonar-mark/sonar-integration.md */
        comment reqRef /* Requirements: docs/reqstream/sonar-mark/sonar-integration/sonar-integration.yaml */

        part sonarQubeClient : SonarQubeClient;
        part sonarHotSpot : SonarHotSpot;
        part sonarIssue : SonarIssue;
    }
}
