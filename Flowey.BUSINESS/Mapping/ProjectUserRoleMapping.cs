using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Flowey.BUSINESS.DTO.ProjectUser;
using Flowey.DOMAIN.Model.Concrete;

namespace Flowey.BUSINESS.Mapping
{
    public class ProjectUserRoleMapping : Profile
    {
        public ProjectUserRoleMapping()
        {
            CreateMap<ProjectUserAddDTO, ProjectUserRole>();
            CreateMap<ProjectUserRole, ProjectUserAddDTO>();

            CreateMap<ProjectUserGetDTO, ProjectUserRole>();
            CreateMap<ProjectUserRole, ProjectUserGetDTO>();
        }
    }
}
