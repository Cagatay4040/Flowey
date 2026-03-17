using Flowey.CORE.DTO.User;
using Flowey.CORE.Result.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.CORE.Interfaces.Services
{
    public interface ISubscriptionService
    {
        #region Get Methods

        Task<IDataResult<List<UserSubscriptionGetDTO>>> GetBillingHistoryAsync(Guid userId);

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
