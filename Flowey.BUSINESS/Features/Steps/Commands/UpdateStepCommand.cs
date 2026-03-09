using Flowey.CORE.Constants;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DATACCESS.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.Features.Steps.Commands
{
    public class UpdateStepCommand : IRequest<IResult>
    {
        public Guid StepId { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }

        public UpdateStepCommand(Guid stepId, string name, int order)
        {
            StepId = stepId;    
            Name = name;
            Order = order;
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

            existingStep.Name = request.Name;
            existingStep.Order = request.Order;

            await _stepRepository.UpdateAsync(existingStep);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.StepUpdated);

            return new Result(ResultStatus.Error, Messages.StepUpdateError);
        }
    }
}
