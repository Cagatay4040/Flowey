using Flowey.CORE.DataAccess.Abstract;
using Flowey.CORE.Events.Task;
using Flowey.CORE.Interfaces.Repositories;
using Flowey.CORE.Interfaces.Security;
using Flowey.CORE.Interfaces.UnitOfWork;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DOMAIN.Model.Concrete;
using Flowey.SHARED.Constants;
using Flowey.SHARED.Enums;
using MediatR;

namespace Flowey.BUSINESS.Features.Tasks.Commands
{
    public class ChangeAssignTaskCommand : IRequest<IResult>, IRequireTaskAuthorization
    {
        public Guid TaskId { get; set; }
        public Guid? UserId { get; set; }

        public RoleType[] RequiredRoles => new[] { RoleType.Admin, RoleType.Editor, RoleType.Member };

        public ChangeAssignTaskCommand(Guid taskId, Guid? userId)
        {
            TaskId = taskId;
            UserId = userId;
        }
    }

    public class ChangeAssignTaskCommandHandler : IRequestHandler<ChangeAssignTaskCommand, IResult>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPublisher _publisher;
        private readonly IUnitOfWork _unitOfWork;

        public ChangeAssignTaskCommandHandler(
            ITaskRepository taskRepository,
            IUserRepository userRepository,
            ICurrentUserService currentUserService,
            IPublisher publisher,
            IUnitOfWork unitOfWork)
        {
            _taskRepository = taskRepository;
            _userRepository = userRepository;
            _currentUserService = currentUserService;
            _publisher = publisher;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult> Handle(ChangeAssignTaskCommand request, CancellationToken cancellationToken)
        {
            var existingTask = await _taskRepository.GetByIdAsync(request.TaskId, false, x => x.TaskHistories);

            if (existingTask == null)
                return new Result(ResultStatus.Error, Messages.TaskNotFound);

            existingTask.AssigneeId = request.UserId;

            existingTask.TaskHistories.Add(new TaskHistory
            {
                TaskId = existingTask.Id,
                UserId = request.UserId,
                StepId = existingTask.CurrentStepId
            });

            await _taskRepository.UpdateAsync(existingTask);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
            {
                var assignerId = _currentUserService.GetUserId().Value;

                if (request.UserId.HasValue && request.UserId != assignerId)
                {
                    string taskIdentifier = existingTask.TaskKey != null
                                            ? $"task #{existingTask.TaskKey}"
                                            : "a task";

                    var senderUser = await _userRepository.GetByIdAsync(assignerId);
                    string senderName = senderUser != null ? $"{senderUser.Name} {senderUser.Surname}" : "System";

                    var taskEvent = new TaskAssigneeChangedEvent(
                                        existingTask.Id,
                                        request.UserId,
                                        assignerId,
                                        existingTask.ProjectId,
                                        existingTask.TaskKey);

                    await _publisher.Publish(taskEvent, cancellationToken);
                }

                return new Result(ResultStatus.Success, Messages.TaskAssignedSuccessfully);
            }

            return new Result(ResultStatus.Error, Messages.TaskAssignError);
        }
    }
}
