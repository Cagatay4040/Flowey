using AutoMapper;
using Flowey.BUSINESS.DTO.Comment;
using Flowey.CORE.Constants;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DATACCESS.Abstract;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.Features.Comments.Queries
{
    public class GetCommentsByTaskIdQuery : IRequest<IDataResult<List<CommentGetDTO>>>
    {
        public Guid TaskId { get; set; }

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
