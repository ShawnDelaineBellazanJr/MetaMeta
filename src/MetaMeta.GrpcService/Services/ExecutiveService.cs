using AutoMapper;
using Azure.Identity;
using Grpc.Core;
using MetaMeta.GrpcService;
using MetaMeta.GrpcService.Protos;
using Microsoft.JSInterop;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;

namespace MetaMeta.GrpcService.Services;

/// <summary>
/// gRPC service that provides executive-level AI orchestration capabilities.
/// </summary>
/// <remarks>
/// This service processes high-level goals from clients and translates them into 
/// structured plans and directives using executive agent capabilities.
/// </remarks>
public class ExecutiveService: ExecutiveAgent.ExecutiveAgentBase
{
    private readonly Kernel _kernel;
    private readonly ILogger<ExecutiveService> _logger;
    private readonly IMapper _mapper;
    
    // Configuration used to set up the service
    private readonly IConfiguration _config;
    
    // @meta:Agent - The executive agent that processes high-level goals
    private readonly ChatCompletionAgent _executive;

    /// <summary>
    /// Initializes a new instance of the ExecutiveService class.
    /// </summary>
    /// <param name="kernel">The kernel builder used to create the semantic kernel instance.</param>
    /// <param name="logger">The logger used for diagnostic information.</param>
    /// <param name="mapper">The AutoMapper instance for mapping between gRPC and domain objects.</param>
    /// <param name="config">The configuration containing service settings.</param>
    public ExecutiveService(IKernelBuilder kernel, ILogger<ExecutiveService> logger, IMapper mapper, IConfiguration config)
    {
        _kernel = kernel.Build();
        _logger = logger;
        _mapper = mapper;
        _config = config;

        // Initialize the executive agent with appropriate instructions
        _executive = new()
        {
            Name = "Executive",
            Instructions = "You are an executive agent. Given a goal and session, return a vision and metadata in JSON format.",
            Description = "Generates a directive from high-level goal input.",
        };
    }

    /// <summary>
    /// Processes a high-level goal request and generates a NorthStar directive.
    /// </summary>
    /// <param name="request">The executive request containing the goal and context.</param>
    /// <param name="context">The server call context.</param>
    /// <returns>An executive response containing the generated plan and results.</returns>
    public override async Task<ExecutiveResponse> NorthStar(ExecutiveRequest request, ServerCallContext context)
    {
        // Step 1: Log the incoming request for traceability
        _logger.LogInformation("Received NorthStar goal from {Assistant} - Session {SessionId}", request.Assistant, request.SessionId);
        
        // Step 2: Process the goal using the kernel
        var results = await _kernel.InvokePromptAsync(request.Goal);

        // Step 3: Build and return gRPC response
        return new ExecutiveResponse
        {
            Results = { results.GetValue<string>() },
            Plan = "{}", // TODO: inject the real plan
            TraceId = Guid.NewGuid().ToString(),
            Timestamp = DateTime.UtcNow.ToString("o")
        };
    }
}
