using Flowey.BUSINESS.Extensions;
using Flowey.CORE.Constants;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DATACCESS.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.Features.Comments.Commands
{
    public class UpdateCommentCommand : IRequest<IResult>
    {
        public Guid CommentId { get; set; }
        public string Content { get; set; }

        public UpdateCommentCommand(Guid commentId, string content)
        {
            CommentId = commentId;
            Content = content;
        }
    }

    public class UpdateCommentCommandHandler : IRequestHandler<UpdateCommentCommand, IResult>
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCommentCommandHandler(ICommentRepository commentRepository, IUnitOfWork unitOfWork)
        {
            _commentRepository = commentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
        {
            var existingComment = await _commentRepository.GetByIdAsync(request.CommentId);

            if (existingComment == null)
                return new Result(ResultStatus.Error, Messages.CommentNotFound);

            var cleanContent = request.Content.ToSafeRichText();

            existingComment.Content = cleanContent;

            await _commentRepository.UpdateAsync(existingComment);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.CommentUpdated);

            return new Result(ResultStatus.Error, Messages.CommentUpdateError);
        }
    }
}
