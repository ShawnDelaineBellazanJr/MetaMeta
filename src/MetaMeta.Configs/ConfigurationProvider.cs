using Microsoft.Extensions.Configuration;

namespace MetaMeta.Configs
{
    /// <summary>
    /// Provides strongly-typed access to application configuration settings.
    /// </summary>
    /// <remarks>
    /// This class simplifies access to configuration values by providing methods
    /// for retrieving typed configuration sections and individual values with fallbacks.
    /// </remarks>
    public class ConfigurationProvider
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the ConfigurationProvider class.
        /// </summary>
        /// <param name="configuration">The configuration source to wrap.</param>
        public ConfigurationProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Binds a configuration section to a new instance of type T.
        /// </summary>
        /// <typeparam name="T">The type to bind configuration values to.</typeparam>
        /// <param name="sectionName">The configuration section name to bind.</param>
        /// <returns>A new instance of T with properties populated from configuration.</returns>
        public T GetSection<T>(string sectionName) where T : class, new()
        {
            // Create a new instance and bind the configuration section to it
            var section = new T();
            _configuration.GetSection(sectionName).Bind(section);
            return section;
        }

        /// <summary>
        /// Gets a configuration value by key or returns empty string if not found.
        /// </summary>
        /// <param name="key">The configuration key.</param>
        /// <returns>The configuration value or empty string.</returns>
        public string GetValue(string key)
        {
            return _configuration[key] ?? string.Empty;
        }

        /// <summary>
        /// Gets a configuration value by key or returns the specified default value if not found.
        /// </summary>
        /// <param name="key">The configuration key.</param>
        /// <param name="defaultValue">The default value to return if key is not found.</param>
        /// <returns>The configuration value or the default value.</returns>
        public string GetValue(string key, string defaultValue)
        {
            return _configuration[key] ?? defaultValue;
        }
    }
} 