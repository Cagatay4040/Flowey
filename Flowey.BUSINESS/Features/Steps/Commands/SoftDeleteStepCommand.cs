using Flowey.CORE.Interfaces.Repositories;
using Flowey.CORE.Interfaces.UnitOfWork;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.SHARED.Constants;
using Flowey.SHARED.Enums;
using MediatR;

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

            var projectSteps = await _stepRepository.GetList(x => x.ProjectId == existingStep.ProjectId);

            var isLastToDoStep = projectSteps.Count(s => s.Category == StepCategory.ToDo) == 1;
            if (existingStep.Category == StepCategory.ToDo && isLastToDoStep)
                return new Result(ResultStatus.Error, Messages.CannotDeleteLastRequiredCategoryStep);

            var isLastDoneStep = projectSteps.Count(s => s.Category == StepCategory.Done) == 1;
            if (existingStep.Category == StepCategory.Done && isLastDoneStep)
                return new Result(ResultStatus.Error, Messages.CannotDeleteLastRequiredCategoryStep);

            var tasksInStep = await _taskRepository.GetList(t => t.CurrentStepId == existingStep.Id, noTracking: false);

            if (tasksInStep.Any() && (!request.TargetStepId.HasValue || request.TargetStepId == Guid.Empty))
                return new Result(ResultStatus.Error, Messages.MustSelectTargetStep);

            if (request.TargetStepId.HasValue && request.TargetStepId != Guid.Empty && tasksInStep.Any())
            {
                foreach (var task in tasksInStep)
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
