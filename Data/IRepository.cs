using System;
using System.Collections.Generic;

namespace Data
{
    /// <summary>
    /// An interface describing a repository with CRUD operations
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IRepository<TEntity> : IReadOnlyRepository<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// Add a given entity to the objectSet
        /// </summary>
        /// <param name="entity">The entity to be added</param>
        void Add(TEntity entity);

        /// <summary>
        /// Updates the objectSet with given entity
        /// </summary>
        /// <param name="entity">The entity to be updated</param>
        void Update(TEntity entity);

        /// <summary>
        /// Updates the objectSet with given entities
        /// </summary>
        /// <param name="entities">The entities to be updated</param>
        void UpdateBatch(ICollection<TEntity> entities);

        /// <summary>
        /// Deletes the entity from its objectSet
        /// </summary>
        /// <param name="entity">The entity to be deleted</param>
        void Delete(TEntity entity);

        /// <summary>
        /// Deletes the entities given their primary keys
        /// </summary>
        /// <param name="keys">The primary keys of the entities to be deleted</param>
        void Delete(ICollection<Guid> keys);
    }
}
