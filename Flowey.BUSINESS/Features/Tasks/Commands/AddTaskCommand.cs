using Flowey.BUSINESS.Abstract;
using Flowey.BUSINESS.DTO.Task;
using Flowey.BUSINESS.Extensions;
using Flowey.CORE.Constants;
using Flowey.CORE.Enums;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DATACCESS.Abstract;
using Flowey.DOMAIN.Model.Concrete;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.Features.Tasks.Commands
{
    public class AddTaskCommand : IRequest<IResult>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public PriorityType Priority { get; set; }
        public DateTime? Deadline { get; set; }
        public Guid ProjectId { get; set; }
        public Guid? UserId { get; set; }
        public List<TaskAddLinkItemDTO> Links { get; set; } = new List<TaskAddLinkItemDTO>();

        public AddTaskCommand(string title, string description, PriorityType priority, DateTime? deadline, Guid projectId, Guid? userId, List<TaskAddLinkItemDTO> links)
        {
            Title = title;
            Description = description;
            Priority = priority;
            Deadline = deadline;
            ProjectId = projectId;
            UserId = userId;
            Links = links;
        }
    }

    public class AddTaskCommandHandler : IRequestHandler<AddTaskCommand, IResult>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IStepRepository _stepRepository;
        private readonly IUserNotificationService _userNotificationService;
        private readonly IUnitOfWork _unitOfWork;

        public AddTaskCommandHandler(
            ITaskRepository taskRepository,
            IProjectRepository projectRepository,
            IStepRepository stepRepository,
            IUserNotificationService userNotificationService,
            IUnitOfWork unitOfWork)
        {
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
            _stepRepository = stepRepository;
            _userNotificationService = userNotificationService;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult> Handle(AddTaskCommand request, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.FirstOrDefaultAsync(x => x.Id == request.ProjectId);

            if (project == null)
                return new Result(ResultStatus.Error, Messages.ProjectNotFound);

            var firstStep = await _stepRepository.GetProjectFirstStepAsync(request.ProjectId);

            if (firstStep == null)
                return new Result(ResultStatus.Error, Messages.ProjectStepsNotFound);

            int currentCount = await _taskRepository.CountAsync(t => t.ProjectId == request.ProjectId, true);
            string newTaskKey = $"{project.ProjectKey}-{currentCount + 1}";

            var task = new DOMAIN.Model.Concrete.Task
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                TaskKey = newTaskKey,
                Priority = request.Priority,
                Deadline = request.Deadline,
                ProjectId = request.ProjectId,
                AssigneeId = request.UserId,
                CurrentStepId = firstStep.Id,
                Description = request.Description.ToSafeRichText()      
            };

            task.TaskHistories.Add(new TaskHistory
            {
                TaskId = task.Id,
                StepId = task.CurrentStepId,
            });

            if (request.Links != null && request.Links.Any())
            {
                foreach (var linkDto in request.Links)
                {
                    task.OutgoingLinks.Add(new TaskLink
                    {
                        TargetTaskId = linkDto.TargetTaskId,
                        LinkType = linkDto.LinkType
                    });
                }
            }

            await _taskRepository.AddAsync(task);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
            {
                await _userNotificationService.SendMentionNotificationsAsync(task.Description, task.Id, task.ProjectId);
                return new Result(ResultStatus.Success, string.Format(Messages.TaskAdded, newTaskKey));
            }

            return new Result(ResultStatus.Error, Messages.TaskCreateError);
        }
    }
}
