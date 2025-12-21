using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Flowey.BUSINESS.DTO.Task;

namespace Flowey.BUSINESS.Mapping
{
    public class TaskMapping : Profile
    {
        public TaskMapping()
        {
            CreateMap<TaskAddDTO, DOMAIN.Model.Concrete.Task>();
            CreateMap<DOMAIN.Model.Concrete.Task, TaskAddDTO>();

            CreateMap<TaskUpdateDTO, DOMAIN.Model.Concrete.Task>();
            CreateMap<DOMAIN.Model.Concrete.Task, TaskUpdateDTO>();

            CreateMap<TaskGetDTO, DOMAIN.Model.Concrete.Task>();
            CreateMap<DOMAIN.Model.Concrete.Task, TaskGetDTO>();
        }
    }
}
