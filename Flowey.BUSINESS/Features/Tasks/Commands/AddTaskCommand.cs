using Flowey.BUSINESS.Extensions;
using Flowey.CORE.DataAccess.Abstract;
using Flowey.CORE.DTO.Task;
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
    public class AddTaskCommand : IRequest<IResult>, IRequireProjectAuthorization
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public PriorityType Priority { get; set; }
        public DateTime? Deadline { get; set; }
        public Guid ProjectId { get; set; }
        public Guid? UserId { get; set; }
        public List<TaskAddLinkItemDTO> Links { get; set; } = new List<TaskAddLinkItemDTO>();

        public RoleType[] RequiredRoles => new[] { RoleType.Admin, RoleType.Editor, RoleType.Member };

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
        private readonly IPublisher _publisher;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public AddTaskCommandHandler(
            ITaskRepository taskRepository,
            IProjectRepository projectRepository,
            IStepRepository stepRepository,
            IPublisher publisher,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
            _stepRepository = stepRepository;
            _publisher = publisher;
            _currentUserService = currentUserService;
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
                var taskAddedEvent = new TaskAddedEvent(
                                        task.Description,
                                        task.Id, 
                                        _currentUserService.GetUserId().Value, 
                                        task.TaskKey, 
                                        task.ProjectId);

                await _publisher.Publish(taskAddedEvent, cancellationToken);
                return new Result(ResultStatus.Success, string.Format(Messages.TaskAdded, newTaskKey));
            }

            return new Result(ResultStatus.Error, Messages.TaskCreateError);
        }
    }
}
