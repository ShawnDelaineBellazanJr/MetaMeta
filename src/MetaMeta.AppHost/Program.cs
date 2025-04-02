var builder = DistributedApplication.CreateBuilder(args);

var key = builder.Configuration["Parameters:OpenAI:ApiKey"];

var cache = builder.AddRedis("cache");


var grpc = builder.AddProject<Projects.MetaMeta_GrpcService>("grpcservice")
    .WithEnvironment("OPENAI_APIKEY", key );


var apiService = builder.AddProject<Projects.MetaMeta_ApiService>("apiservice")
.WithEnvironment("OPENAI_APIKEY", key)
.WithReference(grpc)
    .WaitFor(grpc);

builder.AddProject<Projects.MetaMeta_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
