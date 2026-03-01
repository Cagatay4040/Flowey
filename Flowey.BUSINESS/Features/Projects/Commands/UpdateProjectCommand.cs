using AutoMapper;
using Flowey.BUSINESS.DTO.Project;
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
    public class UpdateProjectCommand : IRequest<IResult>
    {
        public ProjectUpdateDTO ProjectUpdateDTO { get; set; }

        public UpdateProjectCommand(ProjectUpdateDTO projectUpdateDTO)
        {
            ProjectUpdateDTO = projectUpdateDTO;
        }
    }

    public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, IResult>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProjectCommandHandler(
            IProjectRepository projectRepository, 
            IMapper mapper, 
            IUnitOfWork unitOfWork)
        {
            _projectRepository = projectRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {
            var existingProject = await _projectRepository.GetByIdAsync(request.ProjectUpdateDTO.Id);

            if (existingProject == null)
                return new Result(ResultStatus.Error, Messages.ProjectNotFound);

            _mapper.Map(request.ProjectUpdateDTO, existingProject);

            await _projectRepository.UpdateAsync(existingProject);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.ProjectUpdated);

            return new Result(ResultStatus.Error, Messages.ProjectUpdateError);
        }
    }
}
