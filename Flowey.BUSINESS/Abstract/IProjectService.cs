using Flowey.BUSINESS.DTO.Project;
using Flowey.BUSINESS.DTO.ProjectUser;
using Flowey.CORE.Result.Abstract;
using System.Collections.Generic;

namespace Flowey.BUSINESS.Abstract
{
    public interface IProjectService
    {
        #region Get Methods

        Task<IDataResult<List<ProjectGetDTO>>> GetProjectsByLoginUserAsync();
        Task<IDataResult<List<ProjectGetDTO>>> GetMyProjectsAsync();
        Task<ProjectUserGetDTO> GetProjectUserAsync(Guid projectId, Guid userId);

        #endregion

        #region Insert Methods

        Task<IResult> AddAsync(ProjectAddDTO dto);
        Task<IResult> AddWithCreatorAsync(ProjectAddDTO dto);
        Task<IResult> AddUserToProjectAsync(ProjectUserAddDTO dto);

        #endregion

        #region Update Methods

        Task<IResult> UpdateAsync(ProjectUpdateDTO dto);

        #endregion

        #region Delete Methods

        Task<IResult> DeleteAsync(Guid id);
        Task<IResult> SoftDeleteAsync(Guid id);
        Task<IResult> RemoveUserFromProjectAsync(ProjectRemoveUserDTO dto);

        #endregion
    }
}
