using AutoMapper;
using Flowey.CORE.DTO.Project;
using Flowey.DOMAIN.Model.Concrete;

namespace Flowey.BUSINESS.Mapping
{
    public class ProjectMapping : Profile
    {
        public ProjectMapping()
        {
            CreateMap<ProjectGetDTO, Project>();
            CreateMap<Project, ProjectGetDTO>();

            CreateMap<ProjectAddDTO, Project>();
            CreateMap<Project, ProjectAddDTO>();

            CreateMap<ProjectUpdateDTO, Project>();
            CreateMap<Project, ProjectUpdateDTO>();
        }
    }
}
