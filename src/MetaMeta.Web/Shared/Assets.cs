using System.Collections.Generic;

namespace MetaMeta.Web.Shared;

/// <summary>
/// Provides access to web assets
/// </summary>
public static class Assets
{
    private static readonly Dictionary<string, string> _paths = new()
    {
        ["lib/bootstrap/dist/css/bootstrap.min.css"] = "lib/bootstrap/dist/css/bootstrap.min.css",
        ["app.css"] = "css/app.css",
        ["MetaMeta.Web.styles.css"] = "MetaMeta.Web.styles.css",
        ["logo"] = "images/logo.png",
        ["favicon"] = "favicon.png",
        ["js/app.js"] = "js/app.js"
    };

    /// <summary>
    /// Gets the path for a specified asset key
    /// </summary>
    /// <param name="key">The asset key</param>
    /// <returns>The asset path</returns>
    public static string Get(string key) => _paths.TryGetValue(key, out var path) ? path : key;
}