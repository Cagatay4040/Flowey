using Flowey.BUSINESS.Abstract;
using Flowey.BUSINESS.Extensions;
using Flowey.CORE.Constants;
using Flowey.CORE.Enums;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DATACCESS.Abstract;
using Flowey.DOMAIN.Model.Concrete;
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
        public Guid TaskId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public PriorityType Priority { get; set; }
        public DateTime? Deadline { get; set; }

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
        private readonly IUserNotificationService _userNotificationService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateTaskCommandHandler(
            ITaskRepository taskRepository,
            IUserNotificationService userNotificationService,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _taskRepository = taskRepository;
            _userNotificationService = userNotificationService;
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
                UserId = existingTask.AssigneeId,
                StepId = existingTask.CurrentStepId
            });

            await _taskRepository.UpdateAsync(existingTask);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
            {
                await _userNotificationService.SendMentionNotificationsAsync(existingTask.Description, existingTask.Id, existingTask.ProjectId);
                return new Result(ResultStatus.Success, Messages.TaskUpdated);
            }

            return new Result(ResultStatus.Error, Messages.TaskNotFound);
        } 
    }
}
