using AutoMapper;
using Flowey.CORE.DTO.Notification;
using Flowey.DOMAIN.Model.Concrete;

namespace Flowey.BUSINESS.Mapping
{
    public class UserNotificationMapping : Profile
    {
        public UserNotificationMapping()
        {
            CreateMap<UserNotificationGetDTO, UserNotification>();
            CreateMap<UserNotification, UserNotificationGetDTO>();

            CreateMap<UserNotificationAddDTO, UserNotification>();
            CreateMap<UserNotification, UserNotificationAddDTO>();
        }
    }
}
