using Flowey.CORE.Interfaces.Repositories;
using Flowey.CORE.Interfaces.UnitOfWork;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.SHARED.Constants;
using MediatR;

namespace Flowey.BUSINESS.Features.Projects.Commands
{
    public class SoftDeleteProjectCommand : IRequest<IResult>
    {
        public Guid ProjectId { get; set; }

        public SoftDeleteProjectCommand(Guid projectId)
        {
            ProjectId = projectId;
        }
    }

    public class SoftDeleteProjectCommandHandler : IRequestHandler<SoftDeleteProjectCommand, IResult>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SoftDeleteProjectCommandHandler(
            IProjectRepository projectRepository, 
            IUnitOfWork unitOfWork)
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult> Handle(SoftDeleteProjectCommand request, CancellationToken cancellationToken)
        {
            var existingProject = await _projectRepository.FirstOrDefaultAsync(x => x.Id == request.ProjectId);

            if (existingProject == null)
                return new Result(ResultStatus.Error, Messages.ProjectNotFound);

            await _projectRepository.SoftDeleteAsync(existingProject);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.ProjectDeleted);

            return new Result(ResultStatus.Error, Messages.ProjectDeleteError);
        }
    }
}
