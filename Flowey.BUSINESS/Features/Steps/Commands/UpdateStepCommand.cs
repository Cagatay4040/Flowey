using Flowey.CORE.Interfaces.Repositories;
using Flowey.CORE.Interfaces.Security;
using Flowey.CORE.Interfaces.UnitOfWork;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.SHARED.Constants;
using Flowey.SHARED.Enums;
using MediatR;

namespace Flowey.BUSINESS.Features.Steps.Commands
{
    public class UpdateStepCommand : IRequest<IResult>, IRequireStepAuthorization
    {
        public Guid StepId { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public StepCategory Category { get; set; }

        public RoleType[] RequiredRoles => new[] { RoleType.Admin, RoleType.Editor };

        public UpdateStepCommand(Guid stepId, string name, int order, StepCategory category)
        {
            StepId = stepId;    
            Name = name;
            Order = order;
            Category = category;
        }
    }

    public class UpdateStepCommandHandler : IRequestHandler<UpdateStepCommand, IResult>
    {
        private readonly IStepRepository _stepRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateStepCommandHandler(
            IStepRepository stepRepository,
            IUnitOfWork unitOfWork)
        {
            _stepRepository = stepRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult> Handle(UpdateStepCommand request, CancellationToken cancellationToken)
        {
            var existingStep = await _stepRepository.GetByIdAsync(request.StepId);

            if (existingStep == null)
                return new Result(ResultStatus.Error, Messages.StepNotFound);

            if (existingStep.Category != request.Category)
            {
                if (existingStep.Category == StepCategory.ToDo)
                {
                    var isLastToDo = await _stepRepository.CountAsync(s => s.ProjectId == existingStep.ProjectId && s.Category == StepCategory.ToDo) == 1;
                    if (isLastToDo)
                        return new Result(ResultStatus.Error, Messages.CannotDeleteLastRequiredCategoryStep);
                }

                if (existingStep.Category == StepCategory.Done)
                {
                    var isLastDone = await _stepRepository.CountAsync(s => s.ProjectId == existingStep.ProjectId && s.Category == StepCategory.Done) == 1;
                    if (isLastDone)
                        return new Result(ResultStatus.Error, Messages.CannotDeleteLastRequiredCategoryStep);
                }
            }

            existingStep.Name = request.Name;
            existingStep.Order = request.Order;
            existingStep.Category = request.Category;

            await _stepRepository.UpdateAsync(existingStep);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.StepUpdated);

            return new Result(ResultStatus.Error, Messages.StepUpdateError);
        }
    }
}
