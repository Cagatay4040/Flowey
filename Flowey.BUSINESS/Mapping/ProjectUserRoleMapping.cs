using AutoMapper;
using Flowey.CORE.DTO.Project;
using Flowey.CORE.DTO.ProjectUser;
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

            CreateMap<ProjectUserRole, ProjectGetDTO>()
                .IncludeMembers(src => src.Project)
                .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.ProjectId))
                .ForMember(dest => dest.CurrentUserRole, opt => opt.MapFrom(src => src.RoleId.ToString().ToUpperInvariant()));
        }
    }
}
