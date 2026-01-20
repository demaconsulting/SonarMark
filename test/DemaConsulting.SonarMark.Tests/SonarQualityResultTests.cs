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

namespace DemaConsulting.SonarMark.Tests;

/// <summary>
///     Tests for SonarQualityResult class
/// </summary>
[TestClass]
public class SonarQualityResultTests
{
    /// <summary>
    ///     Test that SonarQualityResult can be created with all required properties
    /// </summary>
    [TestMethod]
    public void SonarQualityResult_Constructor_AllProperties_CreatesInstance()
    {
        // Arrange & Act - create quality result with primary constructor
        var conditions = new List<SonarQualityCondition>
        {
            new("new_coverage", "LT", "80", "75.5", "ERROR")
        };

        var metricNames = new Dictionary<string, string>
        {
            { "new_coverage", "Coverage on New Code" }
        };

        var result = new SonarQualityResult(
            "test_project",
            "Test Project Name",
            "ERROR",
            conditions,
            metricNames,
            new List<SonarIssue>(),
            new List<SonarHotSpot>());

        // Assert - verify all properties are set correctly
        Assert.AreEqual("test_project", result.ProjectKey);
        Assert.AreEqual("Test Project Name", result.ProjectName);
        Assert.AreEqual("ERROR", result.QualityGateStatus);
        Assert.HasCount(1, result.Conditions);
        Assert.HasCount(1, result.MetricNames);
    }

    /// <summary>
    ///     Test ToMarkdown with depth 1 produces correct output
    /// </summary>
    [TestMethod]
    public void SonarQualityResult_ToMarkdown_Depth1_ProducesCorrectOutput()
    {
        // Arrange
        var conditions = new List<SonarQualityCondition>
        {
            new("new_coverage", "LT", "80", "75.5", "ERROR"),
            new("new_bugs", "GT", "0", "2", "ERROR")
        };

        var metricNames = new Dictionary<string, string>
        {
            { "new_coverage", "Coverage on New Code" },
            { "new_bugs", "New Bugs" }
        };

        var result = new SonarQualityResult(
            "test_project",
            "Test Project",
            "ERROR",
            conditions,
            metricNames,
            new List<SonarIssue>(),
            new List<SonarHotSpot>());

        // Act
        var markdown = result.ToMarkdown(1);

        // Assert - verify the markdown contains expected elements
        Assert.IsNotNull(markdown);
        Assert.Contains("# Test Project Sonar Analysis", markdown);
        Assert.Contains("**Quality Gate Status:** ERROR", markdown);
        Assert.Contains("## Conditions", markdown);
        Assert.Contains("| Metric | Status | Comparator | Threshold | Actual |", markdown);
        Assert.Contains("|:-------------------------------|:-----:|:--:|--------:|-------:|", markdown);
        Assert.Contains("| Coverage on New Code | ERROR | LT | 80 | 75.5 |", markdown);
        Assert.Contains("| New Bugs | ERROR | GT | 0 | 2 |", markdown);
    }

    /// <summary>
    ///     Test ToMarkdown with depth 3 uses correct heading levels
    /// </summary>
    [TestMethod]
    public void SonarQualityResult_ToMarkdown_Depth3_UsesCorrectHeadingLevels()
    {
        // Arrange
        var conditions = new List<SonarQualityCondition>
        {
            new("new_coverage", "LT", "80", "75.5", "ERROR")
        };

        var metricNames = new Dictionary<string, string>();

        var result = new SonarQualityResult(
            "test_project",
            "Test Project",
            "ERROR",
            conditions,
            metricNames,
            new List<SonarIssue>(),
            new List<SonarHotSpot>());

        // Act
        var markdown = result.ToMarkdown(3);

        // Assert - verify heading levels
        Assert.Contains("### Test Project Sonar Analysis", markdown);
        Assert.Contains("#### Conditions", markdown);
    }

    /// <summary>
    ///     Test ToMarkdown with empty conditions list
    /// </summary>
    [TestMethod]
    public void SonarQualityResult_ToMarkdown_NoConditions_ExcludesConditionsSection()
    {
        // Arrange
        var metricNames = new Dictionary<string, string>();

        var result = new SonarQualityResult(
            "test_project",
            "Test Project",
            "OK",
            new List<SonarQualityCondition>(),
            metricNames,
            new List<SonarIssue>(),
            new List<SonarHotSpot>());

        // Act
        var markdown = result.ToMarkdown(1);

        // Assert - verify conditions section is not present
        Assert.Contains("# Test Project Sonar Analysis", markdown);
        Assert.Contains("**Quality Gate Status:** OK", markdown);
        Assert.DoesNotContain("## Conditions", markdown);
    }

    /// <summary>
    ///     Test ToMarkdown with null threshold and actual values
    /// </summary>
    [TestMethod]
    public void SonarQualityResult_ToMarkdown_NullThresholdAndActual_ExcludesNullValues()
    {
        // Arrange
        var conditions = new List<SonarQualityCondition>
        {
            new("new_coverage", "LT", null, null, "OK")
        };

        var metricNames = new Dictionary<string, string>();

        var result = new SonarQualityResult(
            "test_project",
            "Test Project",
            "OK",
            conditions,
            metricNames,
            new List<SonarIssue>(),
            new List<SonarHotSpot>());

        // Act
        var markdown = result.ToMarkdown(1);

        // Assert - verify null values are shown as empty in table
        Assert.Contains("| Metric | Status | Comparator | Threshold | Actual |", markdown);
        Assert.Contains("| new_coverage | OK | LT |  |  |", markdown);
    }

    /// <summary>
    ///     Test ToMarkdown with depth less than 1 throws exception
    /// </summary>
    [TestMethod]
    public void SonarQualityResult_ToMarkdown_DepthLessThan1_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var metricNames = new Dictionary<string, string>();

        var result = new SonarQualityResult(
            "test_project",
            "Test Project",
            "OK",
            new List<SonarQualityCondition>(),
            metricNames,
            new List<SonarIssue>(),
            new List<SonarHotSpot>());

        // Act & Assert
        try
        {
            result.ToMarkdown(0);
            Assert.Fail("Expected ArgumentOutOfRangeException was not thrown");
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Assert.AreEqual("depth", ex.ParamName);
            Assert.Contains("Depth must be between 1 and 6", ex.Message);
        }
    }

    /// <summary>
    ///     Test ToMarkdown with depth greater than 6 throws exception
    /// </summary>
    [TestMethod]
    public void SonarQualityResult_ToMarkdown_DepthGreaterThan6_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var metricNames = new Dictionary<string, string>();

        var result = new SonarQualityResult(
            "test_project",
            "Test Project",
            "OK",
            new List<SonarQualityCondition>(),
            metricNames,
            new List<SonarIssue>(),
            new List<SonarHotSpot>());

        // Act & Assert
        try
        {
            result.ToMarkdown(7);
            Assert.Fail("Expected ArgumentOutOfRangeException was not thrown");
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Assert.AreEqual("depth", ex.ParamName);
            Assert.Contains("Depth must be between 1 and 6", ex.Message);
        }
    }

    /// <summary>
    ///     Test ToMarkdown with maximum depth of 6
    /// </summary>
    [TestMethod]
    public void SonarQualityResult_ToMarkdown_Depth6_ProducesCorrectOutput()
    {
        // Arrange
        var conditions = new List<SonarQualityCondition>
        {
            new("new_coverage", "LT", "80", "75.5", "ERROR")
        };

        var metricNames = new Dictionary<string, string>();

        var result = new SonarQualityResult(
            "test_project",
            "Test Project",
            "ERROR",
            conditions,
            metricNames,
            new List<SonarIssue>(),
            new List<SonarHotSpot>());

        // Act
        var markdown = result.ToMarkdown(6);

        // Assert - verify heading levels (subheading is capped at 6)
        Assert.Contains("###### Test Project Sonar Analysis", markdown);
        Assert.Contains("###### Conditions", markdown);
    }

    /// <summary>
    ///     Test ToMarkdown with WARN status
    /// </summary>
    [TestMethod]
    public void SonarQualityResult_ToMarkdown_WarnStatus_ProducesCorrectOutput()
    {
        // Arrange
        var conditions = new List<SonarQualityCondition>
        {
            new("new_coverage", "LT", "80", "78.5", "WARN")
        };

        var metricNames = new Dictionary<string, string>();

        var result = new SonarQualityResult(
            "test_project",
            "Test Project",
            "WARN",
            conditions,
            metricNames,
            new List<SonarIssue>(),
            new List<SonarHotSpot>());

        // Act
        var markdown = result.ToMarkdown(1);

        // Assert
        Assert.Contains("# Test Project Sonar Analysis", markdown);
        Assert.Contains("**Quality Gate Status:** WARN", markdown);
        Assert.Contains("| new_coverage | WARN | LT | 80 | 78.5 |", markdown);
    }

    /// <summary>
    ///     Test ToMarkdown uses friendly metric names when available
    /// </summary>
    [TestMethod]
    public void SonarQualityResult_ToMarkdown_WithFriendlyNames_UsesFriendlyNames()
    {
        // Arrange
        var conditions = new List<SonarQualityCondition>
        {
            new("new_coverage", "LT", "80", "75.5", "ERROR"),
            new("unknown_metric", "GT", "0", "5", "ERROR")
        };

        var metricNames = new Dictionary<string, string>
        {
            { "new_coverage", "Coverage on New Code" }
            // unknown_metric not in dictionary - should fall back to key
        };

        var result = new SonarQualityResult(
            "test_project",
            "Test Project",
            "ERROR",
            conditions,
            metricNames,
            new List<SonarIssue>(),
            new List<SonarHotSpot>());

        // Act
        var markdown = result.ToMarkdown(1);

        // Assert - verify friendly name is used when available, key used as fallback
        Assert.Contains("| Coverage on New Code | ERROR | LT | 80 | 75.5 |", markdown);
        Assert.Contains("| unknown_metric | ERROR | GT | 0 | 5 |", markdown);
    }

    /// <summary>
    ///     Test SonarQualityCondition can be created with all properties
    /// </summary>
    [TestMethod]
    public void SonarQualityCondition_Constructor_AllProperties_CreatesInstance()
    {
        // Arrange & Act
        var condition = new SonarQualityCondition(
            "new_coverage",
            "LT",
            "80",
            "75.5",
            "ERROR");

        // Assert
        Assert.AreEqual("new_coverage", condition.Metric);
        Assert.AreEqual("LT", condition.Comparator);
        Assert.AreEqual("80", condition.ErrorThreshold);
        Assert.AreEqual("75.5", condition.ActualValue);
        Assert.AreEqual("ERROR", condition.Status);
    }
}
