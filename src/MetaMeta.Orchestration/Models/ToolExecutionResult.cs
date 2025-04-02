namespace MetaMeta.Orchestration.Models;

/// <summary>
/// Represents the result of a tool execution.
/// </summary>
public class ToolExecutionResult
{
    /// <summary>
    /// Gets or sets the name of the plugin that was executed.
    /// </summary>
    public string PluginName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the name of the tool that was executed.
    /// </summary>
    public string ToolName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets a value indicating whether the execution was successful.
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// Gets or sets the result of the tool execution.
    /// </summary>
    public string Result { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the error message if the execution failed.
    /// </summary>
    public string? ErrorMessage { get; set; }
} 