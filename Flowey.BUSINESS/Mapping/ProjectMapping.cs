using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Flowey.BUSINESS.DTO.Project;
using Flowey.BUSINESS.DTO.ProjectUser;
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
