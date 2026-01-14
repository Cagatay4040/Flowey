using Flowey.BUSINESS.Abstract;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DATACCESS.Abstract;
using System;
using Task = Flowey.DOMAIN.Model.Concrete.Task;
using System.Collections.Generic;

using System.Linq;
using Flowey.BUSINESS.DTO.Task;
using AutoMapper;
using Flowey.CORE.DataAccess.Abstract;
using Flowey.BUSINESS.Constants;
using Flowey.CORE.Enums;

namespace Flowey.BUSINESS.Concrete
{
    public class TaskManager : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IStepRepository _stepRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public TaskManager(ITaskRepository taskRepository, IProjectRepository projectRepository, IStepRepository stepRepository, IUserRepository userRepository, IMapper mapper, ICurrentUserService currentUserService)
        {
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
            _stepRepository = stepRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
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
            var entityList = await _taskRepository.GetTaskHistoryAsync(taskId);

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

            return new DataResult<List<TaskHistoryGetDTO>>(ResultStatus.Success, historyData);
        }

        #endregion

        #region Insert Methods

        public async Task<IResult> AddAndAssignTaskAsync(TaskAddDTO dto)
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

            int effectedRow = await _taskRepository.AddAndAssignTaskAsync(task, dto.UserId);

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, string.Format(Messages.TaskAdded, newTaskKey));

            return new Result(ResultStatus.Error, Messages.TaskCreateError);
        }

        #endregion

        #region Update Methods

        public async Task<IResult> UpdateAsync(TaskUpdateDTO dto)
        {
            var existingTask = await _taskRepository.GetByIdAsync(dto.Id);

            if (existingTask == null)
                return new Result(ResultStatus.Error, Messages.TaskNotFound);

            _mapper.Map(dto, existingTask);

            int effectedRow = await _taskRepository.UpdateAsync(existingTask);

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.TaskUpdated);

            return new Result(ResultStatus.Error, Messages.TaskNotFound);
        }

        public async Task<IResult> ChangeAssignTaskAsync(TaskAssignDTO dto)
        {
            var existingTask = await _taskRepository.GetByIdAsync(dto.TaskId);

            if (existingTask == null)
                return new Result(ResultStatus.Error, Messages.TaskNotFound);

            int effectedRow = await _taskRepository.ChangeAssignTaskAsync(existingTask, dto.UserId);

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.TaskAssignedSuccessfully);

            return new Result(ResultStatus.Error, Messages.TaskAssignError);
        }

        public async Task<IResult> ChangeStepTaskAsync(TaskStepDTO dto)
        {
            var existingTask = await _taskRepository.GetByIdAsync(dto.TaskId);

            if (existingTask == null)
                return new Result(ResultStatus.Error, Messages.TaskNotFound);

            int effectedRow = await _taskRepository.ChangeStepTaskAsync(existingTask, dto.NewStepId);

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

            int effectedRow = await _taskRepository.DeleteAsync(existingTask);

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.TaskDeleted);

            return new Result(ResultStatus.Error, Messages.TaskDeleteError);
        }

        public async Task<IResult> SoftDeleteAsync(Guid id)
        {
            var existingProject = await _taskRepository.FirstOrDefaultAsync(x => x.Id == id);

            if (existingProject == null)
                return new Result(ResultStatus.Error, Messages.TaskNotFound);

            int effectedRow = await _taskRepository.SoftDeleteAsync(existingProject);

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.TaskDeleted);

            return new Result(ResultStatus.Error, Messages.TaskDeleteError);
        }

        #endregion
    }
}
