using AutoMapper;
using Flowey.CORE.DTO.User;
using Flowey.DOMAIN.Model.Concrete;

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
