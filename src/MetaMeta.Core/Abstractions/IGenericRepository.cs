using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaMeta.Core.Abstractions
{
    /// <summary>
    /// Generic repository interface that defines standard data operations for entities.
    /// </summary>
    /// <typeparam name="T">The entity type that derives from BaseEntity.</typeparam>
    /// <remarks>
    /// Provides a common contract for CRUD operations on any entity type,
    /// promoting consistent data access patterns across the application.
    /// </remarks>
    public interface IGenericRepository<T> where T : BaseEntity
    {
        /// <summary>
        /// Retrieves an entity by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to retrieve.</param>
        /// <returns>The found entity or throws an exception if not found.</returns>
        Task<T> GetByIdAsync(Guid id);
        
        /// <summary>
        /// Retrieves all entities of type T from the repository.
        /// </summary>
        /// <returns>An enumerable collection of all entities.</returns>
        Task<IEnumerable<T>> GetAllAsync();
        
        /// <summary>
        /// Adds a new entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddAsync(T entity);
        
        /// <summary>
        /// Updates an existing entity in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateAsync(T entity);
        
        /// <summary>
        /// Deletes an entity from the repository.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeleteAsync(T entity);
    }
}
