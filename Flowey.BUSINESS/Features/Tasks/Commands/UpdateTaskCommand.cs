using AutoMapper;
using Flowey.BUSINESS.Abstract;
using Flowey.BUSINESS.DTO.Task;
using Flowey.BUSINESS.Extensions;
using Flowey.CORE.Constants;
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
        public TaskUpdateDTO TaskUpdateDTO { get; set; }

        public UpdateTaskCommand(TaskUpdateDTO taskUpdateDTO)
        {
            TaskUpdateDTO = taskUpdateDTO;
        }
    }

    public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, IResult>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IMapper _mapper;
        private readonly IUserNotificationService _userNotificationService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateTaskCommandHandler(
            ITaskRepository taskRepository,
            IMapper mapper,
            IUserNotificationService userNotificationService,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
            _userNotificationService = userNotificationService;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
        {
            var dto = request.TaskUpdateDTO;
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
                await _userNotificationService.SendMentionNotificationsAsync(existingTask.Description, existingTask.Id, existingTask.ProjectId);
                return new Result(ResultStatus.Success, Messages.TaskUpdated);
            }

            return new Result(ResultStatus.Error, Messages.TaskNotFound);
        } 
    }
}
