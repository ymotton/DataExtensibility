using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Data
{
    public interface IInternalReadOnlyRepository<TEntity>
        where TEntity : class
    {
        TEntity GetByIdImplementation(Guid entityId, IEnumerable<IEnumerable<string>> relationships);

        IQueryable<TEntity> GetAllImplementation(IEnumerable<IEnumerable<string>> relationships);

        IQueryable<TEntity> GetFilteredImplementation(Expression<Func<TEntity, bool>> filterExpression, IEnumerable<IEnumerable<string>> relationships);
    }
}
