using AutoMapper;
using Flowey.BUSINESS.Abstract;
using Flowey.BUSINESS.Constants;
using Flowey.BUSINESS.DTO.Step;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DATACCESS.Abstract;
using Flowey.DOMAIN.Model.Concrete;
using System;
using System.Collections.Generic;

using System.Linq;

namespace Flowey.BUSINESS.Concrete
{
    public class StepManager : IStepService
    {
        private readonly IStepRepository _stepRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IMapper _mapper;

        public StepManager(IStepRepository stepRepository, IProjectRepository projectRepository, IMapper mapper)
        {
            _stepRepository = stepRepository;
            _projectRepository = projectRepository;
            _mapper = mapper;
        }

        #region Get Methods

        public async Task<IDataResult<List<StepGetDTO>>> GetProjectSteps(Guid projectId)
        {
            var existingProject = await _projectRepository.AnyAsync(x => x.Id == projectId);

            if (!existingProject)
                return new DataResult<List<StepGetDTO>>(ResultStatus.Error, Messages.ProjectNotFound, new List<StepGetDTO>());

            var entityList = await _stepRepository.GetProjectStepsAsync(projectId);
            var data = _mapper.Map<List<StepGetDTO>>(entityList);
            return new DataResult<List<StepGetDTO>>(ResultStatus.Success, data);
        }

        public async Task<List<StepGetDTO>> GetBoardDataAsync(Guid projectId, List<Guid> userIds, bool includeUnassigned)
        {
            var steps = await _stepRepository.GetStepsWithFilteredTasksAsync(projectId, userIds, includeUnassigned);

            var stepDtos = _mapper.Map<List<StepGetDTO>>(steps);

            return stepDtos;
        }

        #endregion

        #region Insert Methods

        public async Task<IResult> AddStepAsync(StepAddDTO dto)
        {
            var existingProject = await _projectRepository.AnyAsync(x => x.Id == dto.ProjectId);

            if (!existingProject)
                return new Result(ResultStatus.Error, Messages.ProjectNotFound);

            var step = _mapper.Map<Step>(dto);

            int effectedRow = await _stepRepository.AddAsync(step);

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.StepAdded);

            return new Result(ResultStatus.Error, Messages.StepCreateError);
        }

        public async Task<IResult> AddRangeStepAsync(List<StepAddDTO> dtos)
        {
            if (dtos == null || !dtos.Any())
                return new Result(ResultStatus.Error, Messages.StepListEmpty);

            var firstProjectId = dtos.First().ProjectId;

            if (dtos.Any(x => x.ProjectId != firstProjectId))
            {
                return new Result(ResultStatus.Error, Messages.BulkStepProjectMismatch);
            }

            var existingProject = await _projectRepository.AnyAsync(x => x.Id == firstProjectId);

            if (!existingProject)
                return new Result(ResultStatus.Error, Messages.ProjectNotFound);

            var steps = _mapper.Map<List<Step>>(dtos);

            int effectedRow = await _stepRepository.AddRangeAsync(steps);

            if (effectedRow > 0)
            {
                string successMessage = string.Format(Messages.StepsCreatedSuccess, steps.Count);
                return new Result(ResultStatus.Success, successMessage);
            }

            return new Result(ResultStatus.Error, Messages.StepsCreateFailed);
        }

        #endregion

        #region Update Methods

        public async Task<IResult> UpdateStepAsync(StepUpdateDTO dto)
        {
            var existingStep = await _stepRepository.GetByIdAsync(dto.StepId);

            if (existingStep == null)
                return new Result(ResultStatus.Error, Messages.StepNotFound);

            _mapper.Map(dto, existingStep);

            int effectedRow = await _stepRepository.UpdateAsync(existingStep);

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.StepUpdated);

            return new Result(ResultStatus.Error, Messages.StepUpdateError);
        }

        public async Task<IResult> UpdateRangeStepAsync(List<StepUpdateDTO> dtos)
        {
            if (dtos == null || !dtos.Any())
                return new Result(ResultStatus.Error, Messages.UpdateListEmpty);

            var ids = dtos.Select(x => x.StepId).ToList();

            var existingSteps = await _stepRepository.GetList(x => ids.Contains(x.Id));

            if (existingSteps.Count != dtos.Count)
            {
                return new Result(ResultStatus.Error, Messages.BulkUpdateAborted);
            }

            foreach (var existingStep in existingSteps)
            {
                var matchingDto = dtos.First(x => x.StepId == existingStep.Id);
                _mapper.Map(matchingDto, existingStep);
            }

            int effectedRow = await _stepRepository.UpdateRangeAsync(existingSteps);

            if (effectedRow > 0)
            {
                string successMessage = string.Format(Messages.StepsUpdated, existingSteps.Count);
                return new Result(ResultStatus.Success, successMessage);
            }

            return new Result(ResultStatus.Error, Messages.StepsUpdateFailed);
        }

        #endregion

        #region Delete Methods

        public async Task<IResult> SoftDeleteAsync(Guid stepId)
        {
            var existingStep = await _stepRepository.GetByIdAsync(stepId);

            if (existingStep == null)
                return new Result(ResultStatus.Error, Messages.StepNotFound);

            int effectedRow = await _stepRepository.SoftDeleteAndReOrderStepsAsync(existingStep);

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.StepDeleted);

            return new Result(ResultStatus.Error, Messages.StepDeleteError);
        }

        #endregion
    }
}
