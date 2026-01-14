using Flowey.CORE.DataAccess.Concrete;
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
    public class UserRepository : EfEntityRepositoryBase<User>, IUserRepository
    {
        private readonly FloweyDbContext _context;

        public UserRepository(FloweyDbContext dbContext) : base(dbContext) 
        {
            _context = dbContext;
        }


        #region Get Methods

        public async Task<List<User>> GetUsersByIdListAsync(List<Guid> userIds)
        {
            var data = await _context.Users
                         .Where(u => userIds.Contains(u.Id))
                         .Select(u => new User { Id = u.Id, Email = u.Email, Name = u.Name, Surname = u.Surname })
                         .ToListAsync();

            return data;
        }

        #endregion

        #region Insert Methods



        #endregion

        #region Update Methods



        #endregion

        #region Delete Methods



        #endregion
    }
}
