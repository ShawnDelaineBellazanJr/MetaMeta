namespace MetaMeta.Orchestration.Models;

/// <summary>
/// Defines the types of operations that the tool agent can perform.
/// </summary>
public enum ToolOperation
{
    /// <summary>
    /// Execute a specific tool.
    /// </summary>
    Execute,
    
    /// <summary>
    /// List available tools.
    /// </summary>
    ListTools,
    
    /// <summary>
    /// Suggest tools for a specific task.
    /// </summary>
    SuggestTools
} 