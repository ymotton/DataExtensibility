using System;
using System.Linq;
using System.Linq.Expressions;

namespace Data
{
    /// <summary>
    /// An interface describing a readonly repository with Get operations
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IReadOnlyRepository<TEntity> : IInternalReadOnlyRepository<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// Returns an instance matching given primary key
        /// </summary>
        /// <param name="entityId">Primary key of the entity</param>
        /// <param name="relationships">Optional parameter to include related entities</param>
        /// <returns></returns>
        TEntity GetById(Guid entityId, params Expression<Func<TEntity, object>>[] relationships);

        /// <summary>
        /// Returns an IQueryable of all the entities in the set
        /// </summary>
        /// <param name="relationships">Optional parameter to include related entities</param>
        /// <returns>IQueryable of all entities in the set</returns>
        IQueryable<TEntity> GetAll(params Expression<Func<TEntity, object>>[] relationships);

        /// <summary>
        /// Returns an IQueryable of all the entities for which the filterExpression resolves to true
        /// </summary>
        /// <param name="filterExpression">filter expression</param>
        /// <param name="relationships">Optional parameter to include related entities</param>
        /// <returns>IQueryable of all entities for which the filterexpression resolves to true</returns>
        IQueryable<TEntity> GetFiltered(Expression<Func<TEntity, bool>> filterExpression, params Expression<Func<TEntity, object>>[] relationships);
    }
}
