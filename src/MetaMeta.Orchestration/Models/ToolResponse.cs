using System.Collections.Generic;
using MetaMeta.Core.Models;

namespace MetaMeta.Orchestration.Models;

/// <summary>
/// Represents a response from a tool operation.
/// </summary>
public class ToolResponse : AgentResponse
{
    /// <summary>
    /// Gets or sets the plugin name used in the operation.
    /// </summary>
    public string PluginName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the tool name used in the operation.
    /// </summary>
    public string ToolName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the operation that was performed.
    /// </summary>
    public ToolOperation Operation { get; set; }
    
    /// <summary>
    /// Gets or sets the result of a tool execution.
    /// </summary>
    public string Result { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the list of available tools when listing tools.
    /// </summary>
    public List<CustomToolMetadata> AvailableTools { get; set; } = new List<CustomToolMetadata>();
    
    /// <summary>
    /// Gets or sets the list of suggested tools for a task.
    /// </summary>
    public List<ToolSuggestion> SuggestedTools { get; set; } = new List<ToolSuggestion>();
} 