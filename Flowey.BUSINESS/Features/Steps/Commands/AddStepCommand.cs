using AutoMapper;
using Flowey.BUSINESS.DTO.Step;
using Flowey.CORE.Constants;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DATACCESS.Abstract;
using Flowey.DOMAIN.Model.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.Features.Steps.Commands
{
    public class AddStepCommand : IRequest<IResult>
    {
        public StepAddDTO StepAddDTO { get; set; }

        public AddStepCommand(StepAddDTO stepAddDTO)
        {
            StepAddDTO = stepAddDTO;
        }
    }

    public class AddStepCommandHandler : IRequestHandler<AddStepCommand, IResult>
    {
        private readonly IStepRepository _stepRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public AddStepCommandHandler(
            IStepRepository stepRepository, 
            IProjectRepository projectRepository, 
            IMapper mapper, 
            IUnitOfWork unitOfWork)
        {
            _stepRepository = stepRepository;
            _projectRepository = projectRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult> Handle(AddStepCommand request, CancellationToken cancellationToken)
        {
            var existingProject = await _projectRepository.AnyAsync(x => x.Id == request.StepAddDTO.ProjectId);

            if (!existingProject)
                return new Result(ResultStatus.Error, Messages.ProjectNotFound);

            var step = _mapper.Map<Step>(request.StepAddDTO);

            await _stepRepository.AddAsync(step);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.StepAdded);

            return new Result(ResultStatus.Error, Messages.StepCreateError);
        }
    }
}
