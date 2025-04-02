namespace MetaMeta.Core.Abstractions
{
    /// <summary>
    /// Defines the contract for a service that hosts and manages AI agents.
    /// </summary>
    /// <remarks>
    /// The agent host service is responsible for the lifecycle management of AI agents,
    /// including initialization, startup, and shutdown operations.
    /// </remarks>
    public interface IAgentHostService
    {
        /// <summary>
        /// Initializes the agent host environment and prepares agents for execution.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the initialization process.</param>
        /// <returns>A task representing the asynchronous initialization operation.</returns>
        Task InitializeAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Starts all registered agents and begins processing agent requests.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the startup process.</param>
        /// <returns>A task representing the asynchronous startup operation.</returns>
        Task StartAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Stops all running agents and performs cleanup operations.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the shutdown process.</param>
        /// <returns>A task representing the asynchronous shutdown operation.</returns>
        Task StopAsync(CancellationToken cancellationToken = default);
    }
} 