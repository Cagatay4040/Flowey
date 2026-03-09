using Flowey.CORE.Constants;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DATACCESS.Abstract;
using Flowey.DOMAIN.Model.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.Features.Tasks.Commands
{
    public class ChangeStepTaskCommand : IRequest<IResult>
    {
        public Guid TaskId { get; set; }
        public Guid NewStepId { get; set; }

        public ChangeStepTaskCommand(Guid taskId, Guid newStepId)
        {
            TaskId = taskId;
            NewStepId = newStepId;
        }
    }

    public class ChangeStepTaskCommandHandler : IRequestHandler<ChangeStepTaskCommand, IResult>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ChangeStepTaskCommandHandler(ITaskRepository taskRepository, IUnitOfWork unitOfWork)
        {
            _taskRepository = taskRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult> Handle(ChangeStepTaskCommand request, CancellationToken cancellationToken)
        {
            var existingTask = await _taskRepository.GetByIdAsync(request.TaskId, false, x => x.TaskHistories);

            if (existingTask == null)
                return new Result(ResultStatus.Error, Messages.TaskNotFound);

            existingTask.CurrentStepId = request.NewStepId;

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
    }
}
