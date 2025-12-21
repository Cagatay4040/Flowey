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

        Task<User> GetUserByEmailAsync(string email);
        Task<bool> IsThisEmailUsedAsync(string email);

        #endregion

        #region Add Methods

        Task<IResult> AddAsync(UserAddDTO dto);

        #endregion

        #region Update Methods

        Task<IResult> UpdateAsync(UserUpdateDTO dto);
        Task<IResult> ChangePasswordAsync(UserPasswordChangeDTO dto);

        #endregion

        #region Delete Methods

        Task<IResult> SoftDeleteAsync(UserDeleteDTO dto);

        #endregion
    }
}
