using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using Models;

namespace Data
{
    /// <summary>
    /// A class providing a default implementation of the Get operations.
    /// 
    /// Serves as base for a repository that implements CRUD operations.
    /// </summary>
    /// <typeparam name="TEntity">Class implementing ITranslatableBusinessObject</typeparam>
    public abstract class AbstractRepository<TEntity> : IRepository<TEntity>
        where TEntity : BusinessObject<TEntity>, new()
    {
        #region Properties

        /// <summary>
        /// A property for the IQueryableContext of the Repository's Set
        /// </summary>
        protected DbContext Context { get; set; }

        /// <summary>
        /// A property for the Repository's IQueryableSet{TEntity}
        /// </summary>
        protected DbSet Set { get; set; }

        protected Type EntityType
        {
            get
            {
                if (_entityType == null)
                {
                    _entityType = BusinessObject<TEntity>.Create().GetType();
                }

                return _entityType;
            }
        }
        private static Type _entityType;

        #endregion

        #region IRepository<TEntity> Members

        /// <summary>
        /// Set the Added state on given entity, and apply the changes to the context
        /// </summary>
        /// <param name="entity">the entity to add</param>
        public abstract void Add(TEntity entity);

        /// <summary>
        /// Updates the objectSet with given entity
        /// </summary>
        /// <param name="entity">The entity to update</param>
        public abstract void Update(TEntity entity);

        /// <summary>
        /// Updates the objectSet with given entities
        /// </summary>
        /// <param name="entities">The entities to update</param>
        public abstract void UpdateBatch(ICollection<TEntity> entities);

        /// <summary>
        /// Deletes the entity from its ObjectSet
        /// </summary>
        /// <param name="entity">The entity to delete</param>
        public abstract void Delete(TEntity entity);

        /// <summary>
        /// Deletes the entities given their primary keys
        /// </summary>
        /// <param name="keys">primary keys of the entities to delete</param>
        public abstract void Delete(ICollection<Guid> keys);

        #endregion

        #region IReadonlyRepository<TEntity> Members

        /// <summary>
        /// Converts lambda expressions representing a relationships to strings
        /// </summary>
        /// <param name="relationships"></param>
        /// <returns></returns>
        protected IEnumerable<IEnumerable<string>> TransformExpressionToString(Expression<Func<TEntity, object>>[] relationships)
        {
            var allIncludes = new List<IEnumerable<string>>();

            foreach (var relationship in relationships)
            {
                var includes = new List<IList<string>>();

                GetRelationshipInternal(relationship, includes);

                foreach (var include in includes)
                {
                    allIncludes.Add(include);
                }
            }

            return allIncludes;
        }
        private static void GetRelationshipInternal(Expression path, IList<IList<string>> includes)
        {
            if (path.NodeType == ExpressionType.Lambda)
            {
                GetRelationshipInternal(((LambdaExpression)path).Body, includes);
            }
            else if (path.NodeType == ExpressionType.MemberAccess)
            {
                var memberExpression = (MemberExpression)path;
                GetRelationshipInternal(memberExpression.Expression, includes);
                if (!includes.Any())
                {
                    includes.Add(new List<string>());
                }
                IList<string> parent = includes.Last();
                parent.Add(memberExpression.Member.Name);
            }
            else if (path.NodeType == ExpressionType.Quote)
            {
                var unaryExpression = (UnaryExpression)path;
                GetRelationshipInternal(unaryExpression.Operand, includes);
            }
            else if (path.NodeType == ExpressionType.NewArrayInit)
            {
                var newArrayExpression = (NewArrayExpression)path;

                IList<string> parent = includes.LastOrDefault();
                if (parent == null)
                {
                    throw new InvalidOperationException("Malformed expression passed, a Memberaccess must preceed a Methodcall");
                }

                foreach (Expression expression in newArrayExpression.Expressions)
                {
                    var partialIncludes = new List<IList<string>>();

                    GetRelationshipInternal(expression, partialIncludes);

                    foreach (IList<string> partialInclude in partialIncludes)
                    {
                        foreach (string parentItem in parent)
                        {
                            partialInclude.Insert(0, parentItem);
                        }

                        includes.Add(partialInclude);
                    }
                }
            }
            else if (path.NodeType == ExpressionType.Call)
            {
                var methodCallExpression = (MethodCallExpression)path;
                foreach (Expression argument in methodCallExpression.Arguments)
                {
                    GetRelationshipInternal(argument, includes);
                }
            }
            else if (path.NodeType == ExpressionType.Parameter)
            {
                return;
            }
            else
            {
                throw new InvalidOperationException("Invalid type of expression");
            }
        }

        /// <summary>
        /// Returns an instance matching given primary key
        /// </summary>
        /// <param name="entityId">Primary key of the entity</param>
        /// <param name="relationships">Optional parameter to include related entities</param>
        /// <returns>Instance of the matching entity</returns>
        public TEntity GetById(Guid entityId, params Expression<Func<TEntity, object>>[] relationships)
        {
            return GetByIdInternal(entityId, TransformExpressionToString(relationships));
        }
        /// <summary>
        /// Returns an instance matching given primary key
        /// </summary>
        /// <param name="entityId">Primary key of the entity</param>
        /// <param name="relationships">Optional parameter to include related entities</param>
        /// <returns>Instance of the matching entity</returns>
        protected TEntity GetByIdInternal(Guid entityId, IEnumerable<IEnumerable<string>> relationships = null)
        {
            return Set.Find(entityId) as TEntity;
        }

        /// <summary>
        /// Returns an IQueryable of all the entities in the set
        /// </summary>
        /// <param name="relationships">Optional parameter to include related entities</param>
        /// <returns>IQueryable of all entities in the set</returns>
        public IQueryable<TEntity> GetAll(params Expression<Func<TEntity, object>>[] relationships)
        {
            return GetAllInternal(TransformExpressionToString(relationships));
        }
        /// <summary>
        /// Returns an IQueryable of all the entities in the set
        /// </summary>
        /// <param name="relationships">Optional parameter to include related entities</param>
        /// <returns>IQueryable of all entities in the set</returns>
        protected IQueryable<TEntity> GetAllInternal(IEnumerable<IEnumerable<string>> relationships = null)
        {
            return CreateTrackingQueryable(GetSetAndInclude(relationships));
        }

        /// <summary>
        /// Returns an IQueryable of all the entities for which the filterExpression resolves to true
        /// </summary>
        /// <param name="filterExpression">filter expression</param>
        /// <param name="relationships">Optional parameter to include related entities</param>
        /// <returns>IQueryable of all entities for which the filterExpression resolves to true</returns>
        public IQueryable<TEntity> GetFiltered(Expression<Func<TEntity, bool>> filterExpression, params Expression<Func<TEntity, object>>[] relationships)
        {
            return GetFilteredInternal(filterExpression, TransformExpressionToString(relationships));
        }
        /// <summary>
        /// Returns an IQueryable of all the entities for which the filterExpression resolves to true
        /// </summary>
        /// <param name="filterExpression">filter expression</param>
        /// <param name="relationships">Optional parameter to include related entities</param>
        /// <returns>IQueryable of all entities for which the filterExpression resolves to true</returns>
        protected IQueryable<TEntity> GetFilteredInternal(Expression<Func<TEntity, bool>> filterExpression, IEnumerable<IEnumerable<string>> relationships = null)
        {
            return CreateTrackingQueryable(GetSetAndInclude(relationships).Where(filterExpression));
        }

        /// <summary>
        /// Returns a new IQueryableSet{TEntity} that will produce entities that include given relationships
        /// </summary>
        /// <param name="relationships">Default parameter to include related entities</param>
        /// <returns></returns>
        protected IQueryable<TEntity> GetSetAndInclude(IEnumerable<IEnumerable<string>> relationships)
        {
            DbQuery set = Set;

            bool isTranslatable = typeof(TEntity).GetInterfaces().Any(e => e.Name == "ITranslatableBusinessObject");

            if (isTranslatable)
            {
                set = set.Include("EntityTranslations");
            }

            if (relationships != null && relationships.Any())
            {
                foreach (IEnumerable<string> relationship in relationships)
                {
                    Type propertyType = typeof (TEntity);

                    foreach (string rel in relationship)
                    {
                        propertyType = propertyType.IsGenericType ? 
                            propertyType.GetGenericArguments().FirstOrDefault().GetProperty(rel).PropertyType : 
                            propertyType.GetProperty(rel).PropertyType;
                    }

                    if (propertyType.IsGenericType)
                    {
                        isTranslatable =
                            propertyType.GetGenericArguments().FirstOrDefault().GetInterfaces().Any(
                                e => e.Name == "ITranslatableBusinessObject");
                    }
                    else
                    {
                        isTranslatable = propertyType.GetInterfaces().Any(
                            e => e.Name == "ITranslatableBusinessObject");
                    }

                    set = isTranslatable ? 
                        set.Include(string.Join(".", relationship) + ".EntityTranslations") : 
                        set.Include(string.Join(".", relationship));
                }
            }

            return (IQueryable<TEntity>)set;
        }

        protected IQueryable<TEntity> CreateTrackingQueryable(IQueryable<TEntity> queryable)
        {
            return queryable;
        }

        #endregion

        #region IInternalReadonlyRepository

        TEntity IInternalReadOnlyRepository<TEntity>.GetByIdImplementation(Guid entityId, IEnumerable<IEnumerable<string>> relationships)
        {
            return GetByIdInternal(entityId, relationships);
        }

        IQueryable<TEntity> IInternalReadOnlyRepository<TEntity>.GetAllImplementation(IEnumerable<IEnumerable<string>> relationships)
        {
            return GetAllInternal(relationships);
        }

        IQueryable<TEntity> IInternalReadOnlyRepository<TEntity>.GetFilteredImplementation(Expression<Func<TEntity, bool>> filterExpression, IEnumerable<IEnumerable<string>> relationships)
        {
            return GetFilteredInternal(filterExpression, relationships);
        }

        #endregion
    }
}
