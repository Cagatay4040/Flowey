using Flowey.CORE.Constants;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DATACCESS.Abstract;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.Features.Steps.Commands
{
    public class SoftDeleteStepCommand : IRequest<IResult>
    {
        public Guid StepId { get; set; }
        public Guid? TargetStepId { get; set; }

        public SoftDeleteStepCommand(Guid stepId, Guid? targetStepId)
        {
            StepId = stepId;
            TargetStepId = targetStepId;
        }
    }

    public class SoftDeleteStepCommandHandler : IRequestHandler<SoftDeleteStepCommand, IResult>
    {
        private readonly IStepRepository _stepRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SoftDeleteStepCommandHandler(
            IStepRepository stepRepository,
            ITaskRepository taskRepository,
            IUnitOfWork unitOfWork)
        {
            _stepRepository = stepRepository;
            _taskRepository = taskRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult> Handle(SoftDeleteStepCommand request, CancellationToken cancellationToken)
        {
            var existingStep = await _stepRepository.GetByIdAsync(request.StepId);

            if (existingStep == null)
                return new Result(ResultStatus.Error, Messages.StepNotFound);

            if (request.TargetStepId.HasValue && request.TargetStepId != Guid.Empty)
            {
                var tasks = await _taskRepository.GetList(t => t.CurrentStepId == existingStep.Id, false);
                foreach (var task in tasks)
                {
                    task.CurrentStepId = request.TargetStepId.Value;
                }
            }

            await _stepRepository.SoftDeleteAndReOrderStepsAsync(existingStep);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.StepDeleted);

            return new Result(ResultStatus.Error, Messages.StepDeleteError);
        }
    }
}
