using AutoMapper;
using Flowey.CORE.DTO.Comment;
using Flowey.DOMAIN.Model.Concrete;

namespace Flowey.BUSINESS.Mapping
{
    public class CommentMapping : Profile
    {
        public CommentMapping()
        {
            CreateMap<CommentAddDTO, Comment>();
            CreateMap<Comment, CommentAddDTO>();

            CreateMap<CommentUpdateDTO, Comment>();
            CreateMap<Comment, CommentUpdateDTO>();

            CreateMap<CommentGetDTO, Comment>();
            CreateMap<Comment, CommentGetDTO>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.Name + " " + src.User.Surname : ""));
        }
    }
}
