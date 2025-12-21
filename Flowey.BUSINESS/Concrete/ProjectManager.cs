using AutoMapper;
using Flowey.BUSINESS.Abstract;
using Flowey.BUSINESS.Constants;
using Flowey.BUSINESS.DTO.Project;
using Flowey.BUSINESS.DTO.ProjectUser;
using Flowey.CORE.DataAccess.Abstract;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DATACCESS.Abstract;
using Flowey.DOMAIN.Model.Concrete;
using System;
using System.Collections.Generic;

using System.Linq;

namespace Flowey.BUSINESS.Concrete
{
    public class ProjectManager : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public ProjectManager(IProjectRepository projectRepository, IMapper mapper, ICurrentUserService currentUserService)
        {
            _projectRepository = projectRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        #region Get Methods

        public async Task<IDataResult<List<ProjectGetDTO>>> GetProjectsByLoginUserAsync()
        {
            var entityList = await _projectRepository.GetProjectsByLoginUserAsync(_currentUserService.GetUserId().Value);
            var data = _mapper.Map<List<ProjectGetDTO>>(entityList);
            return new DataResult<List<ProjectGetDTO>>(ResultStatus.Success, data);
        }

        public async Task<IDataResult<List<ProjectGetDTO>>> GetMyProjectsAsync()
        {
            var entityList = await _projectRepository.GetList(x => x.CreatedBy == _currentUserService.GetUserId().Value);
            var data = _mapper.Map<List<ProjectGetDTO>>(entityList);
            return new DataResult<List<ProjectGetDTO>>(ResultStatus.Success, data);
        }

        public async Task<ProjectUserGetDTO> GetProjectUserAsync(Guid projectId, Guid userId)
        {
            var entity = await _projectRepository.GetProjectUserAsync(projectId, userId);
            var data = _mapper.Map<ProjectUserGetDTO>(entity);
            return data;
        }

        #endregion

        #region Insert Methods

        public async Task<IResult> AddAsync(ProjectAddDTO dto)
        {
            var project = _mapper.Map<Project>(dto);

            int effectedRow = await _projectRepository.AddAsync(project);

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.ProjectAdded);

            return new Result(ResultStatus.Error, Messages.ProjectCreateError);
        }

        public async Task<IResult> AddWithCreatorAsync(ProjectAddDTO dto)
        {
            var project = _mapper.Map<Project>(dto);

            int effectedRow = await _projectRepository.AddWithCreatorAsync(project, _currentUserService.GetUserId().Value);

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.ProjectAdded);

            return new Result(ResultStatus.Error, Messages.ProjectCreateError);
        }

        public async Task<IResult> AddUserToProjectAsync(ProjectUserAddDTO dto)
        {
            var projectUser = _mapper.Map<ProjectUserRole>(dto);

            bool isExists = await _projectRepository.IsUserInProjectAsync(projectUser);
            if (isExists)
                return new Result(ResultStatus.Error, Messages.ProjectAlreadyAssignedToUser);

            int effectedRow = await _projectRepository.AddUserToProjectAsync(projectUser);

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.ProjectAssigned);

            return new Result(ResultStatus.Error, Messages.ProjectAssignError);
        }

        #endregion

        #region Update Methods

        public async Task<IResult> UpdateAsync(ProjectUpdateDTO dto)
        {
            var existingProject = await _projectRepository.GetByIdAsync(dto.Id);

            if (existingProject == null)
                return new Result(ResultStatus.Error, Messages.ProjectNotFound);

            _mapper.Map(dto, existingProject);

            int effectedRow = await _projectRepository.UpdateAsync(existingProject);

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.ProjectUpdated);

            return new Result(ResultStatus.Error, Messages.ProjectUpdateError);
        }

        #endregion

        #region Delete Methods

        public async Task<IResult> DeleteAsync(Guid id)
        {
            var existingProject = await _projectRepository.GetByIdAsync(id);

            if (existingProject == null)
                return new Result(ResultStatus.Error, Messages.ProjectNotFound);

            int effectedRow = await _projectRepository.DeleteAsync(existingProject);

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.ProjectDeleted);

            return new Result(ResultStatus.Error, Messages.ProjectDeleteError);
        }

        public async Task<IResult> SoftDeleteAsync(Guid id)
        {
            var existingProject = await _projectRepository.FirstOrDefaultAsync(x => x.Id == id);

            if (existingProject == null)
                return new Result(ResultStatus.Error, Messages.ProjectNotFound);

            int effectedRow = await _projectRepository.SoftDeleteAsync(existingProject);

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.ProjectDeleted);

            return new Result(ResultStatus.Error, Messages.ProjectDeleteError);
        }

        public async Task<IResult> RemoveUserFromProjectAsync(ProjectRemoveUserDTO dto)
        {
            var relation = await _projectRepository.GetProjectUserAsync(dto.ProjectId, dto.UserId);

            if (relation == null)
                return new Result(ResultStatus.Error, Messages.ProjectUserNotFound);

            int effectedRow = await _projectRepository.RemoveUserFromProjectAsync(relation);

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.ProjectRemoveUser);

            return new Result(ResultStatus.Error, Messages.ProjectRemoveUserError);
        }

        #endregion
    }
}
