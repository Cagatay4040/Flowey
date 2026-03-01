using AutoMapper;
using Flowey.BUSINESS.Abstract;
using Flowey.BUSINESS.DTO.Notification;
using Flowey.BUSINESS.DTO.Task;
using Flowey.BUSINESS.Extensions;
using Flowey.CORE.Constants;
using Flowey.CORE.DataAccess.Abstract;
using Flowey.CORE.Enums;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DATACCESS.Abstract;
using Flowey.DOMAIN.Model.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task = Flowey.DOMAIN.Model.Concrete.Task;

namespace Flowey.BUSINESS.Concrete
{
    public class TaskManager : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IStepRepository _stepRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEntityRepository<TaskHistory> _taskHistoryRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserNotificationService _userNotificationService;
        private readonly IUnitOfWork _unitOfWork;

        public TaskManager(ITaskRepository taskRepository, IProjectRepository projectRepository, IStepRepository stepRepository, IUserRepository userRepository, IEntityRepository<TaskHistory> taskHistoryRepository, IMapper mapper, ICurrentUserService currentUserService, IUserNotificationService userNotificationService, IUnitOfWork unitOfWork)
        {
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
            _stepRepository = stepRepository;
            _userRepository = userRepository;
            _taskHistoryRepository = taskHistoryRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _userNotificationService = userNotificationService;
            _unitOfWork = unitOfWork;
        }

        #region Get Methods

        public async Task<IDataResult<TaskGetDTO>> GetByIdAsync(Guid id)
        {
            var entity = await _taskRepository.FirstOrDefaultAsync(x => x.Id == id);
            var data = _mapper.Map<TaskGetDTO>(entity);
            return new DataResult<TaskGetDTO>(ResultStatus.Success, data);
        }

        public async Task<IDataResult<List<TaskGetDTO>>> GetProjectTasksAsync(Guid projectId)
        {
            var entityList = await _taskRepository.GetList(x => x.ProjectId == projectId);
            var data = _mapper.Map<List<TaskGetDTO>>(entityList);
            return new DataResult<List<TaskGetDTO>>(ResultStatus.Success, data);
        }

        public async Task<IDataResult<List<TaskHistoryGetDTO>>> GetTaskHistoryAsync(Guid taskId)
        {
            var entityList = await _taskHistoryRepository.GetList(
                                x => x.TaskId == taskId, 
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

        #endregion

        #region Insert Methods

        public async Task<IResult> AddTaskAsync(TaskAddDTO dto)
        {
            var project = await _projectRepository.FirstOrDefaultAsync(x => x.Id == dto.ProjectId);

            if (project == null)
                return new Result(ResultStatus.Error, Messages.ProjectNotFound);

            dto.UserId = _currentUserService.GetUserId().Value;

            int currentCount = await _taskRepository.CountAsync(t => t.ProjectId == dto.ProjectId);

            string newTaskKey = $"{project.ProjectKey}-{currentCount + 1}";

            var task = _mapper.Map<Task>(dto);
            task.TaskKey = newTaskKey;

            var firstStep = await _stepRepository.GetProjectFirstStepAsync(dto.ProjectId);

            task.CurrentStepId = firstStep.Id;

            if(firstStep == null)
                return new Result(ResultStatus.Error, Messages.ProjectStepsNotFound);

            task.Description = dto.Description.ToSafeRichText();

            task.TaskHistories = new List<TaskHistory>
            {
                new TaskHistory
                {
                    TaskId = task.Id,
                    StepId = task.CurrentStepId
                }
            };

            await _taskRepository.AddAsync(task);

            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, string.Format(Messages.TaskAdded, newTaskKey));

            return new Result(ResultStatus.Error, Messages.TaskCreateError);
        }

        #endregion

        #region Update Methods

        public async Task<IResult> UpdateAsync(TaskUpdateDTO dto)
        {
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

        public async Task<IResult> ChangeAssignTaskAsync(TaskAssignDTO dto)
        {
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

        public async Task<IResult> ChangeStepTaskAsync(TaskStepDTO dto)
        {
            var existingTask = await _taskRepository.GetByIdAsync(dto.TaskId, false, x => x.TaskHistories);

            if (existingTask == null)
                return new Result(ResultStatus.Error, Messages.TaskNotFound);

            existingTask.CurrentStepId = dto.NewStepId;

            existingTask.TaskHistories.Add(new TaskHistory
            {
                TaskId = existingTask.Id,
                UserId = existingTask.AssigneeId,
                StepId = existingTask.CurrentStepId
            });

            await _taskRepository.UpdateAsync(existingTask);

            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.TaskStepUpdatedSuccessfully);

            return new Result(ResultStatus.Error, Messages.TaskStepUpdateFailed);
        }

        #endregion

        #region Delete Methods

        public async Task<IResult> DeleteAsync(Guid id)
        {
            var existingTask = await _taskRepository.GetByIdAsync(id);

            if (existingTask == null)
                return new Result(ResultStatus.Error, Messages.TaskNotFound);

            await _taskRepository.DeleteAsync(existingTask);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.TaskDeleted);

            return new Result(ResultStatus.Error, Messages.TaskDeleteError);
        }

        public async Task<IResult> SoftDeleteAsync(Guid id)
        {
            var existingProject = await _taskRepository.FirstOrDefaultAsync(x => x.Id == id);

            if (existingProject == null)
                return new Result(ResultStatus.Error, Messages.TaskNotFound);

            await _taskRepository.SoftDeleteAsync(existingProject);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.TaskDeleted);

            return new Result(ResultStatus.Error, Messages.TaskDeleteError);
        }

        #endregion

        #region Helper Methods

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

        #endregion
    }
}
