using Flowey.BUSINESS.DTO.Project;
using Flowey.BUSINESS.DTO.ProjectUser;
using Flowey.BUSINESS.DTO.User;
using Flowey.CORE.Result.Abstract;
using System.Collections.Generic;

namespace Flowey.BUSINESS.Abstract
{
    public interface IProjectService
    {
        #region Get Methods

        Task<IDataResult<List<ProjectGetDTO>>> GetProjectsByLoginUserAsync();
        Task<IDataResult<List<ProjectGetDTO>>> GetMyProjectsAsync();
        Task<IDataResult<UserSelectDTO>> GetProjectUserAsync(Guid projectId, Guid userId);
        Task<IDataResult<List<UserSelectDTO>>> GetProjectUsersAsync(Guid projectId);

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
