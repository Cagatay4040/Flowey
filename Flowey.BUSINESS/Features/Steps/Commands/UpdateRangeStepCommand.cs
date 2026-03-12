using AutoMapper;
using Flowey.BUSINESS.DTO.Step;
using Flowey.CORE.Constants;
using Flowey.CORE.Enums;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DATACCESS.Abstract;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.Features.Steps.Commands
{
    public class UpdateRangeStepCommand : IRequest<IResult>
    {
        public List<StepUpdateDTO> StepUpdateDTOs { get; set; }

        public UpdateRangeStepCommand(List<StepUpdateDTO> stepUpdateDTOs)
        {
            StepUpdateDTOs = stepUpdateDTOs;
        }
    }

    public class UpdateRangeStepCommandHandler : IRequestHandler<UpdateRangeStepCommand, IResult>
    {
        private readonly IStepRepository _stepRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateRangeStepCommandHandler(
            IStepRepository stepRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _stepRepository = stepRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult> Handle(UpdateRangeStepCommand request, CancellationToken cancellationToken)
        {
            if (request.StepUpdateDTOs == null || !request.StepUpdateDTOs.Any())
                return new Result(ResultStatus.Error, Messages.UpdateListEmpty);

            var ids = request.StepUpdateDTOs.Select(x => x.StepId).ToList();
            var existingSteps = await _stepRepository.GetList(x => ids.Contains(x.Id));

            if (existingSteps.Count != request.StepUpdateDTOs.Count)
            {
                return new Result(ResultStatus.Error, Messages.BulkUpdateAborted);
            }

            var affectedProjectIds = existingSteps.Select(x => x.ProjectId).Distinct().ToList();

            var allStepsInAffectedProjects = await _stepRepository.GetList(x => affectedProjectIds.Contains(x.ProjectId));

            foreach (var existingStep in existingSteps)
            {
                var matchingDto = request.StepUpdateDTOs.First(x => x.StepId == existingStep.Id);
                _mapper.Map(matchingDto, existingStep);
            }

            foreach (var projectId in affectedProjectIds)
            {
                var projectStepsAfterUpdate = allStepsInAffectedProjects.Where(x => x.ProjectId == projectId).ToList();

                bool hasToDo = projectStepsAfterUpdate.Any(x => x.Category == StepCategory.ToDo);
                bool hasDone = projectStepsAfterUpdate.Any(x => x.Category == StepCategory.Done);

                if (!hasToDo || !hasDone)
                    return new Result(ResultStatus.Error, Messages.CannotDeleteLastRequiredCategoryStepBulk);
            }

            await _stepRepository.UpdateRangeAsync(existingSteps);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
            {
                string successMessage = string.Format(Messages.StepsUpdated, existingSteps.Count);
                return new Result(ResultStatus.Success, successMessage);
            }

            return new Result(ResultStatus.Error, Messages.StepsUpdateFailed);
        }
    }
}
