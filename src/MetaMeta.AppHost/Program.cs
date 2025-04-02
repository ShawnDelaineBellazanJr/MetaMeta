using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Configure your services here
        var openAiKey = context.Configuration["Parameters:OpenAI:ApiKey"];

        services.AddSingleton<IConfiguration>(context.Configuration);
        
        // You can add other service configurations here
    });

var host = builder.Build();
await host.RunAsync();
