using Flowey.CORE.Constants;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DATACCESS.Abstract;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.Features.Comments.Commands
{
    public class DeleteCommentCommand : IRequest<IResult>
    {
        public Guid Id { get; set; }

        public DeleteCommentCommand(Guid id)
        {
            Id = id;
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
            var existingComment = await _commentRepository.GetByIdAsync(request.Id);

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
