using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HandlebarsDotNet;
using Microsoft.SemanticKernel;
using MetaMeta.Core.Abstractions;

namespace MetaMeta.Core.PromptTemplates;

/// <summary>
/// Factory for creating Handlebars-based prompt templates.
/// </summary>
public class HandlebarsPromptTemplateFactory : MetaMeta.Core.Abstractions.IPromptTemplateFactory
{
    /// <summary>
    /// Creates a Handlebars prompt template from the provided configuration.
    /// </summary>
    /// <param name="config">The prompt template configuration.</param>
    /// <returns>A new Handlebars prompt template.</returns>
    public MetaMeta.Core.Abstractions.IPromptTemplate Create(MetaMeta.Core.Abstractions.PromptTemplateConfig config)
    {
        // Create a custom prompt template that uses Handlebars
        return new CustomHandlebarsPromptTemplate(config.Template, config.Name, config.Description);
    }
}

/// <summary>
/// A custom implementation of IPromptTemplate that uses Handlebars for templating.
/// </summary>
internal class CustomHandlebarsPromptTemplate : MetaMeta.Core.Abstractions.IPromptTemplate
{
    private readonly string _template;
    private readonly HandlebarsTemplate<object, object> _handlebarsTemplate;

    /// <summary>
    /// Initializes a new instance of the CustomHandlebarsPromptTemplate class.
    /// </summary>
    /// <param name="template">The template string with Handlebars syntax.</param>
    /// <param name="name">The name of the template.</param>
    /// <param name="description">The description of the template.</param>
    public CustomHandlebarsPromptTemplate(string template, string name, string description)
    {
        _template = template;
        Name = name;
        Description = description;
        
        // Compile the Handlebars template
        _handlebarsTemplate = Handlebars.Compile(template);
    }

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public string Description { get; }

    /// <inheritdoc/>
    public string Template => _template;

    /// <inheritdoc/>
    public Task<string> RenderAsync(KernelArguments arguments, CancellationToken cancellationToken = default)
    {
        // Convert KernelArguments to a dictionary that Handlebars can use
        var data = new Dictionary<string, object>();
        
        if (arguments != null)
        {
            foreach (var kvp in arguments)
            {
                data[kvp.Key] = kvp.Value;
            }
        }
        
        // Apply the template
        string result = _handlebarsTemplate(data);
        return Task.FromResult(result);
    }

    /// <inheritdoc/>
    public Task<string> RenderAsync(Kernel kernel, KernelArguments? arguments, CancellationToken cancellationToken = default)
    {
        // We don't use the kernel parameter for our simple implementation
        return RenderAsync(arguments ?? new KernelArguments(), cancellationToken);
    }
} 