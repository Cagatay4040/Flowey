using Flowey.BUSINESS.Extensions;
using Flowey.CORE.DataAccess.Abstract;
using Flowey.CORE.DTO.Notification;
using Flowey.CORE.Interfaces.Repositories;
using Flowey.CORE.Interfaces.Services;
using Flowey.CORE.Interfaces.UnitOfWork;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DOMAIN.Model.Concrete;
using Flowey.SHARED.Constants;
using MediatR;
using System.Text.RegularExpressions;

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
        private readonly IUserNotificationService _userNotificationService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserRepository _userRepository;

        public AddCommentCommandHandler(
            ICommentRepository commentRepository,
            ITaskRepository taskRepository,
            IUnitOfWork unitOfWork,
            IUserNotificationService userNotificationService,
            ICurrentUserService currentUserService,
            IUserRepository userRepository)
        {
            _commentRepository = commentRepository;
            _taskRepository = taskRepository;
            _unitOfWork = unitOfWork;
            _userNotificationService = userNotificationService;
            _currentUserService = currentUserService;
            _userRepository = userRepository;
        }

        public async Task<IResult> Handle(AddCommentCommand request, CancellationToken cancellationToken)
        {
            var existingTask = await _taskRepository.AnyAsync(x => x.Id == request.TaskId);

            if (!existingTask)
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
                await SendMentionNotificationsAsync(cleanContent, request.TaskId);
                return new Result(ResultStatus.Success, Messages.CommentAdded);
            }

            return new Result(ResultStatus.Error, Messages.CommentCreateError);
        }

        private async System.Threading.Tasks.Task SendMentionNotificationsAsync(string content, Guid taskId)
        {
            if (string.IsNullOrWhiteSpace(content)) return;

            var currentUserId = _currentUserService.GetUserId().Value;
            var senderUser = await _userRepository.GetByIdAsync(currentUserId);
            string senderName = senderUser != null ? $"{senderUser.Name} {senderUser.Surname}" : "System";

            var mentionedUserIds = ExtractMentionedUserIds(content);
            if (!mentionedUserIds.Any()) return;

            var existingTask = await _taskRepository.GetByIdAsync(taskId);
            string taskIdentifier = existingTask?.TaskKey != null ? $"task #{existingTask.TaskKey}" : "a task";

            foreach (var mentionedUserId in mentionedUserIds)
            {
                if (mentionedUserId != currentUserId)
                {
                    await _userNotificationService.AddUserNotificationAsync(new UserNotificationAddDTO
                    {
                        UserId = mentionedUserId,
                        SenderId = currentUserId,
                        Title = Messages.NewMentionTitle,
                        Message = string.Format(Messages.NewMentionMessage, senderName, taskIdentifier),
                        ActionUrl = $"/board/{existingTask?.ProjectId}?taskId={taskId}"
                    });
                }
            }
        }

        private List<Guid> ExtractMentionedUserIds(string htmlContent)
        {
            var userIds = new List<Guid>();
            if (string.IsNullOrWhiteSpace(htmlContent)) return userIds;

            var regex = new Regex(@"data-id=""([a-fA-F0-9\-]{36})""");
            var matches = regex.Matches(htmlContent);

            foreach (Match match in matches)
            {
                if (match.Groups.Count > 1 && Guid.TryParse(match.Groups[1].Value, out Guid parsedId))
                {
                    if (!userIds.Contains(parsedId))
                    {
                        userIds.Add(parsedId);
                    }
                }
            }
            return userIds;
        }
    }
}
