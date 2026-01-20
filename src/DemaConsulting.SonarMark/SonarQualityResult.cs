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
/// <param name="ProjectKey">Project key</param>
/// <param name="ProjectName">Project name</param>
/// <param name="QualityGateStatus">Quality gate status (OK, WARN, ERROR, or NONE)</param>
/// <param name="Conditions">Quality gate conditions and their statuses</param>
/// <param name="ServerUrl">Server URL for the SonarQube/SonarCloud instance</param>
internal sealed record SonarQualityResult(
    string ProjectKey,
    string ProjectName,
    string QualityGateStatus,
    IReadOnlyList<SonarQualityCondition> Conditions,
    string ServerUrl)
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

        // Add quality gate status heading
        sb.AppendLine($"{heading} Quality Gate Status: {QualityGateStatus}");
        sb.AppendLine();

        // Add project information with dashboard link
        var dashboardUrl = $"{ServerUrl.TrimEnd('/')}/dashboard?id={Uri.EscapeDataString(ProjectKey)}";
        sb.AppendLine($"**Project:** {ProjectName}");
        sb.AppendLine();
        sb.AppendLine($"**Dashboard:** {dashboardUrl}");
        sb.AppendLine();

        // Add conditions section if there are any
        if (Conditions.Count > 0)
        {
            sb.AppendLine($"{subHeading} Conditions");
            sb.AppendLine();

            // Add table header with alignment and appropriate column widths
            // Metric: wide (30) for variable-length names like "new_duplicated_lines_density"
            // Status: short (5) for fixed values "OK", "WARN", "ERROR"
            // Comparator: minimal (2) for fixed 2-char values "LT", "GT", etc.
            // Threshold/Actual: medium (8) for numeric values including decimals
            sb.AppendLine("| Metric | Status | Comparator | Threshold | Actual |");
            sb.AppendLine("|:-------------------------------|:-----:|:--:|--------:|-------:|");

            // Add table rows
            foreach (var condition in Conditions)
            {
                sb.Append($"| {condition.Metric} ");
                sb.Append($"| {condition.Status} ");
                sb.Append($"| {condition.Comparator} ");
                sb.Append($"| {condition.ErrorThreshold ?? ""} ");
                sb.AppendLine($"| {condition.ActualValue ?? ""} |");
            }
        }

        return sb.ToString();
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
