using AutoMapper;
using Flowey.BUSINESS.DTO.Notification;
using Flowey.BUSINESS.DTO.User;
using Flowey.DOMAIN.Model.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
