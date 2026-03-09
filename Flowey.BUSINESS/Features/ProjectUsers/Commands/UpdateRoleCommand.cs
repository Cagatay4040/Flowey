using Flowey.CORE.Constants;
using Flowey.CORE.Enums;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DATACCESS.Abstract;
using Flowey.DOMAIN.Model.Concrete;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.Features.ProjectUsers.Commands
{
    public class UpdateRoleCommand : IRequest<IResult>
    {
        public Guid ProjectId { get; set; }
        public Guid UserId { get; set; }
        public RoleType RoleId { get; set; }

        public UpdateRoleCommand(Guid projectId, Guid userId, RoleType roleId)
        {
            ProjectId = projectId;
            UserId = userId;
            RoleId = roleId;
        }
    }

    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, IResult>
    {
        private readonly IEntityRepository<ProjectUserRole> _projectUserRoleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateRoleCommandHandler(IEntityRepository<ProjectUserRole> projectUserRoleRepository, IUnitOfWork unitOfWork)
        {
            _projectUserRoleRepository = projectUserRoleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var relation = await _projectUserRoleRepository.FirstOrDefaultAsync(
                x => x.ProjectId == request.ProjectId &&
                     x.UserId == request.UserId);

            if (relation == null)
                return new Result(ResultStatus.Error, Messages.ProjectUserNotFound);

            relation.RoleId = request.RoleId;

            await _projectUserRoleRepository.UpdateAsync(relation);

            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if(effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.UserRoleUpdated);

            return new Result(ResultStatus.Error, Messages.UserRoleUpdateFailed);
        }
    }
}
