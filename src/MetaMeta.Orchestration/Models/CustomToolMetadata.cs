using System.Collections.Generic;

namespace MetaMeta.Orchestration.Models;

/// <summary>
/// Represents metadata about a tool (function) in the Semantic Kernel.
/// </summary>
public class CustomToolMetadata
{
    /// <summary>
    /// Gets or sets the name of the plugin that contains the tool.
    /// </summary>
    public string PluginName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the name of the tool.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the description of the tool.
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the parameters of the tool.
    /// </summary>
    public List<ToolParameterInfo> Parameters { get; set; } = new List<ToolParameterInfo>();
}

/// <summary>
/// Represents information about a tool parameter.
/// </summary>
public class ToolParameterInfo
{
    /// <summary>
    /// Gets or sets the name of the parameter.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the description of the parameter.
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the default value of the parameter.
    /// </summary>
    public string DefaultValue { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets a value indicating whether the parameter is required.
    /// </summary>
    public bool IsRequired { get; set; }
} 