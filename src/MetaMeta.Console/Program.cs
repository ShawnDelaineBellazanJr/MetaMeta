using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MetaMeta.Orchestration;

namespace MetaMeta.Console;

class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            // Build the host with the required services
            using var host = CreateHostBuilder(args).Build();
            
            // Get the orchestrator from the service provider
            var orchestrator = host.Services.GetRequiredService<Orchestrator>();
            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            
            // Display welcome message
            System.Console.WriteLine("=== MetaMeta Orchestration Demo ===");
            System.Console.WriteLine("Enter a goal for the AI agents to work on:");
            System.Console.Write("> ");
            
            // Get the goal from user input
            string? goal = System.Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(goal))
            {
                System.Console.WriteLine("No goal provided. Exiting...");
                return;
            }
            
            // Process the goal using the orchestrator
            System.Console.WriteLine($"Processing goal: {goal}");
            System.Console.WriteLine("This may take a few moments...");
            
            var result = await orchestrator.ProcessGoalAsync(goal);
            
            // Display the results
            System.Console.WriteLine("\n=== Execution Results ===");
            System.Console.WriteLine($"Success: {result.Success}");
            System.Console.WriteLine($"Steps planned: {result.PlanStepCount}");
            System.Console.WriteLine($"Steps completed: {result.CompletedStepCount}");
            System.Console.WriteLine("\n--- Summary ---");
            System.Console.WriteLine(result.Summary);
            
            if (result.Errors.Count > 0)
            {
                System.Console.WriteLine("\n--- Errors ---");
                foreach (var error in result.Errors)
                {
                    System.Console.WriteLine($"- {error}");
                }
            }
            
            // Ask if user wants to see detailed step results
            System.Console.WriteLine("\nDo you want to see detailed step results? (y/n)");
            System.Console.Write("> ");
            var showDetails = System.Console.ReadLine()?.Trim().ToLower() == "y";
            
            if (showDetails)
            {
                System.Console.WriteLine("\n=== Detailed Step Results ===");
                foreach (var stepResult in result.StepResults)
                {
                    System.Console.WriteLine($"\nStep {stepResult.StepNumber}: {stepResult.Description}");
                    System.Console.WriteLine($"Agent: {stepResult.Agent}");
                    System.Console.WriteLine($"Operation: {stepResult.Operation}");
                    System.Console.WriteLine($"Success: {stepResult.Success}");
                    
                    if (!string.IsNullOrEmpty(stepResult.Output))
                    {
                        System.Console.WriteLine("Output:");
                        System.Console.WriteLine("-------------------");
                        System.Console.WriteLine(stepResult.Output);
                        System.Console.WriteLine("-------------------");
                    }
                    
                    if (!string.IsNullOrEmpty(stepResult.Error))
                    {
                        System.Console.WriteLine($"Error: {stepResult.Error}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"An error occurred: {ex.Message}");
            System.Console.WriteLine(ex.StackTrace);
        }
    }
    
    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: true);
                config.AddEnvironmentVariables();
                config.AddCommandLine(args);
            })
            .ConfigureServices((hostContext, services) =>
            {
                try
                {
                    // Add orchestration services
                    services.AddOrchestrationServices(hostContext.Configuration);
                    
                    // Add logging
                    services.AddLogging(configure =>
                    {
                        configure.AddConsole();
                        configure.AddDebug();
                    });
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine($"Error configuring services: {ex.Message}");
                    System.Console.WriteLine(ex.StackTrace);
                }
            });
} 