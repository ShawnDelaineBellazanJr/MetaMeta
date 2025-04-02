using Azure.Core;
using Grpc.Core;
using MetaMeta.GrpcService.Protos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using OpenAI.Chat;
using Azure.AI.OpenAI;


namespace MetaMeta.ApiService.Controllers
{
    /// <summary>
    /// API controller for executive-level AI orchestration capabilities.
    /// </summary>
    /// <remarks>
    /// Provides an HTTP API interface to the Executive Agent gRPC service,
    /// allowing clients to submit high-level goals and receive directives.
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    public class ExecutiveController : ControllerBase
    {
        private readonly ExecutiveAgent.ExecutiveAgentClient _client;
        private readonly ILogger<ExecutiveController> _logger;

        /// <summary>
        /// Initializes a new instance of the ExecutiveController class.
        /// </summary>
        /// <param name="client">The gRPC client for communicating with the Executive Agent service.</param>
        /// <param name="logger">The logger used for diagnostic information.</param>
        public ExecutiveController(
            ExecutiveAgent.ExecutiveAgentClient client, ILogger<ExecutiveController> logger)
        {
            _client = client;
            _logger = logger;
        }

        /// <summary>
        /// Processes a high-level goal and generates a NorthStar directive.
        /// </summary>
        /// <param name="goal">The high-level goal provided by the user.</param>
        /// <returns>An executive response containing the generated plan and results.</returns>
        [HttpGet]
        public async Task<ExecutiveResponse> NorthStar(string goal)
        {
            // Step 1: Create a gRPC request from the user's goal
            var request = new ExecutiveRequest
            {
                Goal = goal,
                Assistant = "WebAPI", // Identify the source of the request
                SessionId = Guid.NewGuid().ToString() // Generate a unique session ID for tracking
            };
            
            try
            {
                // Step 2: Send the request to the gRPC service and await response
                _logger.LogInformation("Sending NorthStar request to gRPC service with goal: {Goal}", goal);
                var response = await _client.NorthStarAsync(request);
                
                // Step 3: Return the response from the executive agent
                _logger.LogInformation("NorthStar response received successfully");
                return response;
            }
            catch (RpcException rpcEx)
            {
                // Step 4a: Handle gRPC-specific errors
                _logger.LogError(rpcEx, "gRPC error during NorthStar execution.");
                throw new ApplicationException("gRPC error during NorthStar execution.", rpcEx);
            }
            catch (Exception ex)
            {
                // Step 4b: Handle general exceptions
                _logger.LogError(ex, "Unexpected error during NorthStar execution.");
                throw new ApplicationException("Unexpected error during NorthStar execution.", ex);
            }
        }
    }
}
