using AutoMapper;
using Flowey.BUSINESS.Abstract;
using Flowey.BUSINESS.DTO.Notification;
using Flowey.BUSINESS.DTO.Task;
using Flowey.BUSINESS.Extensions;
using Flowey.CORE.Constants;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DATACCESS.Abstract;
using Flowey.DOMAIN.Model.Concrete;
using Flowey.CORE.DataAccess.Abstract;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.Features.Tasks.Commands
{
    public class UpdateTaskCommand : IRequest<IResult>
    {
        public TaskUpdateDTO TaskUpdateDTO { get; set; }

        public UpdateTaskCommand(TaskUpdateDTO taskUpdateDTO)
        {
            TaskUpdateDTO = taskUpdateDTO;
        }
    }

    public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, IResult>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserNotificationService _userNotificationService;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateTaskCommandHandler(
            ITaskRepository taskRepository,
            IMapper mapper,
            ICurrentUserService currentUserService,
            IUserNotificationService userNotificationService,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _userNotificationService = userNotificationService;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
        {
            var dto = request.TaskUpdateDTO;
            var existingTask = await _taskRepository.GetByIdAsync(dto.TaskId, false, x => x.TaskHistories);

            if (existingTask == null)
                return new Result(ResultStatus.Error, Messages.TaskNotFound);

            _mapper.Map(dto, existingTask);

            existingTask.Description = dto.Description.ToSafeRichText();

            existingTask.TaskHistories.Add(new TaskHistory
            {
                TaskId = existingTask.Id,
                UserId = existingTask.AssigneeId,
                StepId = existingTask.CurrentStepId
            });

            await _taskRepository.UpdateAsync(existingTask);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
            {
                await SendMentionNotificationsAsync(existingTask.Description, existingTask.Id, existingTask.ProjectId);
                return new Result(ResultStatus.Success, Messages.TaskUpdated);
            }

            return new Result(ResultStatus.Error, Messages.TaskNotFound);
        }

        private async System.Threading.Tasks.Task SendMentionNotificationsAsync(string content, Guid taskId, Guid projectId)
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
                        ActionUrl = $"/board/{projectId}?taskId={taskId}"
                    });
                }
            }
        }

        private List<Guid> ExtractMentionedUserIds(string htmlContent)
        {
            var userIds = new List<Guid>();
            if (string.IsNullOrWhiteSpace(htmlContent)) return userIds;

            var regex = new System.Text.RegularExpressions.Regex(@"data-id=""([a-fA-F0-9\-]{36})""");
            var matches = regex.Matches(htmlContent);

            foreach (System.Text.RegularExpressions.Match match in matches)
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
