using AutoMapper;
using Flowey.CORE.DTO.Task;
using Flowey.CORE.Interfaces.Repositories;
using Flowey.CORE.Interfaces.Security;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.SHARED.Enums;
using MediatR;

namespace Flowey.BUSINESS.Features.Tasks.Queries
{
    public class GetProjectTasksQuery : IRequest<IDataResult<List<TaskGetDTO>>>, IRequireProjectAuthorization
    {
        public Guid ProjectId { get; set; }

        public RoleType[] RequiredRoles => new[] { RoleType.Admin, RoleType.Editor, RoleType.Member };

        public GetProjectTasksQuery(Guid projectId)
        {
            ProjectId = projectId;
        }
    }

    public class GetProjectTasksQueryHandler : IRequestHandler<GetProjectTasksQuery, IDataResult<List<TaskGetDTO>>>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IMapper _mapper;

        public GetProjectTasksQueryHandler(ITaskRepository taskRepository, IMapper mapper)
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
        }

        public async Task<IDataResult<List<TaskGetDTO>>> Handle(GetProjectTasksQuery request, CancellationToken cancellationToken)
        {
            var entityList = await _taskRepository.GetList(x => x.ProjectId == request.ProjectId);
            var data = _mapper.Map<List<TaskGetDTO>>(entityList);
            return new DataResult<List<TaskGetDTO>>(ResultStatus.Success, data);
        }
    }
}
