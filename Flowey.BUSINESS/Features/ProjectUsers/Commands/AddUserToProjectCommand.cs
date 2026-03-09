using Flowey.CORE.Constants;
using Flowey.CORE.Enums;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DATACCESS.Abstract;
using Flowey.DOMAIN.Model.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.Features.ProjectUsers.Commands
{
    public class AddUserToProjectCommand : IRequest<IResult>
    {
        public Guid UserId { get; set; }
        public Guid ProjectId { get; set; }
        public RoleType RoleId { get; set; }

        public AddUserToProjectCommand(Guid userId, Guid projectId, RoleType role)
        {
            UserId = userId;
            ProjectId = projectId;
            RoleId = role;
        }
    }

    public class AddUserToProjectCommandHandler : IRequestHandler<AddUserToProjectCommand, IResult>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IEntityRepository<ProjectUserRole> _projectUserRoleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddUserToProjectCommandHandler(
            IProjectRepository projectRepository, 
            IEntityRepository<ProjectUserRole> projectUserRoleRepository, 
            IUnitOfWork unitOfWork)
        {
            _projectRepository = projectRepository;
            _projectUserRoleRepository = projectUserRoleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult> Handle(AddUserToProjectCommand request, CancellationToken cancellationToken)
        {
            var projectUser = new ProjectUserRole
            {
                UserId = request.UserId,
                ProjectId = request.ProjectId,
                RoleId = request.RoleId,
            };

            bool isExists = await _projectRepository.IsUserInProjectAsync(projectUser.ProjectId, projectUser.UserId);

            if (isExists)
                return new Result(ResultStatus.Error, Messages.ProjectAlreadyAssignedToUser);

            await _projectUserRoleRepository.AddAsync(projectUser);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.ProjectAssigned);

            return new Result(ResultStatus.Error, Messages.ProjectAssignError);
        }
    }
}
