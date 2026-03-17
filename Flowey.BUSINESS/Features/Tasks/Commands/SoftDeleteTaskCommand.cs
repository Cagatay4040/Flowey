using Flowey.CORE.Interfaces.Repositories;
using Flowey.CORE.Interfaces.UnitOfWork;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.SHARED.Constants;
using MediatR;

namespace Flowey.BUSINESS.Features.Tasks.Commands
{
    public class SoftDeleteTaskCommand : IRequest<IResult>
    {
        public Guid Id { get; set; }

        public SoftDeleteTaskCommand(Guid id)
        {
            Id = id;
        }
    }

    public class SoftDeleteTaskCommandHandler : IRequestHandler<SoftDeleteTaskCommand, IResult>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SoftDeleteTaskCommandHandler(ITaskRepository taskRepository, IUnitOfWork unitOfWork)
        {
            _taskRepository = taskRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult> Handle(SoftDeleteTaskCommand request, CancellationToken cancellationToken)
        {
            var existingTask = await _taskRepository.FirstOrDefaultAsync(x => x.Id == request.Id);

            if (existingTask == null)
                return new Result(ResultStatus.Error, Messages.TaskNotFound);

            await _taskRepository.SoftDeleteAsync(existingTask);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.TaskDeleted);

            return new Result(ResultStatus.Error, Messages.TaskDeleteError);
        }
    }
}
