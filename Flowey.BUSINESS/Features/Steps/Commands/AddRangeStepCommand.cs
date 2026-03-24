using AutoMapper;
using Flowey.CORE.DTO.Step;
using Flowey.CORE.Interfaces.Repositories;
using Flowey.CORE.Interfaces.Security;
using Flowey.CORE.Interfaces.UnitOfWork;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DOMAIN.Model.Concrete;
using Flowey.SHARED.Constants;
using Flowey.SHARED.Enums;
using MediatR;

namespace Flowey.BUSINESS.Features.Steps.Commands
{
    public class AddRangeStepCommand : IRequest<IResult>, IRequireProjectAuthorization
    {
        public List<StepAddDTO> StepAddDTOs { get; set; }

        public Guid ProjectId { get; set; }
        public RoleType[] RequiredRoles => new[] { RoleType.Admin, RoleType.Editor };

        public AddRangeStepCommand(List<StepAddDTO> stepAddDTOs)
        {
            if (stepAddDTOs == null) throw new ArgumentNullException(nameof(stepAddDTOs));
            if (!stepAddDTOs.Any()) throw new ArgumentException(Messages.StepListEmpty, nameof(stepAddDTOs));

            var firstProjectId = stepAddDTOs.First().ProjectId;
            if (stepAddDTOs.Any(dto => dto.ProjectId != firstProjectId))
                throw new ArgumentException(Messages.StepsMustBelongToSameProject);

            StepAddDTOs = stepAddDTOs;
            ProjectId = stepAddDTOs.First().ProjectId;
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
            var existingProject = await _projectRepository.AnyAsync(x => x.Id == request.ProjectId);

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
