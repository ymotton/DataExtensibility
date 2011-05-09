using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using Models;

namespace Data
{
    /// <summary>
    /// A Repository implementation that supports Self-Tracking Entities and implements CRUD operations
    /// </summary>
    /// <typeparam name="TEntity">Class that implementes IObjectWithChangeTracker, IValidatableBusinessObject, ITranslatableBusinessObject</typeparam>
    public class Repository<TEntity> : AbstractRepository<TEntity>
        where TEntity : BusinessObject<TEntity>, new()
    {
        public Repository(DbContext context)
        {
            Context = context;
            
            Set = context.Set(EntityType);
        }

        #region IRepository<TEntity> Members

        /// <summary>
        /// Set the Added state on given entity, and apply the changes to the context
        /// </summary>
        /// <param name="entity">Added state will be set for this entity</param>
        public override void Add(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            Set.Add(entity);
        }

        /// <summary>
        /// Update the entity, and apply the changes to the context
        /// The entity's state determines the actual operation executed
        /// </summary>
        /// <param name="entity">entity to be updated</param>
        public override void Update(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            Set.Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;
        }

        /// <summary>
        /// Update in batch by running through the list and imply the correct CRUD operations by inspecting the entity state
        /// </summary>
        /// <param name="entities">list of entities</param>
        public override void UpdateBatch(ICollection<TEntity> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }
            
            foreach (TEntity entity in entities)
            {
                Set.Attach(entity);
                Context.Entry(entity).State = EntityState.Modified;
            }
        }

        /// <summary>
        /// Set the Deleted state on given entity, and apply the changes to the context
        /// </summary>
        /// <param name="entity">Deleted state will be set for this entity</param>
        public override void Delete(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            
            var entry = Context.Entry(entity);
            if (entry == null)
            {
                Set.Attach(entity);
                entry = Context.Entry(entity);
            }
            entry.State = EntityState.Deleted;
        }

        /// <summary>
        /// Set the deleted state on all entities for which the keys are provided
        /// </summary>
        /// <param name="keys">Keys of the entities for whom the deleted state will be set</param>
        public override void Delete(ICollection<Guid> keys)
        {
            if (keys == null)
            {
                throw new ArgumentNullException("keys");
            }

            if (keys.Count == 0)
            {
                return;
            }

            foreach (Guid key in keys)
            {
				TEntity entity = GetById(key);

                Set.Remove(entity);
            }
        }

        #endregion
    }
}
