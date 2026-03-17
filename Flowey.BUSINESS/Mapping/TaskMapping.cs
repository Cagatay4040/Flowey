using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Flowey.CORE.DTO.Task;

namespace Flowey.BUSINESS.Mapping
{
    public class TaskMapping : Profile
    {
        public TaskMapping()
        {
            CreateMap<TaskAddDTO, DOMAIN.Model.Concrete.Task>();
            CreateMap<DOMAIN.Model.Concrete.Task, TaskAddDTO>();

            CreateMap<TaskUpdateDTO, DOMAIN.Model.Concrete.Task>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TaskId));

            CreateMap<DOMAIN.Model.Concrete.Task, TaskUpdateDTO>()
                .ForMember(dest => dest.TaskId, opt => opt.MapFrom(src => src.Id));

            CreateMap<TaskGetDTO, DOMAIN.Model.Concrete.Task>();
            CreateMap<DOMAIN.Model.Concrete.Task, TaskGetDTO>();
        }
    }
}
