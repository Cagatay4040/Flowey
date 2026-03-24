using Flowey.CORE.Interfaces.Repositories;
using Flowey.CORE.Interfaces.Security;
using Flowey.CORE.Interfaces.UnitOfWork;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.SHARED.Constants;
using Flowey.SHARED.Enums;
using MediatR;

namespace Flowey.BUSINESS.Features.Comments.Commands
{
    public class DeleteCommentCommand : IRequest<IResult>, IRequireCommentAuthorization
    {
        public Guid CommentId { get; set; }

        public RoleType[] RequiredRoles => new[] { RoleType.Admin, RoleType.Editor, RoleType.Member };

        public DeleteCommentCommand(Guid commentId)
        {
            CommentId = commentId;
        }
    }

    public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, IResult>
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCommentCommandHandler(ICommentRepository commentRepository, IUnitOfWork unitOfWork)
        {
            _commentRepository = commentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
        {
            var existingComment = await _commentRepository.GetByIdAsync(request.CommentId);

            if (existingComment == null) 
                return new Result(ResultStatus.Error, Messages.CommentNotFound);

            await _commentRepository.SoftDeleteAsync(existingComment);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.CommentDeleted);

            return new Result(ResultStatus.Error, Messages.CommentDeleteError);
        }
    }
}
