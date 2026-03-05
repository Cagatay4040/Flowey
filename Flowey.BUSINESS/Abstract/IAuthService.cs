using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flowey.BUSINESS.DTO.User;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DOMAIN.Model.Concrete;

namespace Flowey.BUSINESS.Abstract
{
    public interface IAuthService
    {
        #region Get Methods

        public string GetToken(User user);
        public Task<IDataResult<string>> LoginAsync(UserLoginDTO dto);

        #endregion

        #region Insert Methods

        Task<IResult> RegisterAsync(UserAddDTO dto);

        #endregion

        #region Update Methods

        Task<IResult> ChangePasswordAsync(UserPasswordChangeDTO dto);

        #endregion

        #region Delete Methods



        #endregion
    }
}
