using Flowey.DOMAIN.Model.Concrete;

namespace Flowey.CORE.Interfaces.Repositories
{
    public interface IUserRepository : IEntityRepository<User>
    {
        #region Get Methods

        Task<List<User>> GetUsersByIdListAsync(List<Guid> userIds);

        #endregion

        #region Insert Methods



        #endregion

        #region Update Methods



        #endregion

        #region Delete Methods



        #endregion
    }
}
