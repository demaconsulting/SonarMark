// Copyright (c) DEMA Consulting
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using DemaConsulting.SonarMark.ReportGeneration;
using DemaConsulting.SonarMark.SonarIntegration;

namespace DemaConsulting.SonarMark.Tests.ReportGeneration;

/// <summary>
///     Subsystem tests for the ReportGeneration subsystem (SonarQualityResult integrating issues, hot-spots, and quality gate).
/// </summary>
[TestClass]
public class ReportGenerationTests
{
    /// <summary>
    ///     Test that the subsystem renders a quality gate section including status and conditions.
    /// </summary>
    [TestMethod]
    public void ReportGeneration_QualityGateReport_IncludesStatusAndConditions()
    {
        // Arrange - create a quality result with ERROR gate status and one failing condition
        IReadOnlyList<SonarQualityCondition> conditions =
        [
            new("new_coverage", "LT", "80", "75.5", "ERROR")
        ];

        var metricNames = new Dictionary<string, string>
        {
            { "new_coverage", "Coverage on New Code" }
        };

        var result = new SonarQualityResult(
            "https://sonarcloud.io",
            "test_project",
            "Test Project",
            "ERROR",
            conditions,
            metricNames,
            [],
            []);

        // Act - render the subsystem output as markdown
        var markdown = result.ToMarkdown(1);

        // Assert - quality gate status and condition must appear in the rendered report
        Assert.IsNotNull(markdown);
        Assert.Contains("ERROR", markdown);
        Assert.Contains("Coverage on New Code", markdown);
    }

    /// <summary>
    ///     Test that the subsystem renders an issues section categorized by type and severity.
    /// </summary>
    [TestMethod]
    public void ReportGeneration_IssuesReport_CategorizesByTypeAndSeverity()
    {
        // Arrange - create a quality result with one bug issue
        var issue = new SonarIssue(
            Key: "issue-1",
            Rule: "squid:S1234",
            Severity: "MAJOR",
            Component: "com.example:src/main/java/Example.java",
            Line: 42,
            Message: "Fix this issue",
            Type: "BUG");

        var result = new SonarQualityResult(
            "https://sonarcloud.io",
            "test_project",
            "Test Project",
            "OK",
            [],
            new Dictionary<string, string>(),
            [issue],
            []);

        // Act - render the subsystem output as markdown
        var markdown = result.ToMarkdown(1);

        // Assert - issue details must appear in the rendered report
        Assert.IsNotNull(markdown);
        Assert.Contains("Fix this issue", markdown);
        Assert.Contains("MAJOR", markdown);
    }

    /// <summary>
    ///     Test that the subsystem renders a hot-spots section including priority and category.
    /// </summary>
    [TestMethod]
    public void ReportGeneration_HotSpotsReport_IncludesPriorityAndCategory()
    {
        // Arrange - create a quality result with one HIGH priority hot-spot
        var hotSpot = new SonarHotSpot(
            Key: "hs-1",
            Component: "com.example:src/main/java/Example.java",
            Line: 10,
            Message: "Review this security hot-spot",
            SecurityCategory: "xss",
            VulnerabilityProbability: "HIGH");

        var result = new SonarQualityResult(
            "https://sonarcloud.io",
            "test_project",
            "Test Project",
            "OK",
            [],
            new Dictionary<string, string>(),
            [],
            [hotSpot]);

        // Act - render the subsystem output as markdown
        var markdown = result.ToMarkdown(1);

        // Assert - hot-spot vulnerability probability and category must appear in the rendered report
        Assert.IsNotNull(markdown);
        Assert.Contains("Review this security hot-spot", markdown);
        Assert.Contains("HIGH", markdown);
    }
}
