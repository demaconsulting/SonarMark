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

using DemaConsulting.SonarMark.SonarIntegration;
using Xunit;

namespace DemaConsulting.SonarMark.Tests.SonarIntegration;

/// <summary>
///     Tests for SonarHotSpot class
/// </summary>
public class SonarHotSpotTests
{
    /// <summary>
    ///     Test that SonarHotSpot can be created with all properties
    /// </summary>
    [Fact]
    public void SonarHotSpot_Constructor_AllProperties_CreatesInstance()
    {
        // Arrange & Act
        var hotSpot = new SonarHotSpot(
            "hs-key-123",
            "test_project:src/File.cs",
            42,
            "Security vulnerability detected",
            "sql-injection",
            "HIGH");

        // Assert
        Assert.Equal("hs-key-123", hotSpot.Key);
        Assert.Equal("test_project:src/File.cs", hotSpot.Component);
        Assert.Equal(42, hotSpot.Line);
        Assert.Equal("Security vulnerability detected", hotSpot.Message);
        Assert.Equal("sql-injection", hotSpot.SecurityCategory);
        Assert.Equal("HIGH", hotSpot.VulnerabilityProbability);
    }

    /// <summary>
    ///     Test that SonarHotSpot can be created with null line number
    /// </summary>
    [Fact]
    public void SonarHotSpot_Constructor_NullLine_CreatesInstance()
    {
        // Arrange & Act
        var hotSpot = new SonarHotSpot(
            "hs-key-456",
            "test_project:src/Global.cs",
            null,
            "Global security issue",
            "weak-cryptography",
            "MEDIUM");

        // Assert
        Assert.Equal("hs-key-456", hotSpot.Key);
        Assert.Equal("test_project:src/Global.cs", hotSpot.Component);
        Assert.Null(hotSpot.Line);
        Assert.Equal("Global security issue", hotSpot.Message);
        Assert.Equal("weak-cryptography", hotSpot.SecurityCategory);
        Assert.Equal("MEDIUM", hotSpot.VulnerabilityProbability);
    }

    /// <summary>
    ///     Test that SonarHotSpot supports LOW vulnerability probability
    /// </summary>
    [Fact]
    public void SonarHotSpot_Constructor_LowProbability_CreatesInstance()
    {
        // Arrange & Act
        var hotSpot = new SonarHotSpot(
            "hs-key-789",
            "test_project:src/Helper.cs",
            10,
            "Potential security issue",
            "xss",
            "LOW");

        // Assert
        Assert.Equal("hs-key-789", hotSpot.Key);
        Assert.Equal("test_project:src/Helper.cs", hotSpot.Component);
        Assert.Equal(10, hotSpot.Line);
        Assert.Equal("Potential security issue", hotSpot.Message);
        Assert.Equal("xss", hotSpot.SecurityCategory);
        Assert.Equal("LOW", hotSpot.VulnerabilityProbability);
    }
}

