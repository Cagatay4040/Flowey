using Flowey.CORE.Interfaces.Repositories;
using Flowey.CORE.Interfaces.UnitOfWork;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DOMAIN.Model.Concrete;
using Flowey.SHARED.Constants;
using MediatR;

namespace Flowey.BUSINESS.Features.ProjectUsers.Commands
{
    public class RemoveUserFromProjectCommand : IRequest<IResult>
    {
        public Guid UserId { get; set; }
        public Guid ProjectId { get; set; }

        public RemoveUserFromProjectCommand(Guid userId, Guid projectId)
        {
            UserId = userId;
            ProjectId = projectId;
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
                x => x.ProjectId == request.ProjectId && 
                     x.UserId == request.UserId);

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
