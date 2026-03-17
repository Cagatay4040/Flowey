using Flowey.CORE.DTO.User;
using Flowey.CORE.Interfaces.Repositories;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.SHARED.Constants;
using MediatR;

namespace Flowey.BUSINESS.Features.ProjectUsers.Queries
{
    public class GetProjectUsersQuery : IRequest<IDataResult<List<UserSelectDTO>>>
    {
        public Guid ProjectId { get; set; }

        public GetProjectUsersQuery(Guid projectId)
        {
            ProjectId = projectId;
        }
    }

    public class GetProjectUsersQueryHandler : IRequestHandler<GetProjectUsersQuery, IDataResult<List<UserSelectDTO>>>
    {
        private readonly IProjectRepository _projectRepository;

        public GetProjectUsersQueryHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<IDataResult<List<UserSelectDTO>>> Handle(GetProjectUsersQuery request, CancellationToken cancellationToken)
        {
            var entities = await _projectRepository.GetProjectWithUsersAsync(request.ProjectId);

            if(entities == null)
                return new DataResult<List<UserSelectDTO>>(ResultStatus.Error, Messages.ProjectNotFound, new List<UserSelectDTO>());

            var data = entities.ProjectUserRoles.Select(role => new UserSelectDTO
            {
                Id = role.User.Id,
                FullName = $"{role.User.Name} {role.User.Surname}",
                Email = role.User.Email,
                RoleId = (int)role.RoleId,
                RoleName = role.RoleId.ToString().ToUpper()
            }).ToList();

            return new DataResult<List<UserSelectDTO>>(ResultStatus.Success, data);
        }
    }
}
