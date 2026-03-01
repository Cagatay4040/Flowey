using Flowey.BUSINESS.DTO.Project;
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
        public ProjectAddDTO ProjectAddDTO { get; set; }

        public AddProjectWithCreatorCommand(ProjectAddDTO projectAddDTO)
        {
            ProjectAddDTO = projectAddDTO;
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
            var newProject = new Project
            {
                Id = Guid.NewGuid(),
                Name = request.ProjectAddDTO.Name,
                ProjectKey = request.ProjectAddDTO.ProjectKey,
                ProjectUserRoles = new List<ProjectUserRole>
                {
                    new ProjectUserRole
                    {
                        UserId = _currentUserService.GetUserId().Value,
                        RoleId = (int)RoleType.Admin
                    }
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
