using Flowey.CORE.DTO.Task;
using Flowey.CORE.Interfaces.Repositories;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.SHARED.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowey.BUSINESS.Features.Tasks.Queries
{
    public class GetTaskLinksQuery : IRequest<IDataResult<List<RelatedTaskDTO>>>
    {
        public Guid TaskId { get; set; }

        public GetTaskLinksQuery(Guid taskId)
        {
            TaskId = taskId;
        }
    }

    public class GetTaskLinksQueryHandler : IRequestHandler<GetTaskLinksQuery, IDataResult<List<RelatedTaskDTO>>>
    {
        private readonly ITaskLinkRepository _taskLinkRepository;

        public GetTaskLinksQueryHandler(ITaskLinkRepository taskLinkRepository)
        {
            _taskLinkRepository = taskLinkRepository;
        }

        public async Task<IDataResult<List<RelatedTaskDTO>>> Handle(GetTaskLinksQuery request, CancellationToken cancellationToken)
        {
            var outgoingRaw = await _taskLinkRepository
                .GetQueryable(x => x.SourceTaskId == request.TaskId, noTracking: true)
                .Include(x => x.TargetTask)
                .Select(x => new
                {
                    TaskId = x.TargetTaskId,
                    Title = x.TargetTask.Title,
                    TaskKey = x.TargetTask.TaskKey,
                    LinkType = x.LinkType
                })
                .ToListAsync(cancellationToken); 

            var outgoingLinks = outgoingRaw.Select(x => new RelatedTaskDTO
            {
                TaskId = x.TaskId,
                Title = x.Title,
                TaskKey = x.TaskKey,
                RelationType = GetRelationDisplayName(x.LinkType, true)
            }).ToList();

            var incomingRaw = await _taskLinkRepository
                .GetQueryable(x => x.TargetTaskId == request.TaskId, noTracking: true)
                .Include(x => x.SourceTask)
                .Select(x => new
                {
                    TaskId = x.SourceTaskId,
                    Title = x.SourceTask.Title,
                    TaskKey = x.SourceTask.TaskKey,
                    LinkType = x.LinkType
                })
                .ToListAsync(cancellationToken);

            var incomingLinks = incomingRaw.Select(x => new RelatedTaskDTO
            {
                TaskId = x.TaskId,
                Title = x.Title,
                TaskKey = x.TaskKey,
                RelationType = GetRelationDisplayName(x.LinkType, false)
            }).ToList();

            var allRelatedTasks = outgoingLinks.Concat(incomingLinks).ToList();

            return new DataResult<List<RelatedTaskDTO>>(ResultStatus.Success, allRelatedTasks);
        }

        private static string GetRelationDisplayName(LinkType linkType, bool isOutgoing)
        {
            return linkType switch
            {
                LinkType.Blocks => isOutgoing ? "Blocks" : "Is Blocked By",
                LinkType.Duplicates => isOutgoing ? "Duplicates" : "Is Duplicated By",
                LinkType.RelatesTo => "Relates To",
                _ => "Unknown Relation"
            };
        }
    }
}
