using Flowey.CORE.Interfaces.Repositories;
using Flowey.CORE.Interfaces.Security;
using Flowey.CORE.Interfaces.UnitOfWork;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.SHARED.Constants;
using Flowey.SHARED.Enums;
using MediatR;

namespace Flowey.BUSINESS.Features.Tasks.Commands
{
    public class DeleteTaskLinkCommand : IRequest<IResult>, IRequireTaskAuthorization
    {
        public Guid TaskId { get; set; }
        public Guid TargetTaskId { get; set; }

        public RoleType[] RequiredRoles => new[] { RoleType.Admin, RoleType.Editor, RoleType.Member };

        public DeleteTaskLinkCommand(Guid taskId, Guid targetTaskId)
        {
            TaskId = taskId;
            TargetTaskId = targetTaskId;
        }

        public class DeleteTaskLinkCommandHandler : IRequestHandler<DeleteTaskLinkCommand, IResult>
        {
            private readonly ITaskLinkRepository _taskLinkRepository;
            private readonly IUnitOfWork _unitOfWork;

            public DeleteTaskLinkCommandHandler(
                ITaskLinkRepository taskLinkRepository,
                IUnitOfWork unitOfWork)
            {
                _taskLinkRepository = taskLinkRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<IResult> Handle(DeleteTaskLinkCommand request, CancellationToken cancellationToken)
            {
                var link = await _taskLinkRepository.FirstOrDefaultAsync(x => 
                    (x.SourceTaskId == request.TaskId && x.TargetTaskId == request.TargetTaskId) ||
                    (x.SourceTaskId == request.TargetTaskId && x.TargetTaskId == request.TaskId));

                if (link == null)
                    return new Result(ResultStatus.Error, Messages.TaskLinkNotFound);

                await _taskLinkRepository.DeleteAsync(link);

                int effectedRows = await _unitOfWork.SaveChangesAsync();

                if (effectedRows > 0)
                    return new Result(ResultStatus.Success, Messages.TaskLinkDeleted);

                return new Result(ResultStatus.Error, Messages.TaskLinkDeleteError);
            }
        }
    }
}
