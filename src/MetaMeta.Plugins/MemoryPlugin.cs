using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;

namespace MetaMeta.Plugins;

/// <summary>
/// Plugin for memory and context management.
/// </summary>
public class MemoryPlugin
{
    // Simple in-memory store for demonstration purposes
    private static readonly Dictionary<string, Dictionary<string, string>> _memoryStore = new();

    /// <summary>
    /// Stores a memory item in the agent's context.
    /// </summary>
    /// <param name="key">The memory key.</param>
    /// <param name="value">The memory value.</param>
    /// <param name="sessionId">The session identifier for contextual tracking.</param>
    /// <returns>Confirmation of storage.</returns>
    [KernelFunction, Description("Stores a memory item with the given key and value in the agent's context.")]
    public string StoreMemory(
        [Description("The key for the memory item")] string key,
        [Description("The value to store")] string value,
        [Description("The session identifier")] string sessionId = "default")
    {
        if (!_memoryStore.ContainsKey(sessionId))
        {
            _memoryStore[sessionId] = new Dictionary<string, string>();
        }

        _memoryStore[sessionId][key] = value;
        return $"Memory stored: {key}";
    }

    /// <summary>
    /// Retrieves a memory item from the agent's context.
    /// </summary>
    /// <param name="key">The memory key to retrieve.</param>
    /// <param name="sessionId">The session identifier for contextual tracking.</param>
    /// <returns>The memory value if found, otherwise a not found message.</returns>
    [KernelFunction, Description("Retrieves a memory item with the given key from the agent's context.")]
    public string RetrieveMemory(
        [Description("The key for the memory item to retrieve")] string key,
        [Description("The session identifier")] string sessionId = "default")
    {
        if (!_memoryStore.ContainsKey(sessionId) || !_memoryStore[sessionId].ContainsKey(key))
        {
            return $"Memory not found: {key}";
        }

        return _memoryStore[sessionId][key];
    }

    /// <summary>
    /// Lists all memories in the current session.
    /// </summary>
    /// <param name="sessionId">The session identifier for contextual tracking.</param>
    /// <returns>A JSON representation of all memories in the session.</returns>
    [KernelFunction, Description("Lists all memories stored in the current session.")]
    public string ListMemories(
        [Description("The session identifier")] string sessionId = "default")
    {
        if (!_memoryStore.ContainsKey(sessionId) || _memoryStore[sessionId].Count == 0)
        {
            return "No memories found in this session.";
        }

        var memories = new Dictionary<string, string>(_memoryStore[sessionId]);
        return System.Text.Json.JsonSerializer.Serialize(memories, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true
        });
    }

    /// <summary>
    /// Searches for memories containing the specified query.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <param name="sessionId">The session identifier for contextual tracking.</param>
    /// <returns>A JSON representation of matching memories.</returns>
    [KernelFunction, Description("Searches for memories containing the specified query.")]
    public string SearchMemories(
        [Description("The search query")] string query,
        [Description("The session identifier")] string sessionId = "default")
    {
        if (!_memoryStore.ContainsKey(sessionId) || _memoryStore[sessionId].Count == 0)
        {
            return "No memories found in this session.";
        }

        var matches = new Dictionary<string, string>();
        foreach (var (key, value) in _memoryStore[sessionId])
        {
            if (key.Contains(query, StringComparison.OrdinalIgnoreCase) || 
                value.Contains(query, StringComparison.OrdinalIgnoreCase))
            {
                matches[key] = value;
            }
        }

        if (matches.Count == 0)
        {
            return $"No memories matching '{query}' found.";
        }

        return System.Text.Json.JsonSerializer.Serialize(matches, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true
        });
    }
} 