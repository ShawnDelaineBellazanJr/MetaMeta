using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaMeta.Core.Abstractions
{
    /// <summary>
    /// Base class for all domain entities in the MetaMeta system.
    /// </summary>
    /// <remarks>
    /// Provides common properties and behaviors for all entities,
    /// including unique identification and creation tracking.
    /// </remarks>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Unique identifier for the entity. Defaults to a new GUID string.
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();
       
        /// <summary>
        /// UTC timestamp when the entity was created. Automatically set on instantiation.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
