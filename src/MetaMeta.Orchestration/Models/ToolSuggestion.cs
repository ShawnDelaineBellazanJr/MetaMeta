using System.Collections.Generic;

namespace MetaMeta.Orchestration.Models;

/// <summary>
/// Represents a suggestion for a tool that could be used for a specific task.
/// </summary>
public class ToolSuggestion
{
    /// <summary>
    /// Gets or sets the name of the plugin containing the tool.
    /// </summary>
    public string PluginName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the name of the suggested tool.
    /// </summary>
    public string ToolName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the description of the tool.
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the parameters for the tool.
    /// </summary>
    public List<ToolParameterInfo> Parameters { get; set; } = new List<ToolParameterInfo>();
    
    /// <summary>
    /// Gets or sets the relevance score for this suggestion (0.0 to 1.0).
    /// </summary>
    public double Relevance { get; set; }
} 