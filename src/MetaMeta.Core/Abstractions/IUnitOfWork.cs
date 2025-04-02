using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaMeta.Core.Abstractions
{
    /// <summary>
    /// Unit of Work interface providing a way to coordinate multiple repositories
    /// and ensure atomic operations across them.
    /// </summary>
    /// <remarks>
    /// Implements the Unit of Work pattern to maintain a list of business objects affected
    /// by a business transaction and coordinates the writing out of changes.
    /// </remarks>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Gets the repository for working with Assistant entities.
        /// </summary>
        IAssistantRepository Assistants { get; }
        
        /// <summary>
        /// Saves all pending changes to the data store.
        /// </summary>
        /// <returns>The number of affected records.</returns>
        Task<int> SaveChangesAsync();
    }
}
