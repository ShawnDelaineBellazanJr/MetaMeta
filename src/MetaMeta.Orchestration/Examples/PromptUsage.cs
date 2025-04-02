using System;
using System.Threading.Tasks;
using MetaMeta.Core.Abstractions;
using MetaMeta.Orchestration.Prompts;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using IPromptFactory = MetaMeta.Core.Abstractions.IPromptTemplateFactory;

namespace MetaMeta.Orchestration.Examples
{
    /// <summary>
    /// Demonstrates how to use the prompt loader with the new library structure
    /// </summary>
    public class PromptUsageExamples
    {
        private readonly Kernel _kernel;
        private readonly PromptLoader _promptLoader;
        private readonly ISimpleLogger _logger;

        public PromptUsageExamples(Kernel kernel, IPromptFactory promptFactory)
        {
            _kernel = kernel;
            _logger = new ConsoleLogger();
            _promptLoader = new PromptLoader(promptFactory, _logger);
        }

        /// <summary>
        /// Example of loading and using a prompt from the root Prompts directory
        /// </summary>
        public async Task UseRootPromptAsync()
        {
            // Load a prompt from the root directory
            var promptTemplate = await _promptLoader.LoadPromptAsync("AgentSelectionAgent");

            // Create a function from the prompt template string
            var function = _kernel.CreateFunctionFromPrompt(
                promptTemplate.Template,
                new PromptExecutionSettings());

            // Execute the function
            var result = await _kernel.InvokeAsync(function, new KernelArguments
            {
                ["step_description"] = "Analyze customer feedback for sentiment",
                ["step_number"] = "1",
                ["available_agents"] = "SentimentAnalysisAgent, TextClassificationAgent, SummaryAgent",
                ["context"] = "Processing customer feedback from the last quarter",
                ["goal"] = "Generate a comprehensive feedback analysis report"
            });

            _logger.LogDebug("AgentSelection result: {Result}", result.GetValue<string>());
        }

        /// <summary>
        /// Example of loading and using a prompt from the Library structure
        /// </summary>
        public async Task UseLibraryPromptAsync()
        {
            // Load a prompt from the Library structure with category
            var blogPostPrompt = await _promptLoader.LoadPromptAsync(
                "BlogPostGenerator",
                "BlogPostGenerator",
                "ContentCreation");

            // Create a function from the prompt template string
            var function = _kernel.CreateFunctionFromPrompt(
                blogPostPrompt.Template,
                new PromptExecutionSettings());

            var result = await _kernel.InvokeAsync(function, new KernelArguments
            {
                ["title"] = "The Future of AI in Healthcare",
                ["topic"] = "How AI is transforming medical diagnostics",
                ["keywords"] = "AI, healthcare, machine learning, diagnostics, medical innovation",
                ["target_audience"] = "Healthcare professionals and technology enthusiasts",
                ["tone"] = "Informative and forward-thinking",
                ["word_count"] = "1000"
            });

            _logger.LogDebug("Blog post generated successfully");
        }

        /// <summary>
        /// Example of using the new agent design prompts
        /// </summary>
        public async Task UseAgentDesignPromptAsync()
        {
            // Load an agent design prompt
            var agentPromptDesignerPrompt = await _promptLoader.LoadPromptAsync(
                "AgentPromptDesigner",
                "AgentPromptDesigner",
                "AgentDesign");

            // Create a function from the prompt template string
            var function = _kernel.CreateFunctionFromPrompt(
                agentPromptDesignerPrompt.Template,
                new PromptExecutionSettings());

            var result = await _kernel.InvokeAsync(function, new KernelArguments
            {
                ["agent_name"] = "DataValidationAgent",
                ["agent_purpose"] = "Validate data formats and integrity across multiple schemas",
                ["agent_capabilities"] = "Schema validation, data type checking, consistency verification",
                ["input_parameters"] = "data_source, schema_definition, validation_level",
                ["output_format"] = "JSON report with validation results and error details"
            });

            _logger.LogDebug("Agent prompt design completed successfully");
        }
    }
}