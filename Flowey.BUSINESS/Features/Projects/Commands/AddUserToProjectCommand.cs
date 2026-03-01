using AutoMapper;
using Flowey.BUSINESS.DTO.ProjectUser;
using Flowey.CORE.Constants;
using Flowey.CORE.DataAccess.Abstract;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DATACCESS.Abstract;
using Flowey.DOMAIN.Model.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.Features.Projects.Commands
{
    public class AddUserToProjectCommand : IRequest<IResult>
    {
        public ProjectUserAddDTO ProjectUserAddDTO { get; set; }

        public AddUserToProjectCommand(ProjectUserAddDTO projectUserAddDTO)
        {
            ProjectUserAddDTO = projectUserAddDTO;
        }
    }

    public class AddUserToProjectCommandHandler : IRequestHandler<AddUserToProjectCommand, IResult>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IEntityRepository<ProjectUserRole> _projectUserRoleRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public AddUserToProjectCommandHandler(
            IProjectRepository projectRepository, 
            IEntityRepository<ProjectUserRole> projectUserRoleRepository, 
            IMapper mapper, 
            IUnitOfWork unitOfWork)
        {
            _projectRepository = projectRepository;
            _projectUserRoleRepository = projectUserRoleRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult> Handle(AddUserToProjectCommand request, CancellationToken cancellationToken)
        {
            var projectUser = _mapper.Map<ProjectUserRole>(request.ProjectUserAddDTO);

            bool isExists = await _projectRepository.IsUserInProjectAsync(projectUser.ProjectId, projectUser.UserId);
            if (isExists)
                return new Result(ResultStatus.Error, Messages.ProjectAlreadyAssignedToUser);

            await _projectUserRoleRepository.AddAsync(projectUser);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.ProjectAssigned);

            return new Result(ResultStatus.Error, Messages.ProjectAssignError);
        }
    }
}
