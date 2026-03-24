using Flowey.CORE.Interfaces.Repositories;
using Flowey.CORE.Interfaces.Security;
using Flowey.CORE.Interfaces.UnitOfWork;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.SHARED.Constants;
using Flowey.SHARED.Enums;
using MediatR;

namespace Flowey.BUSINESS.Features.Projects.Commands
{
    public class UpdateProjectCommand : IRequest<IResult>, IRequireProjectAuthorization
    {
        public Guid ProjectId { get; set; }
        public string Name { get; set; }
        public string ProjectKey { get; set; }

        public RoleType[] RequiredRoles => new[] { RoleType.Admin };

        public UpdateProjectCommand(Guid projectId, string name, string projectKey)
        {
            ProjectId = projectId;
            Name = name;
            ProjectKey = projectKey;
        }
    }

    public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, IResult>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProjectCommandHandler(
            IProjectRepository projectRepository, 
            IUnitOfWork unitOfWork)
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {
            var existingProject = await _projectRepository.GetByIdAsync(request.ProjectId);

            if (existingProject == null)
                return new Result(ResultStatus.Error, Messages.ProjectNotFound);

            existingProject.Name = request.Name;
            existingProject.ProjectKey = request.ProjectKey;

            await _projectRepository.UpdateAsync(existingProject);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.ProjectUpdated);

            return new Result(ResultStatus.Error, Messages.ProjectUpdateError);
        }
    }
}
