using System;

namespace MetaMeta.Orchestration.Logging;

/// <summary>
/// A simple logging interface for the Orchestration layer.
/// </summary>
public interface ISimpleLogger
{
    /// <summary>
    /// Logs an information message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void LogInformation(string message);
    
    /// <summary>
    /// Logs a warning message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void LogWarning(string message);
    
    /// <summary>
    /// Logs an error message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void LogError(string message);
    
    /// <summary>
    /// Logs an error message with an exception.
    /// </summary>
    /// <param name="ex">The exception to log.</param>
    /// <param name="message">The message to log.</param>
    void LogError(Exception ex, string message);
} 