using System.Linq.Expressions;

namespace HomeworkService.Repositories;

public interface ICrudRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Creates an entity
    /// </summary>
    /// <param name="entity">New entity</param>
    /// <param name="ct">Cancellation Token</param>
    Task CreateAsync(TEntity entity, CancellationToken ct = default);

    /// <summary>
    /// Creates range of entities
    /// </summary>
    /// <param name="entities">Range of new entities</param>
    /// <param name="ct">Cancellation Token</param>
    Task CreateRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default);

    /// <summary>
    /// Gets entity by it's id from database
    /// </summary>
    /// <param name="id">Entity's id</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="includeProperties">A lambda expression representing the navigation property to be included (e => e.Property1).</param>
    /// <param name="ct">Cancellation Token</param>
    /// <returns>Returns entity if it exists, if not returns null</returns>
    Task<TEntity?> GetByIdAsync(
        object id,
        Expression<Func<TEntity, bool>>? predicate = null,
        Expression<Func<TEntity, object>>[]? includeProperties = null,
        CancellationToken ct = default);
    
    /// <summary>
    /// Gets list of entities from database
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="includeProperties">A lambda expression representing the navigation property to be included (e => e.Property1).</param>
    /// <param name="ct">Cancellation Token</param>
    /// <returns>List of entities</returns>
    Task<IEnumerable<TEntity>> GetAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Expression<Func<TEntity, object>>[]? includeProperties = null,
        CancellationToken ct = default);
    
    /// <summary>
    /// Updates entity by it's id in database
    /// </summary>
    /// <param name="id">Entity's id</param>
    /// <param name="entity">Updated entity</param>
    /// <param name="ct">Cancellation Token</param>
    /// <returns>Returns true if it was updated, or false if it was not found</returns>
    Task<bool> UpdateAsync(object id, TEntity entity, CancellationToken ct = default);
    
    /// <summary>
    /// Deletes entity by it's id in database
    /// </summary>
    /// <param name="id">Entity's id</param>
    /// <param name="ct">Cancellation Token</param>
    /// <returns>Returns true if it was deleted, or false if it was not found</returns>
    Task<bool> DeleteByIdAsync(object id, CancellationToken ct = default);
    
    /// <summary>
    /// Deletes range of entities in database
    /// </summary>
    /// <param name="entities">Range of entities to delete</param>
    /// <param name="ct">Cancellation Token</param>
    Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default);
    
    /// <summary>
    /// Deletes entity in database
    /// </summary>
    /// <param name="entity">Entity to delete</param>
    /// <param name="ct">Cancellation Token</param>
    /// <returns>Returns true if it was deleted, or false if it was not found</returns>
    Task DeleteAsync(TEntity entity, CancellationToken ct = default);

    /// <summary>
    /// Saves changes in database
    /// </summary>
    /// <param name="ct">Cancellation Token</param>
    Task SaveAsync(CancellationToken ct = default);
}