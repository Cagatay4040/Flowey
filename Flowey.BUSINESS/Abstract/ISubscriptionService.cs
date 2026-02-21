using Flowey.BUSINESS.DTO.User;
using Flowey.CORE.Result.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.Abstract
{
    public interface ISubscriptionService
    {
        #region Get Methods


        #endregion

        #region Add Methods

        Task<IDataResult<string>> CheckoutAsync(UserCheckoutRequestDTO checkoutDto);

        #endregion

        #region Update Methods

        #endregion

        #region Delete Methods

        #endregion
    }
}
