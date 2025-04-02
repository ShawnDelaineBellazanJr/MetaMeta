using System;

namespace MetaMeta.Orchestration.Logging;

/// <summary>
/// A simple console-based logger implementation.
/// </summary>
public class ConsoleLogger : ISimpleLogger
{
    /// <summary>
    /// Logs an information message to the console.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void LogInformation(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[INFO] {message}");
        Console.ResetColor();
    }

    /// <summary>
    /// Logs a warning message to the console.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void LogWarning(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[WARNING] {message}");
        Console.ResetColor();
    }

    /// <summary>
    /// Logs an error message to the console.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void LogError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[ERROR] {message}");
        Console.ResetColor();
    }

    /// <summary>
    /// Logs an error message with an exception to the console.
    /// </summary>
    /// <param name="ex">The exception to log.</param>
    /// <param name="message">The message to log.</param>
    public void LogError(Exception ex, string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[ERROR] {message}");
        Console.WriteLine($"Exception: {ex.Message}");
        Console.WriteLine($"Stack Trace: {ex.StackTrace}");
        Console.ResetColor();
    }
}