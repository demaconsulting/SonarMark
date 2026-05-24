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

namespace DemaConsulting.SonarMark.SonarIntegration;

/// <summary>
///     Represents a SonarQube issue
/// </summary>
/// <remarks>
///     Immutable data carrier populated from the SonarQube API by
///     <see cref="SonarQubeClient.ParseIssue"/> and consumed by
///     <see cref="DemaConsulting.SonarMark.ReportGeneration.SonarQualityResult"/> for report
///     rendering via <c>AppendIssuesSection</c>. Immutability makes the record inherently
///     thread-safe; no locking is required when multiple threads read from the same instance.
/// </remarks>
/// <param name="Key">Unique issue identifier as returned by the SonarQube API; non-null.</param>
/// <param name="Rule">Rule key identifying the violated rule (e.g., <c>csharpsquid:S1234</c>); non-null string as returned by the API.</param>
/// <param name="Severity">Severity level as returned by the API (<c>BLOCKER</c>, <c>CRITICAL</c>, <c>MAJOR</c>, <c>MINOR</c>, <c>INFO</c>); non-null.</param>
/// <param name="Component">Component key identifying the source file; non-null string as returned by the API.</param>
/// <param name="Line">Source line number; null when the issue applies to the file as a whole.</param>
/// <param name="Message">Human-readable description of the issue; non-null string as returned by the API.</param>
/// <param name="Type">Issue type as returned by the API (<c>BUG</c>, <c>VULNERABILITY</c>, <c>CODE_SMELL</c>); non-null.</param>
internal sealed record SonarIssue(
    string Key,
    string Rule,
    string Severity,
    string Component,
    int? Line,
    string Message,
    string Type);
