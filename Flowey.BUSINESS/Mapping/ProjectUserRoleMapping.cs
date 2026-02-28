using AutoMapper;
using Flowey.BUSINESS.DTO.Project;
using Flowey.BUSINESS.DTO.ProjectUser;
using Flowey.DOMAIN.Model.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            CreateMap<ProjectUserRole, ProjectGetDTO>()
                .IncludeMembers(src => src.Project)
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ProjectId))
                .ForMember(dest => dest.CurrentUserRole, opt => opt.MapFrom(src => src.Role.Code));
        }
    }
}
