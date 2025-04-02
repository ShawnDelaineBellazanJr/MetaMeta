namespace MetaMeta.Orchestration.Models;

/// <summary>
/// Represents an item stored in memory.
/// </summary>
public class MemoryItem
{
    /// <summary>
    /// Gets or sets the collection containing the memory item.
    /// </summary>
    public string Collection { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the key of the memory item.
    /// </summary>
    public string Key { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the value of the memory item.
    /// </summary>
    public string Value { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the relevance score (0.0 to 1.0) when this item is a search result.
    /// </summary>
    public double Relevance { get; set; } = 1.0;
} 