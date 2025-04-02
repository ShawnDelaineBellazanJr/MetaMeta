using System.Threading.Tasks;
using Microsoft.SemanticKernel;

namespace MetaMeta.Core.Abstractions;

/// <summary>
/// Defines a factory for creating prompt templates.
/// </summary>
public interface IPromptTemplateFactory
{
    /// <summary>
    /// Creates a prompt template.
    /// </summary>
    /// <param name="config">The prompt template configuration.</param>
    /// <returns>The created prompt template.</returns>
    IPromptTemplate Create(PromptTemplateConfig config);
} 