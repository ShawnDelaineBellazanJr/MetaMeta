using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MetaMeta.Plugins;

/// <summary>
/// Plugin system for extending MetaMeta with custom functionality.
/// </summary>
/// <remarks>
/// The plugin system provides a standardized way to extend the MetaMeta platform
/// with custom capabilities without modifying the core application.
/// </remarks>
public static class PluginSystem
{
    private static readonly Dictionary<string, IPlugin> _registeredPlugins = new();

    /// <summary>
    /// Registers a plugin with the MetaMeta plugin system.
    /// </summary>
    /// <param name="plugin">The plugin to register.</param>
    /// <returns>True if registration was successful, false if a plugin with the same ID is already registered.</returns>
    public static bool RegisterPlugin(IPlugin plugin)
    {
        // Step 1: Validate the plugin
        if (plugin == null)
        {
            throw new ArgumentNullException(nameof(plugin), "Plugin cannot be null.");
        }

        // Step 2: Check if a plugin with this ID is already registered
        if (_registeredPlugins.ContainsKey(plugin.Id))
        {
            return false;
        }

        // Step 3: Add the plugin to the registration dictionary
        _registeredPlugins[plugin.Id] = plugin;
        return true;
    }

    /// <summary>
    /// Executes a plugin by its ID with the provided context.
    /// </summary>
    /// <param name="pluginId">The ID of the plugin to execute.</param>
    /// <param name="context">The context object containing data for the plugin execution.</param>
    /// <returns>The result of the plugin execution.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when no plugin with the specified ID is registered.</exception>
    public static async Task<PluginResult> ExecutePluginAsync(string pluginId, PluginContext context)
    {
        // Step 1: Check if the plugin exists
        if (!_registeredPlugins.TryGetValue(pluginId, out var plugin))
        {
            throw new KeyNotFoundException($"No plugin with ID '{pluginId}' is registered.");
        }

        // Step 2: Execute the plugin with the provided context
        return await plugin.ExecuteAsync(context);
    }

    /// <summary>
    /// Gets all registered plugins.
    /// </summary>
    /// <returns>A collection of all registered plugins.</returns>
    public static IEnumerable<IPlugin> GetAllPlugins()
    {
        return _registeredPlugins.Values;
    }
}

/// <summary>
/// Interface for MetaMeta plugins.
/// </summary>
/// <remarks>
/// Implement this interface to create custom plugins that can be registered with the MetaMeta plugin system.
/// </remarks>
public interface IPlugin
{
    /// <summary>
    /// Gets the unique identifier for the plugin.
    /// </summary>
    string Id { get; }
    
    /// <summary>
    /// Gets the display name of the plugin.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Gets the description of the plugin's functionality.
    /// </summary>
    string Description { get; }
    
    /// <summary>
    /// Gets the version of the plugin.
    /// </summary>
    string Version { get; }
    
    /// <summary>
    /// Executes the plugin with the provided context.
    /// </summary>
    /// <param name="context">The context containing data for the plugin execution.</param>
    /// <returns>The result of the plugin execution.</returns>
    Task<PluginResult> ExecuteAsync(PluginContext context);
}

/// <summary>
/// Context object containing data for plugin execution.
/// </summary>
public class PluginContext
{
    /// <summary>
    /// Gets or sets the input data for the plugin.
    /// </summary>
    public required object Input { get; set; }
    
    /// <summary>
    /// Gets or sets custom parameters for the plugin execution.
    /// </summary>
    public Dictionary<string, object> Parameters { get; set; } = new();
    
    /// <summary>
    /// Gets or sets the cancellation token for the plugin execution.
    /// </summary>
    public CancellationToken CancellationToken { get; set; } = CancellationToken.None;
}

/// <summary>
/// Result object returned from plugin execution.
/// </summary>
public class PluginResult
{
    /// <summary>
    /// Gets or sets whether the plugin execution was successful.
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// Gets or sets the output data from the plugin.
    /// </summary>
    public object? Output { get; set; }
    
    /// <summary>
    /// Gets or sets an error message if the execution failed.
    /// </summary>
    public string? ErrorMessage { get; set; }
}
