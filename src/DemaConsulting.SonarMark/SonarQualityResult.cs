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

namespace DemaConsulting.SonarMark;

/// <summary>
///     Represents quality analysis results from SonarQube/SonarCloud
/// </summary>
/// <param name="ServerUrl">Server URL</param>
/// <param name="ProjectKey">Project key</param>
/// <param name="ProjectName">Project name</param>
/// <param name="QualityGateStatus">Quality gate status (OK, WARN, ERROR, or NONE)</param>
/// <param name="Conditions">Quality gate conditions and their statuses</param>
/// <param name="MetricNames">Dictionary mapping metric keys to friendly names</param>
/// <param name="Issues">List of issues found in the project</param>
/// <param name="HotSpots">List of security hot-spots found in the project</param>
internal sealed record SonarQualityResult(
    string ServerUrl,
    string ProjectKey,
    string ProjectName,
    string QualityGateStatus,
    IReadOnlyList<SonarQualityCondition> Conditions,
    IReadOnlyDictionary<string, string> MetricNames,
    IReadOnlyList<SonarIssue> Issues,
    IReadOnlyList<SonarHotSpot> HotSpots)
{
    /// <summary>
    ///     Converts the quality result to markdown format
    /// </summary>
    /// <param name="depth">The heading depth level (1-6) for the report title</param>
    /// <returns>Markdown representation of the quality result</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when depth is not between 1 and 6</exception>
    public string ToMarkdown(int depth)
    {
        if (depth < 1 || depth > 6)
        {
            throw new ArgumentOutOfRangeException(nameof(depth), depth, "Depth must be between 1 and 6");
        }

        var heading = new string('#', depth);
        var subHeadingDepth = Math.Min(depth + 1, 6);
        var subHeading = new string('#', subHeadingDepth);
        var sb = new System.Text.StringBuilder();

        AppendHeader(sb, heading);
        AppendConditionsSection(sb, subHeading);
        AppendIssuesSection(sb, subHeading);
        AppendHotSpotsSection(sb, subHeading);

        return sb.ToString();
    }

    /// <summary>
    ///     Appends the header section with project name, dashboard link, and quality gate status
    /// </summary>
    private void AppendHeader(System.Text.StringBuilder sb, string heading)
    {
        // Add project name as main heading
        sb.AppendLine($"{heading} {ProjectName} Sonar Analysis");
        sb.AppendLine();

        // Add dashboard link
        var dashboardUrl = $"{ServerUrl.TrimEnd('/')}/dashboard?id={Uri.EscapeDataString(ProjectKey)}";
        sb.AppendLine($"**Dashboard:** <{dashboardUrl}>");
        sb.AppendLine();

        // Add quality gate status as text content
        sb.AppendLine($"**Quality Gate Status:** {QualityGateStatus}");
        sb.AppendLine();
    }

    /// <summary>
    ///     Appends the conditions section if there are any conditions
    /// </summary>
    private void AppendConditionsSection(System.Text.StringBuilder sb, string subHeading)
    {
        if (Conditions.Count == 0)
        {
            return;
        }

        sb.AppendLine($"{subHeading} Conditions");
        sb.AppendLine();

        // Add table header with alignment and appropriate column widths
        sb.AppendLine("| Metric | Status | Comparator | Threshold | Actual |");
        sb.AppendLine("|:-------------------------------|:-----:|:--:|--------:|-------:|");

        // Add table rows
        foreach (var condition in Conditions)
        {
            AppendConditionRow(sb, condition);
        }

        sb.AppendLine();
    }

    /// <summary>
    ///     Appends a single condition row to the table
    /// </summary>
    private void AppendConditionRow(System.Text.StringBuilder sb, SonarQualityCondition condition)
    {
        // Use friendly name if available, otherwise fall back to metric key
        var metricName = MetricNames.TryGetValue(condition.Metric, out var friendlyName)
            ? friendlyName
            : condition.Metric;

        sb.Append($"| {metricName} ");
        sb.Append($"| {condition.Status} ");
        sb.Append($"| {condition.Comparator} ");
        sb.Append($"| {condition.ErrorThreshold ?? ""} ");
        sb.AppendLine($"| {condition.ActualValue ?? ""} |");
    }

    /// <summary>
    ///     Appends the issues section with count and details
    /// </summary>
    private void AppendIssuesSection(System.Text.StringBuilder sb, string subHeading)
    {
        sb.AppendLine($"{subHeading} Issues");
        sb.AppendLine();

        sb.AppendLine(FormatFoundText(Issues.Count, "issue"));
        sb.AppendLine();

        if (Issues.Count > 0)
        {
            foreach (var issue in Issues)
            {
                var component = CleanComponent(issue.Component);
                var lineInfo = issue.Line.HasValue ? $"({issue.Line})" : "";
                sb.AppendLine($"{component}{lineInfo}: {issue.Severity} {issue.Type} [{issue.Rule}] {issue.Message}");
                sb.AppendLine();
            }
        }
    }

    /// <summary>
    ///     Appends the security hot-spots section with count and details
    /// </summary>
    private void AppendHotSpotsSection(System.Text.StringBuilder sb, string subHeading)
    {
        sb.AppendLine($"{subHeading} Security Hot-Spots");
        sb.AppendLine();

        sb.AppendLine(FormatFoundText(HotSpots.Count, "security hot-spot"));
        sb.AppendLine();

        if (HotSpots.Count > 0)
        {
            foreach (var hotSpot in HotSpots)
            {
                var component = CleanComponent(hotSpot.Component);
                var lineInfo = hotSpot.Line.HasValue ? $"({hotSpot.Line})" : "";
                sb.AppendLine(
                    $"{component}{lineInfo}: {hotSpot.VulnerabilityProbability} [{hotSpot.SecurityCategory}] {hotSpot.Message}");
                sb.AppendLine();
            }
        }
    }

    /// <summary>
    ///     Formats a count with proper pluralization and "Found" prefix
    /// </summary>
    /// <param name="count">The count value</param>
    /// <param name="singularNoun">The singular form of the noun</param>
    /// <returns>Formatted text like "Found no issues", "Found 1 issue", or "Found 5 issues"</returns>
    private static string FormatFoundText(int count, string singularNoun)
    {
        return count switch
        {
            0 => $"Found no {singularNoun}s",
            1 => $"Found 1 {singularNoun}",
            _ => $"Found {count} {singularNoun}s"
        };
    }

    /// <summary>
    ///     Cleans the component path by removing the project key prefix
    /// </summary>
    /// <param name="component">Component path from SonarQube</param>
    /// <returns>Cleaned component path</returns>
    private string CleanComponent(string component)
    {
        var prefix = $"{ProjectKey}:";
        if (component.StartsWith(prefix, StringComparison.Ordinal))
        {
            return component.Substring(prefix.Length);
        }

        return component;
    }
}

/// <summary>
///     Represents a single quality gate condition
/// </summary>
/// <param name="Metric">Metric key (e.g., "new_coverage", "new_bugs")</param>
/// <param name="Comparator">Comparison operator (e.g., "LT", "GT")</param>
/// <param name="ErrorThreshold">Error threshold value</param>
/// <param name="ActualValue">Actual value from analysis</param>
/// <param name="Status">Condition status (OK, WARN, ERROR)</param>
internal sealed record SonarQualityCondition(
    string Metric,
    string Comparator,
    string? ErrorThreshold,
    string? ActualValue,
    string Status);
