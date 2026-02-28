using AutoMapper;
using Flowey.BUSINESS.Abstract;
using Flowey.CORE.Constants;
using Flowey.BUSINESS.DTO.Project;
using Flowey.BUSINESS.DTO.ProjectUser;
using Flowey.BUSINESS.DTO.User;
using Flowey.CORE.DataAccess.Abstract;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DATACCESS.Abstract;
using Flowey.DOMAIN.Model.Concrete;
using System;
using System.Collections.Generic;

using System.Linq;
using Flowey.CORE.Enums;

namespace Flowey.BUSINESS.Concrete
{
    public class ProjectManager : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public ProjectManager(IProjectRepository projectRepository, IMapper mapper, ICurrentUserService currentUserService, IUnitOfWork unitOfWork)
        {
            _projectRepository = projectRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        #region Get Methods

        public async Task<IDataResult<List<ProjectGetDTO>>> GetProjectsByLoginUserAsync()
        {
            var entityList = await _projectRepository.GetUserProjectMembershipsAsync(_currentUserService.GetUserId().Value);
            var data = _mapper.Map<List<ProjectGetDTO>>(entityList);
            return new DataResult<List<ProjectGetDTO>>(ResultStatus.Success, data);
        }

        public async Task<IDataResult<List<ProjectGetDTO>>> GetMyProjectsAsync()
        {
            var entityList = await _projectRepository.GetUserProjectMembershipsAsync(_currentUserService.GetUserId().Value, RoleType.Admin);
            var data = _mapper.Map<List<ProjectGetDTO>>(entityList);
            return new DataResult<List<ProjectGetDTO>>(ResultStatus.Success, data);
        }

        public async Task<IDataResult<UserSelectDTO>> GetProjectUserAsync(Guid projectId, Guid userId)
        {
            var entity = await _projectRepository.GetProjectUserAsync(projectId, userId);

            var data = new UserSelectDTO
            {
                Id = entity.User.Id,
                FullName = $"{entity.User.Name} {entity.User.Surname}",
                Email = entity.User.Email
            };

            return new DataResult<UserSelectDTO>(ResultStatus.Success, data);
        }

        public async Task<IDataResult<List<UserSelectDTO>>> GetProjectUsersAsync(Guid projectId)
        {
            var entities = await _projectRepository.GetProjectUsersAsync(projectId);

            var data = entities.Select(role => new UserSelectDTO
            {
                Id = role.User.Id,
                FullName = $"{role.User.Name} {role.User.Surname}",
                Email = role.User.Email
            }).ToList();

            return new DataResult<List<UserSelectDTO>>(ResultStatus.Success, data);
        }

        #endregion

        #region Insert Methods

        public async Task<IResult> AddAsync(ProjectAddDTO dto)
        {
            var project = _mapper.Map<Project>(dto);

            await _projectRepository.AddAsync(project);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.ProjectAdded);

            return new Result(ResultStatus.Error, Messages.ProjectCreateError);
        }

        public async Task<IResult> AddWithCreatorAsync(ProjectAddDTO dto)
        {
            var project = _mapper.Map<Project>(dto);

            await _projectRepository.AddWithCreatorAsync(project, _currentUserService.GetUserId().Value);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

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

            await _projectRepository.AddUserToProjectAsync(projectUser);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

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

            await _projectRepository.UpdateAsync(existingProject);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

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

            await _projectRepository.DeleteAsync(existingProject);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.ProjectDeleted);

            return new Result(ResultStatus.Error, Messages.ProjectDeleteError);
        }

        public async Task<IResult> SoftDeleteAsync(Guid id)
        {
            var existingProject = await _projectRepository.FirstOrDefaultAsync(x => x.Id == id);

            if (existingProject == null)
                return new Result(ResultStatus.Error, Messages.ProjectNotFound);

            await _projectRepository.SoftDeleteAsync(existingProject);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.ProjectDeleted);

            return new Result(ResultStatus.Error, Messages.ProjectDeleteError);
        }

        public async Task<IResult> RemoveUserFromProjectAsync(ProjectRemoveUserDTO dto)
        {
            var relation = await _projectRepository.GetProjectUserAsync(dto.ProjectId, dto.UserId);

            if (relation == null)
                return new Result(ResultStatus.Error, Messages.ProjectUserNotFound);

            await _projectRepository.RemoveUserFromProjectAsync(relation);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.ProjectRemoveUser);

            return new Result(ResultStatus.Error, Messages.ProjectRemoveUserError);
        }

        #endregion
    }
}
