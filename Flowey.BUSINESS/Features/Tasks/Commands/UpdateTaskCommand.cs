using Flowey.BUSINESS.Extensions;
using Flowey.BUSINESS.Features.Tasks.Events;
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

namespace Flowey.BUSINESS.Features.Tasks.Commands
{
    public class UpdateTaskCommand : IRequest<IResult>, IRequireTaskAuthorization
    {
        public Guid TaskId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public PriorityType Priority { get; set; }
        public DateTime? Deadline { get; set; }

        public RoleType[] RequiredRoles => new[] { RoleType.Admin, RoleType.Editor, RoleType.Member };

        public UpdateTaskCommand(Guid taskId, string title, string description, PriorityType priority, DateTime? deadline)
        {
            TaskId = taskId;
            Title = title;
            Description = description;
            Priority = priority;
            Deadline = deadline;
        }
    }

    public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, IResult>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IPublisher _publisher;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateTaskCommandHandler(ITaskRepository taskRepository, IPublisher publisher, ICurrentUserService currentUserService, IUnitOfWork unitOfWork)
        {
            _taskRepository = taskRepository;
            _publisher = publisher;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
        {
            var existingTask = await _taskRepository.GetByIdAsync(request.TaskId, false, x => x.TaskHistories);

            if (existingTask == null)
                return new Result(ResultStatus.Error, Messages.TaskNotFound);
  
            existingTask.Title = request.Title;
            existingTask.Description = request.Description.ToSafeRichText();
            existingTask.Priority = request.Priority;
            existingTask.Deadline = request.Deadline;   

            existingTask.TaskHistories.Add(new TaskHistory
            {
                TaskId = existingTask.Id,
                UserId = _currentUserService.GetUserId().Value,
                StepId = existingTask.CurrentStepId
            });

            await _taskRepository.UpdateAsync(existingTask);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
            {
                var taskUpdatedEvent = new TaskUpdatedEvent(
                                            existingTask.Description, 
                                            existingTask.Id, 
                                            _currentUserService.GetUserId().Value, 
                                            existingTask.TaskKey, 
                                            existingTask.ProjectId);

                await _publisher.Publish(taskUpdatedEvent, cancellationToken);
                return new Result(ResultStatus.Success, Messages.TaskUpdated);
            }

            return new Result(ResultStatus.Error, Messages.TaskUpdateError);
        } 
    }
}
