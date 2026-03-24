using Flowey.CORE.Interfaces.Repositories;
using Flowey.CORE.Interfaces.Security;
using Flowey.CORE.Interfaces.UnitOfWork;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DOMAIN.Model.Concrete;
using Flowey.SHARED.Constants;
using Flowey.SHARED.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowey.BUSINESS.Features.Tasks.Commands
{
    public class LinkTasksCommand : IRequest<IResult>, IRequireTaskAuthorization
    {
        public Guid TaskId { get; set; }
        public Guid TargetTaskId { get; set; }
        public LinkType LinkType { get; set; }

        public RoleType[] RequiredRoles => new[] { RoleType.Admin, RoleType.Editor, RoleType.Member };

        public LinkTasksCommand(Guid taskId, Guid targetTaskId, LinkType linkType)
        {
            TaskId = taskId;
            TargetTaskId = targetTaskId;
            LinkType = linkType;
        }

        public class LinkTasksCommandHandler : IRequestHandler<LinkTasksCommand, IResult>
        {
            private readonly ITaskRepository _taskRepository;
            private readonly ITaskLinkRepository _taskLinkRepository;
            private readonly IUnitOfWork _unitOfWork;

            public LinkTasksCommandHandler(
                ITaskRepository taskRepository,
                ITaskLinkRepository taskLinkRepository,
                IUnitOfWork unitOfWork)
            {
                _taskRepository = taskRepository;
                _taskLinkRepository = taskLinkRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<IResult> Handle(LinkTasksCommand request, CancellationToken cancellationToken)
            {
                if (request.TaskId == request.TargetTaskId)
                    return new Result(ResultStatus.Error, Messages.CannotLinkTaskToItself);

                var sourceExists = await _taskRepository.AnyAsync(x => x.Id == request.TaskId);
                var targetExists = await _taskRepository.AnyAsync(x => x.Id == request.TargetTaskId);

                if (!sourceExists || !targetExists)
                    return new Result(ResultStatus.Error, Messages.TaskNotFound);

                var existingLink = await _taskLinkRepository.GetQueryable(x =>
                    x.SourceTaskId == request.TaskId &&
                    x.TargetTaskId == request.TargetTaskId &&
                    x.LinkType == request.LinkType)
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(cancellationToken);

                if (existingLink != null)
                {
                    if (existingLink.IsActive)
                        return new Result(ResultStatus.Error, Messages.TaskLinkAlreadyExists);

                    existingLink.IsActive = true;
                    await _taskLinkRepository.UpdateAsync(existingLink);
                }
                else
                {
                    var taskLink = new TaskLink
                    {
                        SourceTaskId = request.TaskId,
                        TargetTaskId = request.TargetTaskId,
                        LinkType = request.LinkType
                    };

                    await _taskLinkRepository.AddAsync(taskLink);
                }

                int effectedRows = await _unitOfWork.SaveChangesAsync();

                if (effectedRows > 0)
                    return new Result(ResultStatus.Success, Messages.TasksLinkedSuccessfully);

                return new Result(ResultStatus.Error, Messages.TaskLinkFailed);
            }
        }
    }
}
