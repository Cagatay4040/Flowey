using Flowey.BUSINESS.DTO.User;
using Flowey.CORE.DataAccess.Abstract;
using Flowey.CORE.Result.Abstract;
using Flowey.DOMAIN.Model.Concrete;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Flowey.BUSINESS.Abstract
{
    public interface IUserService
    {
        #region Get Methods

        Task<bool> IsThisEmailUsedAsync(string email);
        Task<List<UserSelectDTO>> SearchUsersAsync(string searchTerm);

        #endregion

        #region Add Methods



        #endregion

        #region Update Methods

        Task<IResult> UpdateAsync(UserUpdateDTO dto);

        #endregion

        #region Delete Methods

        Task<IResult> SoftDeleteAsync(UserDeleteDTO dto);

        #endregion
    }
}
