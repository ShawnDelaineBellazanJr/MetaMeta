/// <summary>
/// Entry point for the MetaMeta gRPC service application.
/// </summary>
/// <remarks>
/// This class configures the application services, dependency injection,
/// middleware, and maps the gRPC endpoints.
/// </remarks>

using MetaMeta.GrpcService.Services;
using MetaMeta.ServiceDefaults;
using Microsoft.AspNetCore.Server.Kestrel.Core;

// Create the web application builder
var builder = WebApplication.CreateBuilder(args);

// Add service defaults for distributed application configuration
builder.AddServiceDefaults();

// Register AutoMapper for object mapping
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Add gRPC service support
builder.Services.AddGrpc();

// Build the application
var app = builder.Build();

// Map health check and metrics endpoints
app.MapDefaultEndpoints();

// Register the ExecutiveService gRPC service
app.MapGrpcService<ExecutiveService>();

// Add informational root endpoint
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

// Start the application
app.Run();
