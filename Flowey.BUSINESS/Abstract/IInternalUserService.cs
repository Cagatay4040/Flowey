using Flowey.CORE.Result.Abstract;
using Flowey.DOMAIN.Model.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.Abstract
{
    public interface IInternalUserService
    {
        #region Get Methods

        Task<User> GetUserByIdAsync(Guid id);

        Task<User> GetUserByEmailAsync(string email);

        #endregion

        #region Insert Methods

        Task<IResult> AddAsync(User user);

        #endregion

        #region Update Methods

        Task<IResult> UpdateAsync(User user);

        #endregion

        #region Delete Methods


        #endregion
    }
}
