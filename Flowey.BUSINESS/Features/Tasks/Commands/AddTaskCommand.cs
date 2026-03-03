using AutoMapper;
using Flowey.BUSINESS.Abstract;
using Flowey.BUSINESS.DTO.Task;
using Flowey.BUSINESS.Extensions;
using Flowey.CORE.Constants;
using Flowey.CORE.DataAccess.Abstract;
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
        public TaskAddDTO TaskAddDTO { get; set; }

        public AddTaskCommand(TaskAddDTO taskAddDTO)
        {
            TaskAddDTO = taskAddDTO;
        }
    }

    public class AddTaskCommandHandler : IRequestHandler<AddTaskCommand, IResult>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IStepRepository _stepRepository;
        private readonly IUserNotificationService _userNotificationService;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public AddTaskCommandHandler(
            ITaskRepository taskRepository,
            IProjectRepository projectRepository,
            IStepRepository stepRepository,
            IUserNotificationService userNotificationService,
            IMapper mapper,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
            _stepRepository = stepRepository;
            _userNotificationService = userNotificationService;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult> Handle(AddTaskCommand request, CancellationToken cancellationToken)
        {
            var dto = request.TaskAddDTO;
            var project = await _projectRepository.FirstOrDefaultAsync(x => x.Id == dto.ProjectId);

            if (project == null)
                return new Result(ResultStatus.Error, Messages.ProjectNotFound);

            dto.UserId = _currentUserService.GetUserId().Value;

            int currentCount = await _taskRepository.CountAsync(t => t.ProjectId == dto.ProjectId);
            string newTaskKey = $"{project.ProjectKey}-{currentCount + 1}";

            var task = _mapper.Map<Flowey.DOMAIN.Model.Concrete.Task>(dto);
            task.TaskKey = newTaskKey;

            var firstStep = await _stepRepository.GetProjectFirstStepAsync(dto.ProjectId);
            
            if (firstStep == null)
                return new Result(ResultStatus.Error, Messages.ProjectStepsNotFound);

            task.CurrentStepId = firstStep.Id;
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
            {
                await _userNotificationService.SendMentionNotificationsAsync(task.Description, task.Id, task.ProjectId);
                return new Result(ResultStatus.Success, string.Format(Messages.TaskAdded, newTaskKey));
            }

            return new Result(ResultStatus.Error, Messages.TaskCreateError);
        }
    }
}
