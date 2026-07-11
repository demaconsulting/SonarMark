// Views rendered into docs/design/generated/ for the Design document.
// Reopens the SonarMark package so each 'expose' statement can reference
// its target part def directly. Per SysML2Tools syntax, 'expose' is only valid
// inside a named 'view' usage (not a 'view def' definition), and it - not
// 'render' - is what scopes the rendered diagram content.
package SonarMark {
    view SoftwareStructureView {
        expose SonarMark;
    }

    view SonarMarkView {
        expose SonarMarkSystem;
    }

    view CliView {
        expose CliSubsystem;
    }

    view SonarIntegrationView {
        expose SonarIntegrationSubsystem;
    }

    view ReportGenerationView {
        expose ReportGenerationSubsystem;
    }

    view SelfTestView {
        expose SelfTestSubsystem;
    }
}
