using System.Threading;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;

namespace MetaMeta.Core.Abstractions;

/// <summary>
/// Interface for prompt templates in the MetaMeta system.
/// </summary>
public interface IPromptTemplate
{
    /// <summary>
    /// Gets the name of the template.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the description of the template.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Gets the template string.
    /// </summary>
    string Template { get; }

    /// <summary>
    /// Renders the template with the provided arguments.
    /// </summary>
    /// <param name="arguments">The arguments to render the template with.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The rendered template.</returns>
    Task<string> RenderAsync(KernelArguments arguments, CancellationToken cancellationToken = default);

    /// <summary>
    /// Renders the template with the provided kernel and arguments.
    /// </summary>
    /// <param name="kernel">The kernel to use for rendering.</param>
    /// <param name="arguments">The arguments to render the template with.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The rendered template.</returns>
    Task<string> RenderAsync(Kernel kernel, KernelArguments? arguments, CancellationToken cancellationToken = default);
} 