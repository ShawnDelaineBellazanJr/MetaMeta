using System.Collections.Generic;
using MetaMeta.Core.Models;

namespace MetaMeta.Orchestration.Models;

/// <summary>
/// Represents a request to execute a tool operation.
/// </summary>
public class ToolRequest : AgentRequest
{
    /// <summary>
    /// Gets or sets the plugin name containing the tool to execute.
    /// </summary>
    public string PluginName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the name of the tool to execute.
    /// </summary>
    public string ToolName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the parameters for the tool execution.
    /// </summary>
    public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
    
    /// <summary>
    /// Gets or sets the description of what needs to be accomplished when suggesting tools.
    /// </summary>
    public string TaskDescription { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the maximum number of tools to suggest.
    /// </summary>
    public int SuggestionLimit { get; set; } = 3;
    
    /// <summary>
    /// Gets or sets the operation type for the tool agent.
    /// </summary>
    public ToolOperation Operation { get; set; } = ToolOperation.Execute;
} 