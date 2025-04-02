using MetaMeta.Core.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Memory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaMeta.Infrastructure.Repositories
{
    /// <summary>
    /// Generic repository implementation that provides standard data operations for entities.
    /// </summary>
    /// <typeparam name="T">The entity type that derives from BaseEntity.</typeparam>
    /// <remarks>
    /// Implements the IGenericRepository interface using Entity Framework Core 
    /// for data access operations across different entity types.
    /// </remarks>
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        // The database context used for data access operations
        private readonly DbContext _context;
        
        // The DbSet for the current entity type
        protected readonly DbSet<T> _dbSet;

        /// <summary>
        /// Initializes a new instance of the GenericRepository class.
        /// </summary>
        /// <param name="context">The database context to use for data access.</param>
        public GenericRepository(DbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        /// <summary>
        /// Adds a new entity to the database.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task AddAsync(T entity)
        {
            // Add the entity to the DbSet asynchronously
            await _dbSet.AddAsync(entity);
        }

        /// <summary>
        /// Deletes an entity from the database.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task DeleteAsync(T entity)
        {
            // Remove the entity from the DbSet
            _dbSet.Remove(entity);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Retrieves all entities from the database.
        /// </summary>
        /// <returns>An enumerable collection of all entities.</returns>
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            // Retrieve all entities as a list asynchronously
            return await _dbSet.ToListAsync();
        }

        /// <summary>
        /// Retrieves an entity by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to retrieve.</param>
        /// <returns>The found entity.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the entity with the specified ID is not found.</exception>
        public async Task<T> GetByIdAsync(Guid id)
        {
            // Find the entity by its ID asynchronously
            var entity = await _dbSet.FindAsync(id);
            return entity ?? throw new KeyNotFoundException($"Entity with id {id} not found");
        }

        /// <summary>
        /// Updates an existing entity in the database.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task UpdateAsync(T entity)
        {
            // Update the entity in the DbSet
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }
    }
}
