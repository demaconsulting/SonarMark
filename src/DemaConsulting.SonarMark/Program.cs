namespace DemaConsulting.SonarMark;

/// <summary>
/// Main program entry point for SonarMark application
/// </summary>
internal static class Program
{
    /// <summary>
    /// Application version constant
    /// </summary>
    public const string Version = "0.0.0";

    /// <summary>
    /// Main entry point
    /// </summary>
    /// <param name="args">Command line arguments</param>
    /// <returns>Exit code</returns>
    public static int Main(string[] args)
    {
        Console.WriteLine($"SonarMark v{Version}");
        Console.WriteLine("Tool to create a code quality report from a SonarQube/SonarCloud analysis");
        return 0;
    }
}
