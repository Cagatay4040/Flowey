using AutoMapper;
using Flowey.BUSINESS.DTO.Comment;
using Flowey.BUSINESS.Extensions;
using Flowey.CORE.Constants;
using Flowey.CORE.DataAccess.Abstract;
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
        public CommentUpdateDTO Dto { get; set; }

        public UpdateCommentCommand(CommentUpdateDTO dto)
        {
            Dto = dto;
        }
    }

    public class UpdateCommentCommandHandler : IRequestHandler<UpdateCommentCommand, IResult>
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCommentCommandHandler(ICommentRepository commentRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _commentRepository = commentRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
        {
            var existingComment = await _commentRepository.GetByIdAsync(request.Dto.CommentId);

            if (existingComment == null)
                return new Result(ResultStatus.Error, Messages.CommentNotFound);

            var cleanContent = request.Dto.Content.ToSafeRichText();

            _mapper.Map(request.Dto, existingComment);

            existingComment.Content = cleanContent;

            await _commentRepository.UpdateAsync(existingComment);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.CommentUpdated);

            return new Result(ResultStatus.Error, Messages.CommentUpdateError);
        }
    }
}
