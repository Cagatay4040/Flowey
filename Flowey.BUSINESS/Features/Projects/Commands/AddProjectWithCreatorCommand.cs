using Flowey.CORE.Constants;
using Flowey.CORE.DataAccess.Abstract;
using Flowey.CORE.Enums;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DATACCESS.Abstract;
using Flowey.DOMAIN.Model.Concrete;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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
        private readonly IStepRepository _stepRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public AddProjectWithCreatorCommandHandler(
            IProjectRepository projectRepository,
            IStepRepository stepRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _projectRepository = projectRepository;
            _stepRepository = stepRepository;
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
            }
            };

            await _projectRepository.AddAsync(newProject);

            var defaultSteps = new List<Step>
            {
                new Step { ProjectId = projectId, Name = "To Do", Order = 1, Category = StepCategory.ToDo },
                new Step { ProjectId = projectId, Name = "In Progress", Order = 2, Category = StepCategory.InProgress },
                new Step { ProjectId = projectId, Name = "Done", Order = 3, Category = StepCategory.Done }
            };

            await _stepRepository.AddRangeAsync(defaultSteps);

            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.ProjectAdded);

            return new Result(ResultStatus.Error, Messages.ProjectCreateError);
        }
    }
}
