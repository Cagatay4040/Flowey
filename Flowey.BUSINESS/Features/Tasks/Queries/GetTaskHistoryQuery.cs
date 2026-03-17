using Flowey.CORE.DTO.Task;
using Flowey.CORE.Interfaces.Repositories;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DOMAIN.Model.Concrete;
using Flowey.SHARED.Constants;
using Flowey.SHARED.Enums;
using MediatR;

namespace Flowey.BUSINESS.Features.Tasks.Queries
{
    public class GetTaskHistoryQuery : IRequest<IDataResult<List<TaskHistoryGetDTO>>>
    {
        public Guid TaskId { get; set; }

        public GetTaskHistoryQuery(Guid taskId)
        {
            TaskId = taskId;
        }
    }

    public class GetTaskHistoryQueryHandler : IRequestHandler<GetTaskHistoryQuery, IDataResult<List<TaskHistoryGetDTO>>>
    {
        private readonly IEntityRepository<TaskHistory> _taskHistoryRepository;
        private readonly IUserRepository _userRepository;

        public GetTaskHistoryQueryHandler(IEntityRepository<TaskHistory> taskHistoryRepository, IUserRepository userRepository)
        {
            _taskHistoryRepository = taskHistoryRepository;
            _userRepository = userRepository;
        }

        public async Task<IDataResult<List<TaskHistoryGetDTO>>> Handle(GetTaskHistoryQuery request, CancellationToken cancellationToken)
        {
            var entityList = await _taskHistoryRepository.GetList(
                                x => x.TaskId == request.TaskId,
                                true,
                                query => query.OrderBy(x => x.CreatedDate),
                                x => x.Task,
                                x => x.Step,
                                x => x.User);

            var historyData = new List<TaskHistoryGetDTO>();

            var actorIds = entityList.Select(x => x.CreatedBy).Distinct().ToList();
            var actors = await _userRepository.GetUsersByIdListAsync(actorIds);
            var actorDictionary = actors.ToDictionary(k => k.Id, v => $"{v.Name} {v.Surname}");

            for (int i = 0; i < entityList.Count; i++)
            {
                var currentEntity = entityList[i];

                string actorName = actorDictionary.ContainsKey(currentEntity.CreatedBy)
                                        ? actorDictionary[currentEntity.CreatedBy]
                                        : "Unknown User";

                var changeDate = currentEntity.CreatedDate;

                // --- CREATED ---
                if (i == 0)
                {
                    historyData.Add(new TaskHistoryGetDTO
                    {
                        CreatedDate = changeDate,
                        CreatedByUserName = actorName,
                        ChangeType = HistoryChangeType.Created,
                        PropertyName = "Task",
                        DisplayMessage = TaskHistoryMessages.TaskCreated
                    });
                    continue;
                }

                var previousEntity = entityList[i - 1];

                // --- STEP CHANGED ---
                if (previousEntity.StepId != currentEntity.StepId)
                {
                    historyData.Add(new TaskHistoryGetDTO
                    {
                        CreatedDate = changeDate,
                        CreatedByUserName = actorName,
                        ChangeType = HistoryChangeType.StepChanged,
                        PropertyName = "Step",
                        OldValue = previousEntity.Step?.Name,
                        NewValue = currentEntity.Step?.Name,
                        DisplayMessage = string.Format(TaskHistoryMessages.StepChanged, previousEntity.Step?.Name, currentEntity.Step?.Name)
                    });
                }

                // --- ASSIGNEE CHANGED ---
                if (previousEntity.UserId != currentEntity.UserId)
                {
                    var oldUserName = previousEntity.User != null ? $"{previousEntity.User.Name} {previousEntity.User.Surname}" : "Unassigned";
                    var newUserName = currentEntity.User != null ? $"{currentEntity.User.Name} {currentEntity.User.Surname}" : "Unassigned";

                    historyData.Add(new TaskHistoryGetDTO
                    {
                        CreatedDate = changeDate,
                        CreatedByUserName = actorName,
                        ChangeType = HistoryChangeType.AssigneeChanged,
                        PropertyName = "Assignee",
                        OldValue = oldUserName,
                        NewValue = newUserName,
                        DisplayMessage = string.Format(TaskHistoryMessages.AssigneeChanged, oldUserName, newUserName)
                    });
                }

                // --- UPDATED ---
                if (previousEntity.StepId == currentEntity.StepId && previousEntity.UserId == currentEntity.UserId)
                {
                    historyData.Add(new TaskHistoryGetDTO
                    {
                        CreatedDate = changeDate,
                        CreatedByUserName = actorName,
                        ChangeType = HistoryChangeType.Updated,
                        PropertyName = "Details",
                        DisplayMessage = TaskHistoryMessages.TaskUpdated
                    });
                }
            }

            historyData = historyData.OrderByDescending(x => x.CreatedDate).ToList();

            return new DataResult<List<TaskHistoryGetDTO>>(ResultStatus.Success, historyData);
        }
    }
}
