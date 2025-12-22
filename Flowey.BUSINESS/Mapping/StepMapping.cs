using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Flowey.BUSINESS.DTO.Step;
using Flowey.DOMAIN.Model.Concrete;

namespace Flowey.BUSINESS.Mapping
{
    public class StepMapping : Profile
    {
        public StepMapping()
        {
            CreateMap<StepGetDTO, Step>();
            CreateMap<Step, StepGetDTO>();

            CreateMap<StepAddDTO, Step>();
            CreateMap<Step, StepAddDTO>();

            CreateMap<StepUpdateDTO, Step>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.StepId));

            CreateMap<Step, StepUpdateDTO>()
                .ForMember(dest => dest.StepId, opt => opt.MapFrom(src => src.Id));
        }
    }
}
