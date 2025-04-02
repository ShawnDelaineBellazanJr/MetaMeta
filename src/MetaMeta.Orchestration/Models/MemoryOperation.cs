namespace MetaMeta.Orchestration.Models;

/// <summary>
/// Defines the memory operation types.
/// </summary>
public enum MemoryOperation
{
    /// <summary>
    /// Store data in memory.
    /// </summary>
    Store,
    
    /// <summary>
    /// Retrieve data from memory.
    /// </summary>
    Retrieve,
    
    /// <summary>
    /// Delete data from memory.
    /// </summary>
    Delete,
    
    /// <summary>
    /// Search for data in memory.
    /// </summary>
    Search
} 