using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaMeta.Orchestration
{
    /// <summary>
    /// Represents the directive object created by the Executive Agent that provides
    /// a high-level vision and execution plan based on a user goal.
    /// </summary>
    /// <remarks>
    /// NorthStarDirective serves as the central coordination mechanism for agent orchestration,
    /// translating high-level user goals into structured execution plans.
    /// </remarks>
    public class NorthStarDirective
    {
        // @meta:Agent - The executive agent that generates the vision and plan
        private readonly ChatCompletionAgent _agent;

        /// <summary>
        /// Initializes a new instance of the NorthStarDirective class with a configured executive agent.
        /// </summary>
        public NorthStarDirective()
        {
            _agent = new()
            {
                Name = "NorthStar",
                Instructions = "You are an executive agent. Given a goal and session, return a vision and metadata in JSON format.",
                Description = "Generates a directive from high-level goal input.",
            };
        }

        /// <summary>
        /// The assistant identifier associated with this directive.
        /// </summary>
        public required string Assistant { get; set; }
        
        /// <summary>
        /// The original goal provided by the user.
        /// </summary>
        public required string Goal { get; set; }
        
        /// <summary>
        /// The generated vision statement that provides direction for achieving the goal.
        /// </summary>
        public required string Vision { get; set; } // renamed from NorthStar
        
        /// <summary>
        /// Unique session identifier for tracking the directive execution.
        /// </summary>
        public required string SessionId { get; set; }
        
        /// <summary>
        /// Additional metadata related to the directive execution and context.
        /// </summary>
        public required Dictionary<string, string> Metadata { get; set; }

        /// <summary>
        /// Generates a structured plan based on the high-level goal.
        /// </summary>
        /// <param name="goal">The user-provided goal to generate an objective for.</param>
        /// <returns>A string containing the generated execution plan.</returns>
        public async Task<string?> Objective(string goal)
        {
            // Step 1: Run the agent with the goal as input prompt
            var response = await _agent.Kernel.InvokePromptAsync($"steps to achieve goal: {goal}");
            
            // Step 2: Map the response to the directive's structure
            return response.RenderedPrompt;
        }
    }
}
