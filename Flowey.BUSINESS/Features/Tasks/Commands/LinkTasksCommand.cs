using Flowey.CORE.Constants;
using Flowey.CORE.Enums;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DATACCESS.Abstract;
using Flowey.DOMAIN.Model.Concrete;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.Features.Tasks.Commands
{
    public class LinkTasksCommand : IRequest<IResult>
    {
        public Guid SourceTaskId { get; set; }
        public Guid TargetTaskId { get; set; }
        public LinkType LinkType { get; set; }

        public LinkTasksCommand(Guid sourceTaskId, Guid targetTaskId, LinkType linkType)
        {
            SourceTaskId = sourceTaskId;
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
                if (request.SourceTaskId == request.TargetTaskId)
                    return new Result(ResultStatus.Error, Messages.CannotLinkTaskToItself);

                var sourceExists = await _taskRepository.AnyAsync(x => x.Id == request.SourceTaskId);
                var targetExists = await _taskRepository.AnyAsync(x => x.Id == request.TargetTaskId);

                if (!sourceExists || !targetExists)
                    return new Result(ResultStatus.Error, Messages.TaskNotFound);

                var existingLink = await _taskLinkRepository.GetQueryable(x =>
                    x.SourceTaskId == request.SourceTaskId &&
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
                        SourceTaskId = request.SourceTaskId,
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
