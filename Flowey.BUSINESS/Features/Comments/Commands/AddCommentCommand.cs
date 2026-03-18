using Flowey.BUSINESS.Extensions;
using Flowey.BUSINESS.Features.Comments.Events;
using Flowey.CORE.Interfaces.Repositories;
using Flowey.CORE.Interfaces.UnitOfWork;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DOMAIN.Model.Concrete;
using Flowey.SHARED.Constants;
using MediatR;

namespace Flowey.BUSINESS.Features.Comments.Commands
{
    public class AddCommentCommand : IRequest<IResult>
    {
        public string Content { get; set; }
        public Guid TaskId { get; set; }
        public Guid UserId { get; set; }

        public AddCommentCommand(string content, Guid taskId, Guid userId)
        {
            Content = content;
            TaskId = taskId;
            UserId = userId;
        }
    }

    public class AddCommentCommandHandler : IRequestHandler<AddCommentCommand, IResult>
    {
        private readonly ICommentRepository _commentRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPublisher _publisher;

        public AddCommentCommandHandler(
            ICommentRepository commentRepository,
            ITaskRepository taskRepository,
            IUnitOfWork unitOfWork,
            IPublisher publisher)
        {
            _commentRepository = commentRepository;
            _taskRepository = taskRepository;
            _unitOfWork = unitOfWork;
            _publisher = publisher;
        }

        public async Task<IResult> Handle(AddCommentCommand request, CancellationToken cancellationToken)
        {
            var existingTask = await _taskRepository.GetByIdAsync(request.TaskId);

            if (existingTask == null)
                return new Result(ResultStatus.Error, Messages.TaskNotFound);

            var cleanContent = request.Content.ToSafeRichText();

            var comment = new Comment
            {
                Content = cleanContent,
                TaskId = request.TaskId,
                UserId = request.UserId
            };

            await _commentRepository.AddAsync(comment);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
            {
                var commentEvent = new CommentAddedEvent(
                                        cleanContent, 
                                        request.TaskId, 
                                        request.UserId,
                                        existingTask.TaskKey,
                                        existingTask.ProjectId);

                await _publisher.Publish(commentEvent, cancellationToken);
                return new Result(ResultStatus.Success, Messages.CommentAdded);
            }

            return new Result(ResultStatus.Error, Messages.CommentCreateError);
        }
    }
}
