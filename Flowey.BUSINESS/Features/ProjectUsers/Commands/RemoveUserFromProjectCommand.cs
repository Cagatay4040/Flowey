using Flowey.BUSINESS.DTO.ProjectUser;
using Flowey.CORE.Constants;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DATACCESS.Abstract;
using Flowey.DOMAIN.Model.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.Features.ProjectUsers.Commands
{
    public class RemoveUserFromProjectCommand : IRequest<IResult>
    {
        public ProjectRemoveUserDTO ProjectRemoveUserDTO { get; set; }

        public RemoveUserFromProjectCommand(ProjectRemoveUserDTO projectRemoveUserDTO)
        {
            ProjectRemoveUserDTO = projectRemoveUserDTO;
        }
    }

    public class RemoveUserFromProjectCommandHandler : IRequestHandler<RemoveUserFromProjectCommand, IResult>
    {
        private readonly IEntityRepository<ProjectUserRole> _projectUserRoleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveUserFromProjectCommandHandler(
            IEntityRepository<ProjectUserRole> projectUserRoleRepository, 
            IUnitOfWork unitOfWork)
        {
            _projectUserRoleRepository = projectUserRoleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult> Handle(RemoveUserFromProjectCommand request, CancellationToken cancellationToken)
        {
            var relation = await _projectUserRoleRepository.FirstOrDefaultAsync(
                x => x.ProjectId == request.ProjectRemoveUserDTO.ProjectId && 
                     x.UserId == request.ProjectRemoveUserDTO.UserId);

            if (relation == null)
                return new Result(ResultStatus.Error, Messages.ProjectUserNotFound);

            await _projectUserRoleRepository.SoftDeleteAsync(relation);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.ProjectRemoveUser);

            return new Result(ResultStatus.Error, Messages.ProjectRemoveUserError);
        }
    }
}
