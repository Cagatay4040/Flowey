using Flowey.BUSINESS.DTO.Step;
using Flowey.CORE.Result.Abstract;
using Flowey.DOMAIN.Model.Concrete;
using System.Collections.Generic;

namespace Flowey.BUSINESS.Abstract
{
    public interface IStepService
    {
        #region Get Methods

        Task<IDataResult<List<StepGetDTO>>> GetProjectSteps(Guid projectId);
        Task<List<StepGetDTO>> GetBoardDataAsync(Guid projectId, List<Guid> userIds, bool includeUnassigned);

        #endregion

        #region Insert Methods

        Task<IResult> AddStepAsync(StepAddDTO dto);
        Task<IResult> AddRangeStepAsync(List<StepAddDTO> dtos);

        #endregion

        #region Update Methods

        Task<IResult> UpdateStepAsync(StepUpdateDTO dto);
        Task<IResult> UpdateRangeStepAsync(List<StepUpdateDTO> dtos);

        #endregion

        #region Delete Methods

        Task<IResult> SoftDeleteAsync(Guid stepId);

        #endregion
    }
}
