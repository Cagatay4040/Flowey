using Flowey.CORE.Interfaces.Repositories;
using Flowey.CORE.Interfaces.Services;
using Flowey.CORE.Interfaces.UnitOfWork;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DOMAIN.Model.Concrete;
using Flowey.SHARED.Constants;
using Flowey.SHARED.Enums;
using MediatR;

namespace Flowey.BUSINESS.Features.Projects.Commands
{
    public class AddProjectWithCreatorCommand : IRequest<IResult>
    {
        public string Name { get; set; }
        public string ProjectKey { get; set; }

        public AddProjectWithCreatorCommand(string name, string projectKey)
        {
            Name = name;
            ProjectKey = projectKey;
        }
    }

    public class AddProjectWithCreatorCommandHandler : IRequestHandler<AddProjectWithCreatorCommand, IResult>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public AddProjectWithCreatorCommandHandler(
            IProjectRepository projectRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _projectRepository = projectRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult> Handle(AddProjectWithCreatorCommand request, CancellationToken cancellationToken)
        {
            var projectId = Guid.NewGuid();

            var newProject = new Project
            {
                Id = projectId,
                Name = request.Name,
                ProjectKey = request.ProjectKey,
                ProjectUserRoles = new List<ProjectUserRole>
                {
                    new ProjectUserRole
                    {
                        UserId = _currentUserService.GetUserId().Value,
                        RoleId = RoleType.Admin
                    }
                },
                Steps = new List<Step>
                {
                    new Step { ProjectId = projectId, Name = "To Do", Order = 1, Category = StepCategory.ToDo },
                    new Step { ProjectId = projectId, Name = "In Progress", Order = 2, Category = StepCategory.InProgress },
                    new Step { ProjectId = projectId, Name = "Done", Order = 3, Category = StepCategory.Done }
                }
            };

            await _projectRepository.AddAsync(newProject);

            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.ProjectAdded);

            return new Result(ResultStatus.Error, Messages.ProjectCreateError);
        }
    }
}
