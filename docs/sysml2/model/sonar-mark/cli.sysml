package SonarMark {
    part def CliSubsystem {
        doc /* Command-line argument parsing and program-output routing. */

        comment testRef /* Test: test/DemaConsulting.SonarMark.Tests/Cli/CliTests.cs */
        comment designRef /* Design: docs/design/sonar-mark/cli.md */
        comment verificationRef /* Verification: docs/verification/sonar-mark/cli.md */
        comment reqRef /* Requirements: docs/reqstream/sonar-mark/cli/cli.yaml */

        part context : Context;
    }
}
