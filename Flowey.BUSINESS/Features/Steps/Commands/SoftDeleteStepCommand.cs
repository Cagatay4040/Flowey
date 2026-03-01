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

        public SoftDeleteStepCommand(Guid stepId)
        {
            StepId = stepId;
        }
    }

    public class SoftDeleteStepCommandHandler : IRequestHandler<SoftDeleteStepCommand, IResult>
    {
        private readonly IStepRepository _stepRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SoftDeleteStepCommandHandler(
            IStepRepository stepRepository, 
            IUnitOfWork unitOfWork)
        {
            _stepRepository = stepRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult> Handle(SoftDeleteStepCommand request, CancellationToken cancellationToken)
        {
            var existingStep = await _stepRepository.GetByIdAsync(request.StepId);

            if (existingStep == null)
                return new Result(ResultStatus.Error, Messages.StepNotFound);

            await _stepRepository.SoftDeleteAndReOrderStepsAsync(existingStep);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.StepDeleted);

            return new Result(ResultStatus.Error, Messages.StepDeleteError);
        }
    }
}
