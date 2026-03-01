using AutoMapper;
using Flowey.BUSINESS.DTO.Step;
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
        public StepUpdateDTO StepUpdateDTO { get; set; }

        public UpdateStepCommand(StepUpdateDTO stepUpdateDTO)
        {
            StepUpdateDTO = stepUpdateDTO;
        }
    }

    public class UpdateStepCommandHandler : IRequestHandler<UpdateStepCommand, IResult>
    {
        private readonly IStepRepository _stepRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateStepCommandHandler(
            IStepRepository stepRepository, 
            IMapper mapper, 
            IUnitOfWork unitOfWork)
        {
            _stepRepository = stepRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult> Handle(UpdateStepCommand request, CancellationToken cancellationToken)
        {
            var existingStep = await _stepRepository.GetByIdAsync(request.StepUpdateDTO.StepId);

            if (existingStep == null)
                return new Result(ResultStatus.Error, Messages.StepNotFound);

            _mapper.Map(request.StepUpdateDTO, existingStep);

            await _stepRepository.UpdateAsync(existingStep);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.StepUpdated);

            return new Result(ResultStatus.Error, Messages.StepUpdateError);
        }
    }
}
