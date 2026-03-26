using Flowey.BUSINESS.Extensions;
using Flowey.BUSINESS.Features.Comments.Events;
using Flowey.CORE.DataAccess.Abstract;
using Flowey.CORE.Interfaces.Repositories;
using Flowey.CORE.Interfaces.Security;
using Flowey.CORE.Interfaces.UnitOfWork;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DOMAIN.Model.Concrete;
using Flowey.SHARED.Constants;
using Flowey.SHARED.Enums;
using MediatR;

namespace Flowey.BUSINESS.Features.Comments.Commands
{
    public class AddCommentCommand : IRequest<IResult>, IRequireTaskAuthorization
    {
        public string Content { get; set; }
        public Guid TaskId { get; set; }

        public RoleType[] RequiredRoles => new[] { RoleType.Admin, RoleType.Editor, RoleType.Member };

        public AddCommentCommand(string content, Guid taskId)
        {
            Content = content;
            TaskId = taskId;
        }
    }

    public class AddCommentCommandHandler : IRequestHandler<AddCommentCommand, IResult>
    {
        private readonly ICommentRepository _commentRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPublisher _publisher;

        public AddCommentCommandHandler(
            ICommentRepository commentRepository,
            ITaskRepository taskRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork,
            IPublisher publisher)
        {
            _commentRepository = commentRepository;
            _taskRepository = taskRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _publisher = publisher;
        }

        public async Task<IResult> Handle(AddCommentCommand request, CancellationToken cancellationToken)
        {
            var existingTask = await _taskRepository.GetByIdAsync(request.TaskId);

            if (existingTask == null)
                return new Result(ResultStatus.Error, Messages.TaskNotFound);

            var currentUserId = _currentUserService.GetUserId().Value;

            var cleanContent = request.Content.ToSafeRichText();

            var comment = new Comment
            {
                Content = cleanContent,
                TaskId = request.TaskId,
                UserId = currentUserId
            };

            await _commentRepository.AddAsync(comment);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
            {
                var commentEvent = new CommentAddedEvent(
                                        cleanContent, 
                                        request.TaskId,
                                        currentUserId,
                                        existingTask.TaskKey,
                                        existingTask.ProjectId);

                await _publisher.Publish(commentEvent, cancellationToken);
                return new Result(ResultStatus.Success, Messages.CommentAdded);
            }

            return new Result(ResultStatus.Error, Messages.CommentCreateError);
        }
    }
}
