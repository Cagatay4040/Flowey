using Flowey.BUSINESS.DTO.Task;
using Flowey.CORE.Result.Abstract;
using Flowey.DOMAIN.Model.Concrete;
using System.Collections.Generic;

namespace Flowey.BUSINESS.Abstract
{
    public interface ITaskService
    {
        #region Get Methods

        Task<IDataResult<TaskGetDTO>> GetByIdAsync(Guid id);
        Task<IDataResult<List<TaskGetDTO>>> GetProjectTasksAsync(Guid projectId);
        Task<IDataResult<List<TaskHistoryGetDTO>>> GetTaskHistoryAsync(Guid taskId);

        #endregion

        #region Insert Methods

        Task<IResult> AddAndAssignTaskAsync(TaskAddDTO dto);

        #endregion

        #region Update Methods

        Task<IResult> UpdateAsync(TaskUpdateDTO dto);
        Task<IResult> ChangeAssignTaskAsync(TaskAssignDTO dto);
        Task<IResult> ChangeStepTaskAsync(TaskStepDTO dto);

        #endregion

        #region Delete Methods

        Task<IResult> DeleteAsync(Guid id);
        Task<IResult> SoftDeleteAsync(Guid id);

        #endregion
    }
}
