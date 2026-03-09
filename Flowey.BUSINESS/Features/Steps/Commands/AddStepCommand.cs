using Flowey.CORE.Constants;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DATACCESS.Abstract;
using Flowey.DOMAIN.Model.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.Features.Steps.Commands
{
    public class AddStepCommand : IRequest<IResult>
    {
        public string Name { get; set; }
        public int Order { get; set; }
        public Guid ProjectId { get; set; }

        public AddStepCommand(string name, int order, Guid projectId)
        {
            Name = name;
            Order = order;
            ProjectId = projectId;
        }
    }

    public class AddStepCommandHandler : IRequestHandler<AddStepCommand, IResult>
    {
        private readonly IStepRepository _stepRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddStepCommandHandler(
            IStepRepository stepRepository, 
            IProjectRepository projectRepository, 
            IUnitOfWork unitOfWork)
        {
            _stepRepository = stepRepository;
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult> Handle(AddStepCommand request, CancellationToken cancellationToken)
        {
            var existingProject = await _projectRepository.AnyAsync(x => x.Id == request.ProjectId);

            if (!existingProject)
                return new Result(ResultStatus.Error, Messages.ProjectNotFound);

            var step = new Step
            {
                Name = request.Name,
                Order = request.Order,
                ProjectId = request.ProjectId
            };

            await _stepRepository.AddAsync(step);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.StepAdded);

            return new Result(ResultStatus.Error, Messages.StepCreateError);
        }
    }
}
