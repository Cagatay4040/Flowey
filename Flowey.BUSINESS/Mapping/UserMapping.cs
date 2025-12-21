using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Flowey.BUSINESS.DTO.User;
using Flowey.DOMAIN.Model.Concrete;

namespace Flowey.BUSINESS.Mapping
{
    public class UserMapping : Profile
    {
        public UserMapping()
        {
            CreateMap<UserAddDTO, User>();
            CreateMap<User,UserAddDTO>();

            CreateMap<UserUpdateDTO, User>();
            CreateMap<User, UserUpdateDTO>();

            CreateMap<UserGetDTO, User>();
            CreateMap<User, UserGetDTO>();

            CreateMap<UserDeleteDTO, User>();
            CreateMap<User, UserDeleteDTO>();

            CreateMap<UserLoginDTO, User>();
            CreateMap<User, UserLoginDTO>();
        }
    }
}
