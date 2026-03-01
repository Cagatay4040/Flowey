using Flowey.BUSINESS.Abstract;
using Flowey.BUSINESS.DTO.Notification;
using Flowey.BUSINESS.DTO.Task;
using Flowey.CORE.Constants;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DATACCESS.Abstract;
using Flowey.DOMAIN.Model.Concrete;
using Flowey.CORE.DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.Features.Tasks.Commands
{
    public class ChangeAssignTaskCommand : IRequest<IResult>
    {
        public TaskAssignDTO TaskAssignDTO { get; set; }

        public ChangeAssignTaskCommand(TaskAssignDTO taskAssignDTO)
        {
            TaskAssignDTO = taskAssignDTO;
        }
    }

    public class ChangeAssignTaskCommandHandler : IRequestHandler<ChangeAssignTaskCommand, IResult>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserNotificationService _userNotificationService;
        private readonly IUnitOfWork _unitOfWork;

        public ChangeAssignTaskCommandHandler(
            ITaskRepository taskRepository,
            IUserRepository userRepository,
            ICurrentUserService currentUserService,
            IUserNotificationService userNotificationService,
            IUnitOfWork unitOfWork)
        {
            _taskRepository = taskRepository;
            _userRepository = userRepository;
            _currentUserService = currentUserService;
            _userNotificationService = userNotificationService;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult> Handle(ChangeAssignTaskCommand request, CancellationToken cancellationToken)
        {
            var dto = request.TaskAssignDTO;
            var existingTask = await _taskRepository.GetByIdAsync(dto.TaskId, false, x => x.TaskHistories);

            if (existingTask == null)
                return new Result(ResultStatus.Error, Messages.TaskNotFound);

            existingTask.AssigneeId = dto.UserId;

            existingTask.TaskHistories.Add(new TaskHistory
            {
                TaskId = existingTask.Id,
                UserId = dto.UserId,
                StepId = existingTask.CurrentStepId
            });

            await _taskRepository.UpdateAsync(existingTask);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
            {
                if (dto.UserId.HasValue && dto.UserId != _currentUserService.GetUserId().Value)
                {
                    string taskIdentifier = existingTask.TaskKey != null
                                            ? $"task #{existingTask.TaskKey}"
                                            : "a task";

                    var senderUser = await _userRepository.GetByIdAsync(_currentUserService.GetUserId().Value);
                    string senderName = senderUser != null ? $"{senderUser.Name} {senderUser.Surname}" : "System";

                    await _userNotificationService.AddUserNotificationAsync(new UserNotificationAddDTO
                    {
                        UserId = dto.UserId.Value,
                        SenderId = _currentUserService.GetUserId().Value,
                        Title = Messages.TaskReassignedTitle,
                        Message = string.Format(Messages.TaskReassignedMessage, senderName, taskIdentifier),
                        ActionUrl = $"/board/{existingTask.ProjectId}?taskId={existingTask.Id}"
                    });
                }
                return new Result(ResultStatus.Success, Messages.TaskAssignedSuccessfully);
            }

            return new Result(ResultStatus.Error, Messages.TaskAssignError);
        }
    }
}
