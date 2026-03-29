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
    public class GetBoardDataQuery : IRequest<IDataResult<List<StepGetDTO>>>, IRequireProjectAuthorization
    {
        public Guid ProjectId { get; set; }
        public List<Guid> UserIds { get; set; }
        public bool IncludeUnassigned { get; set; }
        public List<PriorityType>? Priorities { get; set; }

        public RoleType[] RequiredRoles => new[] { RoleType.Admin, RoleType.Editor, RoleType.Member };

        public GetBoardDataQuery(Guid projectId, List<Guid> userIds, bool includeUnassigned, List<PriorityType>? priorities)
        {
            ProjectId = projectId;
            UserIds = userIds;
            IncludeUnassigned = includeUnassigned;
            Priorities = priorities;
        }
    }

    public class GetBoardDataQueryHandler : IRequestHandler<GetBoardDataQuery, IDataResult<List<StepGetDTO>>>
    {
        private readonly IStepRepository _stepRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IMapper _mapper;

        public GetBoardDataQueryHandler(
            IStepRepository stepRepository, 
            IProjectRepository projectRepository, 
            IMapper mapper)
        {
            _stepRepository = stepRepository;
            _projectRepository = projectRepository;
            _mapper = mapper;
        }

        public async Task<IDataResult<List<StepGetDTO>>> Handle(GetBoardDataQuery request, CancellationToken cancellationToken)
        {
            var existingProject = await _projectRepository.AnyAsync(x => x.Id == request.ProjectId);

            if (!existingProject)
                return new DataResult<List<StepGetDTO>>(ResultStatus.Error, new List<StepGetDTO>(), Messages.ProjectNotFound);

            var steps = await _stepRepository.GetStepsWithFilteredTasksAsync(request.ProjectId, request.UserIds, request.IncludeUnassigned, request.Priorities);

            var data = _mapper.Map<List<StepGetDTO>>(steps);

            return new DataResult<List<StepGetDTO>>(ResultStatus.Success, data);
        }
    }
}
