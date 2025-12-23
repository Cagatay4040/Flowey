using Flowey.BUSINESS.DTO.Role;
using Flowey.CORE.Result.Abstract;
using Flowey.DOMAIN.Model.Concrete;
using System.Collections.Generic;

namespace Flowey.BUSINESS.Abstract
{
    public interface IRoleService
    {
        #region Get Methods

        Task<IDataResult<RoleGetDTO>> GetUserRole(Guid projectId);

        #endregion

        #region Insert Methods



        #endregion

        #region Update Methods



        #endregion

        #region Delete Methods



        #endregion
    }
}
