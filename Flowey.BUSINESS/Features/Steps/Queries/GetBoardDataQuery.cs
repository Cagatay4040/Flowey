using Flowey.BUSINESS.DTO.Step;
using Flowey.CORE.Constants;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DATACCESS.Abstract;
using MediatR;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.Features.Steps.Queries
{
    public class GetBoardDataQuery : IRequest<IDataResult<List<StepGetDTO>>>
    {
        public Guid ProjectId { get; set; }
        public List<Guid> UserIds { get; set; }
        public bool IncludeUnassigned { get; set; }

        public GetBoardDataQuery(Guid projectId, List<Guid> userIds, bool includeUnassigned)
        {
            ProjectId = projectId;
            UserIds = userIds;
            IncludeUnassigned = includeUnassigned;
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
                return new DataResult<List<StepGetDTO>>(ResultStatus.Error, Messages.ProjectNotFound, new List<StepGetDTO>());

            var steps = await _stepRepository.GetStepsWithFilteredTasksAsync(request.ProjectId, request.UserIds, request.IncludeUnassigned);

            var data = _mapper.Map<List<StepGetDTO>>(steps);

            return new DataResult<List<StepGetDTO>>(ResultStatus.Success, data);
        }
    }
}
