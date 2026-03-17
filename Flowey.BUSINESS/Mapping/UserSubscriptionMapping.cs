using AutoMapper;
using Flowey.CORE.DTO.User;
using Flowey.DOMAIN.Model.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.Mapping
{
    public class UserSubscriptionMapping : Profile
    {
        public UserSubscriptionMapping()
        {
            CreateMap<UserSubscriptionGetDTO, UserSubscription>();
            CreateMap<UserSubscription, UserSubscriptionGetDTO>();
        }
    }
}
