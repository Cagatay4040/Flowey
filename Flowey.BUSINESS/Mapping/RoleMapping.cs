using AutoMapper;
using Flowey.BUSINESS.DTO.Role;
using Flowey.DOMAIN.Model.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.Mapping
{
    public class RoleMapping : Profile
    {
        public RoleMapping()
        {
            CreateMap<RoleGetDTO, Role>();
            CreateMap<Role, RoleGetDTO>();
        }
    }
}
