using AutoMapper;
using Flowey.BUSINESS.DTO.Step;
using Flowey.CORE.Constants;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DATACCESS.Abstract;
using Flowey.DOMAIN.Model.Concrete;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.Features.Steps.Commands
{
    public class AddRangeStepCommand : IRequest<IResult>
    {
        public List<StepAddDTO> StepAddDTOs { get; set; }

        public AddRangeStepCommand(List<StepAddDTO> stepAddDTOs)
        {
            StepAddDTOs = stepAddDTOs;
        }
    }

    public class AddRangeStepCommandHandler : IRequestHandler<AddRangeStepCommand, IResult>
    {
        private readonly IStepRepository _stepRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public AddRangeStepCommandHandler(
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

        public async Task<IResult> Handle(AddRangeStepCommand request, CancellationToken cancellationToken)
        {
            if (request.StepAddDTOs == null || !request.StepAddDTOs.Any())
                return new Result(ResultStatus.Error, Messages.StepListEmpty);

            var firstProjectId = request.StepAddDTOs.First().ProjectId;

            if (request.StepAddDTOs.Any(x => x.ProjectId != firstProjectId))
            {
                return new Result(ResultStatus.Error, Messages.BulkStepProjectMismatch);
            }

            var existingProject = await _projectRepository.AnyAsync(x => x.Id == firstProjectId);

            if (!existingProject)
                return new Result(ResultStatus.Error, Messages.ProjectNotFound);

            var steps = _mapper.Map<List<Step>>(request.StepAddDTOs);

            await _stepRepository.AddRangeAsync(steps);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
            {
                string successMessage = string.Format(Messages.StepsCreatedSuccess, steps.Count);
                return new Result(ResultStatus.Success, successMessage);
            }

            return new Result(ResultStatus.Error, Messages.StepsCreateFailed);
        }
    }
}
