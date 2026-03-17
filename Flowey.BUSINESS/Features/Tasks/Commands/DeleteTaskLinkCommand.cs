using Flowey.CORE.Interfaces.Repositories;
using Flowey.CORE.Interfaces.UnitOfWork;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using MediatR;

namespace Flowey.BUSINESS.Features.Tasks.Commands
{
    public class DeleteTaskLinkCommand : IRequest<IResult>
    {
        public Guid SourceTaskId { get; set; }
        public Guid TargetTaskId { get; set; }

        public DeleteTaskLinkCommand(Guid sourceTaskId, Guid targetTaskId)
        {
            SourceTaskId = sourceTaskId;
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
                    (x.SourceTaskId == request.SourceTaskId && x.TargetTaskId == request.TargetTaskId) ||
                    (x.SourceTaskId == request.TargetTaskId && x.TargetTaskId == request.SourceTaskId));

                if (link == null)
                    return new Result(ResultStatus.Error, "Task link not found.");

                await _taskLinkRepository.DeleteAsync(link);

                int effectedRows = await _unitOfWork.SaveChangesAsync();

                if (effectedRows > 0)
                    return new Result(ResultStatus.Success, "Task link deleted successfully.");

                return new Result(ResultStatus.Error, "Failed to delete task link.");
            }
        }
    }
}
