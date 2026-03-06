using Flowey.CORE.DataAccess.Abstract;
using Flowey.DOMAIN.Model.Abstract;
using Flowey.DOMAIN.Model.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.DATACCESS.Services
{
    public class AuditInterceptor : SaveChangesInterceptor
    {
        private readonly ICurrentUserService _currentUserService;

        public AuditInterceptor(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            var context = eventData.Context;
            if (context == null) return base.SavingChangesAsync(eventData, result, cancellationToken);

            var currentUserId = _currentUserService.GetUserIdOrDefault();

            foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.IsActive = true;
                    entry.Entity.CreatedDate = DateTime.UtcNow;

                    if (currentUserId.HasValue)
                    {
                        entry.Entity.CreatedBy = currentUserId.Value;
                    }
                    else
                    {
                        if (entry.Entity is User userEntity)
                        {
                            entry.Entity.CreatedBy = userEntity.Id;
                        }
                        else
                        {
                            entry.Entity.CreatedBy = Guid.Empty;
                        }
                    }
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.ModifiedBy = currentUserId;
                    entry.Entity.ModifiedDate = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    entry.Entity.IsActive = false;
                    entry.Entity.ModifiedBy = currentUserId;
                    entry.Entity.ModifiedDate = DateTime.UtcNow;
                }
            }

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
