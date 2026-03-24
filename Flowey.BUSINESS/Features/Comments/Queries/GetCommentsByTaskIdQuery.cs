using AutoMapper;
using Flowey.CORE.DTO.Comment;
using Flowey.CORE.Interfaces.Repositories;
using Flowey.CORE.Interfaces.Security;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.SHARED.Constants;
using Flowey.SHARED.Enums;
using MediatR;

namespace Flowey.BUSINESS.Features.Comments.Queries
{
    public class GetCommentsByTaskIdQuery : IRequest<IDataResult<List<CommentGetDTO>>>, IRequireTaskAuthorization
    {
        public Guid TaskId { get; set; }

        public RoleType[] RequiredRoles => new[] { RoleType.Admin, RoleType.Editor, RoleType.Member };

        public GetCommentsByTaskIdQuery(Guid taskId)
        {
            TaskId = taskId;
        }
    }

    public class GetCommentsByTaskIdQueryHandler : IRequestHandler<GetCommentsByTaskIdQuery, IDataResult<List<CommentGetDTO>>>
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IMapper _mapper;

        public GetCommentsByTaskIdQueryHandler(ICommentRepository commentRepository, IMapper mapper)
        {
            _commentRepository = commentRepository;
            _mapper = mapper;
        }

        public async Task<IDataResult<List<CommentGetDTO>>> Handle(GetCommentsByTaskIdQuery request, CancellationToken cancellationToken)
        {
            var comments = await _commentRepository.GetList(c => c.TaskId == request.TaskId && c.IsActive, includes: x => x.User);
            var commentDtos = _mapper.Map<List<CommentGetDTO>>(comments).OrderByDescending(c => c.CreatedDate).ToList();
            return new DataResult<List<CommentGetDTO>>(ResultStatus.Success, Messages.CommentListed, commentDtos);
        }
    }
}
