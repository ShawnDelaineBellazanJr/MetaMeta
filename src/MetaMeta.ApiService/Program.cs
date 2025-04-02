using MetaMeta.GrpcService.Protos;
using MetaMeta.ServiceDefaults;
using MetaMeta.ServiceDefaults.Configurations;
using Microsoft.OpenApi.Models;
using MetaMeta.Orchestration;
using MetaMeta.Core.Abstractions;
using MetaMeta.Core.PromptTemplates;
using Microsoft.Extensions.Configuration;
using MetaMeta.ApiService.Extensions;

var builder = WebApplication.CreateBuilder(args);

// ✅ Add Aspire service defaults (logging, telemetry, health, etc.)
builder.AddServiceDefaults();

// ✅ gRPC client registration using Aspire environment routing
builder.Services.AddGrpcClient<ExecutiveAgent.ExecutiveAgentClient>(opt =>
{
    opt.Address = new Uri("https://grpcservice");
});
// ✅ Add controllers and problem details middleware
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();

// ✅ Add Swagger (REST UI docs)
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations(); // For [SwaggerOperation]
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MetaMeta Executive API",
        Version = "v1",
        Description = "Autonomous Agent API for executing NorthStar-aligned goals."
    });
});

// 🆕 Add Semantic Kernel and Agents
builder.Services.AddSingleton<IPromptTemplateFactory, HandlebarsPromptTemplateFactory>();

// 🆕 Add Orchestration services with configuration
builder.Services.AddOrchestrationServices(builder.Configuration);

// ✅ Build the app
var app = builder.Build();

// ✅ Use centralized error handling
app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseRouting();

// ✅ Map REST controllers
app.MapControllers();

// ✅ Swagger only in dev
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerDocumentation();
}

// ✅ Add Aspire telemetry/health/etc.
app.MapDefaultEndpoints(); // includes health/liveness/startup

// ✅ Run the app
app.Run();
