using Flowey.CORE.Constants;
using Flowey.CORE.DataAccess.Abstract;
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
    public class TransferOwnershipCommand : IRequest<IResult>
    {
        public Guid ProjectId { get; set; }
        public Guid NewOwnerId { get; set; }

        public TransferOwnershipCommand(Guid projectId, Guid newOwnerId)
        {
            ProjectId = projectId;
            NewOwnerId = newOwnerId;
        }
    }

    public class TransferOwnershipCommandHandler : IRequestHandler<TransferOwnershipCommand, IResult>
    {
        private readonly IEntityRepository<ProjectUserRole> _projectUserRoleRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public TransferOwnershipCommandHandler(
            IEntityRepository<ProjectUserRole> projectUserRoleRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _projectUserRoleRepository = projectUserRoleRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult> Handle(TransferOwnershipCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();

            if(currentUserId == request.NewOwnerId)
                return new Result(ResultStatus.Error, Messages.CannotTransferOwnershipToYourself);

            var oldOwner = await _projectUserRoleRepository.FirstOrDefaultAsync(
                x => x.ProjectId == request.ProjectId &&
                     x.UserId == currentUserId);

            if (oldOwner == null)
                return new Result(ResultStatus.Error, Messages.ProjectUserNotFound);

            if (oldOwner.RoleId != RoleType.Admin)
                return new Result(ResultStatus.Error, Messages.UnauthorizedToTransferOwnership);

            var newOwner = await _projectUserRoleRepository.FirstOrDefaultAsync(
                x => x.ProjectId == request.ProjectId &&
                     x.UserId == request.NewOwnerId);

            if (newOwner == null)
                return new Result(ResultStatus.Error, Messages.ProjectUserNotFound);

            newOwner.RoleId = RoleType.Admin;
            oldOwner.RoleId = RoleType.Member;

            await _projectUserRoleRepository.UpdateAsync(newOwner);
            await _projectUserRoleRepository.UpdateAsync(oldOwner);

            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.ProjectOwnershipTransferred);

            return new Result(ResultStatus.Error, Messages.ProjectOwnershipTransferFailed);
        }
    }
}
