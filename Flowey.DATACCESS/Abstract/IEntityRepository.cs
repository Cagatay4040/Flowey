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
        #region Add Methods
        Task<int> AddAsync(TEntity entity);
        int Add(TEntity entity);
        int AddRange(IEnumerable<TEntity> entities);
        Task<int> AddRangeAsync(IEnumerable<TEntity> entities);

        #endregion

        #region Update Methods
        Task<int> UpdateAsync(TEntity entity);
        int Update(TEntity entity);
        int UpdateRange(List<TEntity> entities);
        Task<int> UpdateRangeAsync(List<TEntity> entities);

        #endregion

        #region Delete Methods
        Task<int> DeleteAsync(Guid id);
        int Delete(Guid id);
        Task<int> DeleteAsync(TEntity entity);
        int Delete(TEntity entity);
        int SoftDelete(TEntity entity);
        Task<int> SoftDeleteAsync(TEntity entity);
        bool DeleteRange(Expression<Func<TEntity, bool>> predicate);
        Task<bool> DeleteRangeAsync(Expression<Func<TEntity, bool>> predicate);

        #endregion

        #region Get Methods
        Task<List<TEntity>> GetAll(bool noTracking = true);
        Task<List<TEntity>> GetList(Expression<Func<TEntity, bool>> predicate, bool noTracking = true, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params Expression<Func<TEntity, object>>[] includes);
        Task<TEntity> GetByIdAsync(Guid id, bool noTracking = true, params Expression<Func<TEntity, object>>[] includes);
        bool Any(Expression<Func<TEntity, bool>> predicate);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);
        int Count(Expression<Func<TEntity, bool>> predicate);
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> predicate, bool noTracking = true, params Expression<Func<TEntity, object>>[] includes);
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool noTracking = true, params Expression<Func<TEntity, object>>[] includes);
        IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate, bool noTracking = true, params Expression<Func<TEntity, object>>[] includes);

        #endregion
    }
}
