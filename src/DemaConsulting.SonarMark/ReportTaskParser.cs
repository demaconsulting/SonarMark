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
///  Parser for SonarQube report-task.txt files
/// </summary>
internal static class ReportTaskParser
{
    /// <summary>
    ///  Report task file name
    /// </summary>
    private const string ReportTaskFileName = "report-task.txt";

    /// <summary>
    ///  Finds the report-task.txt file in the specified directory or its subdirectories
    /// </summary>
    /// <param name="searchDirectory">Directory to search in</param>
    /// <returns>Full path to the report-task.txt file if found, null otherwise</returns>
    public static string? FindReportTask(string searchDirectory)
    {
        if (!Directory.Exists(searchDirectory))
        {
            return null;
        }

        try
        {
            var files = Directory.GetFiles(searchDirectory, ReportTaskFileName, SearchOption.AllDirectories);
            return files.Length > 0 ? files[0] : null;
        }
        catch (UnauthorizedAccessException)
        {
            return null;
        }
    }

    /// <summary>
    ///  Parses a report-task.txt file
    /// </summary>
    /// <param name="filePath">Path to the report-task.txt file</param>
    /// <returns>Parsed ReportTask object</returns>
    /// <exception cref="ArgumentException">Thrown when file doesn't exist or required fields are missing</exception>
    public static ReportTask Parse(string filePath)
    {
        // Verify the file exists
        if (!File.Exists(filePath))
        {
            throw new ArgumentException($"File not found: {filePath}", nameof(filePath));
        }

        // Parse all key-value pairs from the file
        var properties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var line in File.ReadLines(filePath))
        {
            var trimmedLine = line.Trim();
            if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith('#'))
            {
                continue;
            }

            var separatorIndex = trimmedLine.IndexOf('=');
            if (separatorIndex > 0)
            {
                var key = trimmedLine[..separatorIndex].Trim();
                var value = trimmedLine[(separatorIndex + 1)..].Trim();
                properties[key] = value;
            }
        }

        // Validate projectKey field is present
        if (!properties.TryGetValue("projectKey", out var projectKey) || string.IsNullOrWhiteSpace(projectKey))
        {
            throw new ArgumentException("Missing required field: projectKey", nameof(filePath));
        }

        // Validate serverUrl field is present
        if (!properties.TryGetValue("serverUrl", out var serverUrl) || string.IsNullOrWhiteSpace(serverUrl))
        {
            throw new ArgumentException("Missing required field: serverUrl", nameof(filePath));
        }

        // Validate ceTaskId field is present
        if (!properties.TryGetValue("ceTaskId", out var ceTaskId) || string.IsNullOrWhiteSpace(ceTaskId))
        {
            throw new ArgumentException("Missing required field: ceTaskId", nameof(filePath));
        }

        // Create and return the report task
        return new ReportTask(projectKey, serverUrl, ceTaskId);
    }
}
