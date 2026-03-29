using AutoMapper;
using Flowey.CORE.DTO.Project;
using Flowey.CORE.Interfaces.Repositories;
using Flowey.CORE.Interfaces.UnitOfWork;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DOMAIN.Model.Concrete;
using Flowey.SHARED.Constants;
using Flowey.SHARED.Enums;
using MediatR;

namespace Flowey.BUSINESS.Features.Projects.Commands
{
    public class AddProjectCommand : IRequest<IResult>
    {
        public ProjectAddDTO ProjectAddDTO { get; set; }

        public AddProjectCommand(ProjectAddDTO projectAddDTO)
        {
            ProjectAddDTO = projectAddDTO;
        }
    }

    public class AddProjectCommandHandler : IRequestHandler<AddProjectCommand, IResult>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public AddProjectCommandHandler(
            IProjectRepository projectRepository, 
            IMapper mapper, 
            IUnitOfWork unitOfWork)
        {
            _projectRepository = projectRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult> Handle(AddProjectCommand request, CancellationToken cancellationToken)
        {
            var project = _mapper.Map<Project>(request.ProjectAddDTO);

            await _projectRepository.AddAsync(project);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.ProjectAdded);

            return new Result(ResultStatus.Error, Messages.ProjectCreateError);
        }
    }
}
