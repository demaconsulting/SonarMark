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
///     Represents a SonarQube security hot-spot
/// </summary>
/// <remarks>
///     Immutable data carrier populated from the SonarQube API by <see cref="SonarQubeClient"/>
///     and consumed by <see cref="DemaConsulting.SonarMark.ReportGeneration.SonarQualityResult"/>
///     for report rendering. Immutability makes the record inherently thread-safe; no locking is
///     required when multiple threads read from the same instance.
/// </remarks>
/// <param name="Key">Unique hot-spot identifier as returned by the SonarQube API; non-null.</param>
/// <param name="Component">Component key identifying the source file; non-null string as returned by the API.</param>
/// <param name="Line">Source line number; null when the hot-spot applies to the file as a whole rather than a specific line.</param>
/// <param name="Message">Human-readable description of the hot-spot; non-null string as returned by the API.</param>
/// <param name="SecurityCategory">Security category key (e.g., <c>sql-injection</c>, <c>weak-cryptography</c>); non-null string as returned by the API.</param>
/// <param name="VulnerabilityProbability">Likelihood that the hot-spot is an actual vulnerability (e.g., <c>HIGH</c>, <c>MEDIUM</c>, <c>LOW</c>); non-null string as returned by the API.</param>
internal sealed record SonarHotSpot(
    string Key,
    string Component,
    int? Line,
    string Message,
    string SecurityCategory,
    string VulnerabilityProbability);
