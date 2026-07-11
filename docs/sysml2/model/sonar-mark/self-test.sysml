package SonarMark {
    part def SelfTestSubsystem {
        doc /* Self-validation test runner exercised at startup for OTS/BuildMark self-check evidence. */

        comment testRef /* Test: test/DemaConsulting.SonarMark.Tests/SelfTest/SelfTestTests.cs */
        comment designRef /* Design: docs/design/sonar-mark/self-test.md */
        comment verificationRef /* Verification: docs/verification/sonar-mark/self-test.md */
        comment reqRef /* Requirements: docs/reqstream/sonar-mark/self-test/self-test.yaml */

        part validation : Validation;
    }
}
