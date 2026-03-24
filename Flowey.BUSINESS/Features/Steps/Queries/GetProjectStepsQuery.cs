using AutoMapper;
using Flowey.CORE.DTO.Step;
using Flowey.CORE.Interfaces.Repositories;
using Flowey.CORE.Interfaces.Security;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.SHARED.Constants;
using Flowey.SHARED.Enums;
using MediatR;

namespace Flowey.BUSINESS.Features.Steps.Queries
{
    public class GetProjectStepsQuery : IRequest<IDataResult<List<StepGetDTO>>>, IRequireProjectAuthorization
    {
        public Guid ProjectId { get; set; }

        public RoleType[] RequiredRoles => new[] { RoleType.Admin, RoleType.Editor, RoleType.Member };

        public GetProjectStepsQuery(Guid projectId)
        {
            ProjectId = projectId;
        }
    }

    public class GetProjectStepsQueryHandler : IRequestHandler<GetProjectStepsQuery, IDataResult<List<StepGetDTO>>>
    {
        private readonly IStepRepository _stepRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IMapper _mapper;

        public GetProjectStepsQueryHandler(
            IStepRepository stepRepository, 
            IProjectRepository projectRepository, 
            IMapper mapper)
        {
            _stepRepository = stepRepository;
            _projectRepository = projectRepository;
            _mapper = mapper;
        }

        public async Task<IDataResult<List<StepGetDTO>>> Handle(GetProjectStepsQuery request, CancellationToken cancellationToken)
        {
            var existingProject = await _projectRepository.AnyAsync(x => x.Id == request.ProjectId);

            if (!existingProject)
                return new DataResult<List<StepGetDTO>>(ResultStatus.Error, Messages.ProjectNotFound, new List<StepGetDTO>());

            var entityList = await _stepRepository.GetProjectStepsAsync(request.ProjectId);
            var data = _mapper.Map<List<StepGetDTO>>(entityList);
            return new DataResult<List<StepGetDTO>>(ResultStatus.Success, data);
        }
    }
}
