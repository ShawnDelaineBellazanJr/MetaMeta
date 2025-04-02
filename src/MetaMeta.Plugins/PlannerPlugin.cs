using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;

namespace MetaMeta.Plugins;

/// <summary>
/// Plugin for planning and agent delegation.
/// </summary>
public class PlannerPlugin
{
    /// <summary>
    /// Creates a plan for a given goal, breaking it down into steps.
    /// </summary>
    /// <param name="goal">The goal to plan for.</param>
    /// <param name="maxSteps">The maximum number of steps in the plan.</param>
    /// <returns>A JSON-formatted plan with steps and assigned agents.</returns>
    [KernelFunction, Description("Creates a step-by-step plan to achieve a goal, with assigned agents for each step.")]
    public async Task<string> CreatePlanAsync(
        [Description("The goal to create a plan for")] string goal,
        [Description("Maximum number of steps in the plan")] int maxSteps = 5)
    {
        // In a real implementation, this would use a planning system or LLM
        var plan = new
        {
            goal = goal,
            steps = new[]
            {
                new { id = 1, description = "Understand the requirements", agent = "ReasoningAgent" },
                new { id = 2, description = "Research relevant information", agent = "MemoryAgent" },
                new { id = 3, description = "Generate initial solution", agent = "ContentAgent" },
                new { id = 4, description = "Validate solution approach", agent = "ReasoningAgent" },
                new { id = 5, description = "Finalize and deliver solution", agent = "ToolAgent" }
            },
            maxSteps = maxSteps
        };

        // Convert to JSON string
        return System.Text.Json.JsonSerializer.Serialize(plan, new System.Text.Json.JsonSerializerOptions 
        { 
            WriteIndented = true 
        });
    }

    /// <summary>
    /// Delegates a task to the most appropriate agent.
    /// </summary>
    /// <param name="task">The task to delegate.</param>
    /// <param name="context">Additional context for the task.</param>
    /// <returns>The agent name and confidence score.</returns>
    [KernelFunction, Description("Delegates a task to the most appropriate agent based on capabilities and context.")]
    public string DelegateTask(
        [Description("The task to delegate")] string task,
        [Description("Additional context for the task")] string context = "")
    {
        // In a real implementation, this would analyze the task and determine the best agent
        var agents = new[]
        {
            ("ReasoningAgent", "Analytical thinking, problem solving, step-by-step reasoning"),
            ("ContentAgent", "Content creation, writing, formatting text"),
            ("MemoryAgent", "Information retrieval, remembering context, historical data"),
            ("ToolAgent", "Executing external tools, API calls, file operations")
        };

        string bestAgent = "ReasoningAgent";
        double bestScore = 0.0;

        // Simple keyword matching for demonstration
        foreach (var (agent, skills) in agents)
        {
            double score = 0;
            foreach (var keyword in task.Split(' '))
            {
                if (skills.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                {
                    score += 0.1;
                }
            }
            
            if (score > bestScore)
            {
                bestScore = score;
                bestAgent = agent;
            }
        }

        return $"{{\"agent\": \"{bestAgent}\", \"confidence\": {Math.Max(0.5, bestScore)}}}";
    }
} 