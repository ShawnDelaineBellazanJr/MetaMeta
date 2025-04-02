namespace MetaMeta.Orchestration.Models;

/// <summary>
/// Defines the types of content that can be generated.
/// </summary>
public enum ContentType
{
    /// <summary>
    /// Plain text content.
    /// </summary>
    Text,
    
    /// <summary>
    /// Source code content.
    /// </summary>
    Code,
    
    /// <summary>
    /// Structured document content.
    /// </summary>
    Document,
    
    /// <summary>
    /// Email content.
    /// </summary>
    Email,
    
    /// <summary>
    /// Mixed content types.
    /// </summary>
    Mixed
} 