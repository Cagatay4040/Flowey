using Flowey.CORE.Constants;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DATACCESS.Abstract;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.Features.Projects.Commands
{
    public class DeleteProjectCommand : IRequest<IResult>
    {
        public Guid ProjectId { get; set; }

        public DeleteProjectCommand(Guid projectId)
        {
            ProjectId = projectId;
        }
    }

    public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, IResult>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteProjectCommandHandler(
            IProjectRepository projectRepository, 
            IUnitOfWork unitOfWork)
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
        {
            var existingProject = await _projectRepository.GetByIdAsync(request.ProjectId);

            if (existingProject == null)
                return new Result(ResultStatus.Error, Messages.ProjectNotFound);

            await _projectRepository.DeleteAsync(existingProject);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.ProjectDeleted);

            return new Result(ResultStatus.Error, Messages.ProjectDeleteError);
        }
    }
}
