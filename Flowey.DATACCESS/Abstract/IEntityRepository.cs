using Flowey.CORE.DataAccess.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.DATACCESS.Abstract
{
    public interface IEntityRepository<TEntity> where TEntity : class, IEntity, new()
    {
        #region Get Methods
        Task<List<TEntity>> GetAll(bool noTracking = true);
        Task<List<TEntity>> GetList(Expression<Func<TEntity, bool>> predicate, bool noTracking = true, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params Expression<Func<TEntity, object>>[] includes);
        Task<(List<TEntity> Items, int TotalCount)> GetPagedListAsync(Expression<Func<TEntity, bool>> predicate = null, int pageIndex = 0, int pageSize = 10, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, bool noTracking = true, params Expression<Func<TEntity, object>>[] includes);
        IQueryable<TEntity> GetQueryable(Expression<Func<TEntity, bool>> predicate = null, bool noTracking = true);
        Task<TEntity> GetByIdAsync(Guid id, bool noTracking = true, params Expression<Func<TEntity, object>>[] includes);
        bool Any(Expression<Func<TEntity, bool>> predicate, bool ignoreQueryFilter = false);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, bool ignoreQueryFilter = false);
        int Count(Expression<Func<TEntity, bool>> predicate, bool ignoreQueryFilter = false);
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, bool ignoreQueryFilter = false);
        Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> predicate, bool noTracking = true, params Expression<Func<TEntity, object>>[] includes);
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool noTracking = true, params Expression<Func<TEntity, object>>[] includes);
        IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate, bool noTracking = true, params Expression<Func<TEntity, object>>[] includes);

        #endregion

        #region Add Methods
        Task AddAsync(TEntity entity);
        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        Task AddRangeAsync(IEnumerable<TEntity> entities);

        #endregion

        #region Update Methods
        Task UpdateAsync(TEntity entity);
        void Update(TEntity entity);
        void UpdateRange(List<TEntity> entities);
        Task UpdateRangeAsync(List<TEntity> entities);

        #endregion

        #region Delete Methods
        Task DeleteAsync(Guid id);
        void Delete(Guid id);
        Task DeleteAsync(TEntity entity);
        void Delete(TEntity entity);
        void SoftDelete(TEntity entity);
        Task SoftDeleteAsync(TEntity entity);
        void DeleteRange(Expression<Func<TEntity, bool>> predicate);
        Task DeleteRangeAsync(Expression<Func<TEntity, bool>> predicate);

        #endregion
    }
}
