using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Flowey.CORE.DataAccess.Abstract;
using Flowey.DATACCESS.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Flowey.DATACCESS.Concrete
{
    public class EfEntityRepositoryBase<TEntity> : IEntityRepository<TEntity> where TEntity : class, IEntity, new()
    {
        private readonly DbContext dbContext;

        protected DbSet<TEntity> entity => dbContext.Set<TEntity>();

        public EfEntityRepositoryBase(DbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        #region Insert Methods
        public virtual async Task AddAsync(TEntity entity)
        {
            await this.entity.AddAsync(entity);
        }

        public virtual void Add(TEntity entity)
        {
            this.entity.Add(entity);
        }

        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            if (entities != null && !entities.Any())
                return;

            await entity.AddRangeAsync(entities);
        }

        public virtual void AddRange(IEnumerable<TEntity> entities)
        {
            if (entities != null && !entities.Any())
                return;

            entity.AddRange(entities);
        }
        #endregion

        #region Update Methods
        public virtual Task UpdateAsync(TEntity entity)
        {
            this.entity.Attach(entity);
            dbContext.Entry(entity).State = EntityState.Modified;
            return Task.CompletedTask;
        }

        public virtual void Update(TEntity entity)
        {
            this.entity.Attach(entity);
            dbContext.Entry(entity).State = EntityState.Modified;
        }

        public virtual void UpdateRange(List<TEntity> entities)
        {
            dbContext.UpdateRange(entities);
        }

        public virtual Task UpdateRangeAsync(List<TEntity> entities)
        {
            dbContext.UpdateRange(entities);
            return Task.CompletedTask;
        }
        #endregion

        #region Delete Methods
        public virtual Task DeleteAsync(Guid id)
        {
            var entity = this.entity.Find(id);
            return DeleteAsync(entity);
        }

        public virtual void Delete(Guid id)
        {
            var entity = this.entity.Find(id);
            Delete(entity);
        }

        public virtual Task DeleteAsync(TEntity entity)
        {
            if (dbContext.Entry(entity).State == EntityState.Detached)
            {
                this.entity.Attach(entity);
            }

            this.entity.Remove(entity);

            return Task.CompletedTask;
        }

        public virtual void Delete(TEntity entity)
        {
            if (dbContext.Entry(entity).State == EntityState.Detached)
            {
                this.entity.Attach(entity);
            }

            this.entity.Remove(entity);
        }

        public virtual void SoftDelete(TEntity entity)
        {
            entity.GetType().GetProperty("IsActive").SetValue(entity, false);
            this.entity.Attach(entity);
            dbContext.Entry(entity).State = EntityState.Modified;
        }

        public async virtual Task SoftDeleteAsync(TEntity entity)
        {
            entity.GetType().GetProperty("IsActive").SetValue(entity, false);
            this.entity.Attach(entity);
            dbContext.Entry(entity).State = EntityState.Modified;
            await Task.CompletedTask;
        }

        public virtual void DeleteRange(Expression<Func<TEntity, bool>> predicate)
        {
            dbContext.RemoveRange(entity.Where(predicate));
        }

        public virtual async Task DeleteRangeAsync(Expression<Func<TEntity, bool>> predicate)
        {
            dbContext.RemoveRange(entity.Where(predicate));
            await Task.CompletedTask;
        }
        #endregion

        #region Get Methods
        public virtual IQueryable<TEntity> AsQueryable() => entity.AsQueryable();

        public virtual IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate, bool noTracking = true, params Expression<Func<TEntity, object>>[] includes)
        {
            var query = entity.AsQueryable();

            if (predicate != null)
                query = query.Where(predicate);

            query = ApplyIncludes(query, includes);

            if (noTracking)
                query = query.AsNoTracking();

            return query;
        }

        public virtual Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool noTracking = true, params Expression<Func<TEntity, object>>[] includes)
        {
            return Get(predicate, noTracking, includes).FirstOrDefaultAsync();
        }

        public virtual async Task<List<TEntity>> GetList(Expression<Func<TEntity, bool>> predicate, bool noTracking = true, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = entity;

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            foreach (Expression<Func<TEntity, object>> include in includes)
            {
                query = query.Include(include);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (noTracking)
                query = query.AsNoTracking();

            return await query.ToListAsync();
        }

        public virtual async Task<List<TEntity>> GetAll(bool noTracking = true)
        {
            if (noTracking)
                return await entity.AsNoTracking().ToListAsync();

            return await entity.ToListAsync();
        }

        public virtual async Task<TEntity> GetByIdAsync(Guid id, bool noTracking = true, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = dbContext.Set<TEntity>().AsQueryable();

            if (includes != null && includes.Any())
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            if (noTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id);
        }

        public virtual bool Any(Expression<Func<TEntity, bool>> predicate)
        {
            return entity.Any(predicate);
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await entity.AnyAsync(predicate);
        }

        public virtual int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return entity.Count(predicate);
        }

        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await entity.CountAsync(predicate);
        }

        public virtual async Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> predicate, bool noTracking = true, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = entity;

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            query = ApplyIncludes(query, includes);

            if (noTracking)
                query = query.AsNoTracking();

            return await query.SingleOrDefaultAsync();

        }

        private static IQueryable<TEntity> ApplyIncludes(IQueryable<TEntity> query, params Expression<Func<TEntity, object>>[] includes)
        {
            if (includes != null)
            {
                foreach (var includeItem in includes)
                {
                    query = query.Include(includeItem);
                }
            }

            return query;
        }
        #endregion
    }
}
