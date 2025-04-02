using Microsoft.SemanticKernel;
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
        // The kernel instance for executing prompts
        private readonly Kernel _kernel;

        /// <summary>
        /// Initializes a new instance of the NorthStarDirective class with a configured kernel.
        /// </summary>
        /// <param name="kernel">The Semantic Kernel instance to use for executing prompts.</param>
        public NorthStarDirective(Kernel kernel)
        {
            _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
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
            // Create the prompt for generating a plan
            var prompt = new StringBuilder();
            prompt.AppendLine("You are an executive agent responsible for planning and coordinating complex tasks.");
            prompt.AppendLine("Given a goal, generate a clear, structured plan with specific steps to achieve it.");
            prompt.AppendLine();
            prompt.AppendLine($"GOAL: {goal}");
            prompt.AppendLine();
            prompt.AppendLine("EXECUTION PLAN:");
            
            // Execute the prompt
            var result = await _kernel.InvokePromptAsync(prompt.ToString());
            
            // Return the generated plan
            return result.GetValue<string>();
        }
    }
}
