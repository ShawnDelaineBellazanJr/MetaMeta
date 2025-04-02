using MetaMeta.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaMeta.Core.Abstractions
{
    /// <summary>
    /// Repository interface for Assistant entities, providing specialized data access operations.
    /// </summary>
    /// <remarks>
    /// Extends the generic repository interface with assistant-specific operations.
    /// This interface allows for future extension with assistant-specific query methods
    /// without modifying the generic repository.
    /// </remarks>
    public interface IAssistantRepository: IGenericRepository<Assistant>
    {
        // Additional assistant-specific repository methods can be added here
    }
}
