using MetaMeta.Core.Abstractions;
using MetaMeta.Infrastructure.Data;
using MetaMeta.Infrastructure.Repositories;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaMeta.Infrastructure
{
    /// <summary>
    /// Implementation of the Unit of Work pattern providing a way to coordinate 
    /// multiple repositories and ensure atomic operations.
    /// </summary>
    /// <remarks>
    /// Manages the database context for multiple repositories and provides
    /// a way to commit all changes in a single transaction.
    /// </remarks>
    public class UnitOfWork : IUnitOfWork
    {
        // The database context shared by all repositories
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the UnitOfWork class.
        /// </summary>
        /// <param name="context">The database context to use for data access.</param>
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            
            // Initialize repositories
            Assistants = new AssistantRepository(_context);
        }
        
        /// <summary>
        /// Gets the repository for working with Assistant entities.
        /// </summary>
        public IAssistantRepository Assistants { get; set; }

        /// <summary>
        /// Saves all pending changes to the database.
        /// </summary>
        /// <returns>The number of affected records.</returns>
        public async Task<int> SaveChangesAsync()
        {
            // Persist all changes to the database in a single transaction
            return await _context.SaveChangesAsync();
        }
    }
}
