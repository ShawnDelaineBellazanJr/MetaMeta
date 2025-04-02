namespace MetaMeta.Core.Abstractions;

/// <summary>
/// Configuration for a prompt template.
/// </summary>
public class PromptTemplateConfig
{
    /// <summary>
    /// Gets or sets the name of the prompt template.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the template content.
    /// </summary>
    public string Template { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the description of the prompt template.
    /// </summary>
    public string Description { get; set; } = string.Empty;
} 