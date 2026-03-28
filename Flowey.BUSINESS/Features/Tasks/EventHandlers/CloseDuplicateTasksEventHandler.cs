using Flowey.CORE.Events.Task;
using Flowey.CORE.Interfaces.Repositories;
using Flowey.CORE.Interfaces.UnitOfWork;
using Flowey.DOMAIN.Model.Concrete;
using Flowey.SHARED.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowey.BUSINESS.Features.Tasks.EventHandlers
{
    public class CloseDuplicateTasksEventHandler : INotificationHandler<TaskCompletedEvent>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ITaskLinkRepository _taskLinkRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CloseDuplicateTasksEventHandler(
            ITaskRepository taskRepository,
            ITaskLinkRepository taskLinkRepository,
            IUnitOfWork unitOfWork)
        {
            _taskRepository = taskRepository;
            _taskLinkRepository = taskLinkRepository;
            _unitOfWork = unitOfWork;
        }

        async System.Threading.Tasks.Task INotificationHandler<TaskCompletedEvent>.Handle(TaskCompletedEvent notification, CancellationToken cancellationToken)
        {
            var duplicateLinks = await _taskLinkRepository.GetList(link =>
                (link.SourceTaskId == notification.CompletedTaskId || link.TargetTaskId == notification.CompletedTaskId) &&
                link.LinkType == LinkType.Duplicates);

            if (!duplicateLinks.Any()) return;

            var duplicateTaskIds = duplicateLinks.Select(link =>
                link.SourceTaskId == notification.CompletedTaskId ? link.TargetTaskId : link.SourceTaskId)
                .Distinct()
                .ToList();

            var duplicateTasks = await _taskRepository.GetQueryable(
                t => duplicateTaskIds.Contains(t.Id) && t.CurrentStepId != notification.DoneStepId,
                false)
                .Include(x => x.TaskHistories)
                .ToListAsync();

            foreach (var task in duplicateTasks)
            {
                task.CurrentStepId = notification.DoneStepId;

                task.TaskHistories.Add(new TaskHistory
                {
                    TaskId = task.Id,
                    UserId = notification.UserId,
                    StepId = notification.DoneStepId
                });
            }

            if (duplicateTasks.Any())
            {
                await _taskRepository.UpdateRangeAsync(duplicateTasks);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
