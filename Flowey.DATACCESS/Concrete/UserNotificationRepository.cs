using Flowey.DATACCESS.Abstract;
using Flowey.DATACCESS.Concrete.EntityFramework.Contexts;
using Flowey.DOMAIN.Model.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.DATACCESS.Concrete
{
    public class UserNotificationRepository : EfEntityRepositoryBase<UserNotification>, IUserNotificationRepository
    {
        private readonly FloweyDbContext _context;

        public UserNotificationRepository(FloweyDbContext dbContext) : base(dbContext) 
        {
            _context = dbContext;
        }

        #region Get Methods

        public async Task<List<UserNotification>> GetUserNotifications(Guid userId)
        {
            var data = await _context.UserNotifications
                                .AsNoTracking()
                                .Where(x => x.UserId == userId)
                                .OrderByDescending(x => x.CreatedDate)
                                .ToListAsync();
            return data;
        }

        #endregion

        #region Inster Methods

        #endregion

        #region Update Methods

        #endregion

        #region Delete Methods

        #endregion
    }
}
