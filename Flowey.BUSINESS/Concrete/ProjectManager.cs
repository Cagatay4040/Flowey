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
using Microsoft.EntityFrameworkCore;

namespace Flowey.BUSINESS.Concrete
{
    public class ProjectManager : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IEntityRepository<ProjectUserRole> _projectUserRoleRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public ProjectManager(IProjectRepository projectRepository, IEntityRepository<ProjectUserRole> projectUserRoleRepository, IMapper mapper, ICurrentUserService currentUserService, IUnitOfWork unitOfWork)
        {
            _projectRepository = projectRepository;
            _projectUserRoleRepository = projectUserRoleRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        #region Get Methods

        public async Task<IDataResult<List<ProjectGetDTO>>> GetProjectsByLoginUserAsync()
        {
            var entityList = await _projectUserRoleRepository.GetList(x => x.UserId == _currentUserService.GetUserId().Value, true, null, x => x.Project, x => x.Role);
            var data = _mapper.Map<List<ProjectGetDTO>>(entityList);
            return new DataResult<List<ProjectGetDTO>>(ResultStatus.Success, data);
        }

        public async Task<IDataResult<List<ProjectGetDTO>>> GetMyProjectsAsync()
        {
            var entityList = await _projectUserRoleRepository.GetList(x => x.UserId == _currentUserService.GetUserId().Value && x.RoleId == (int)RoleType.Admin, true, null, x => x.Project, x => x.Role);
            var data = _mapper.Map<List<ProjectGetDTO>>(entityList);
            return new DataResult<List<ProjectGetDTO>>(ResultStatus.Success, data);
        }

        public async Task<IDataResult<List<UserSelectDTO>>> GetProjectUsersAsync(Guid projectId)
        {
            var entities = await _projectRepository.GetProjectWithUsersAsync(projectId);

            if(entities == null)
                return new DataResult<List<UserSelectDTO>>(ResultStatus.Error, Messages.ProjectNotFound, new List<UserSelectDTO>());

            var data = entities.ProjectUserRoles.Select(role => new UserSelectDTO
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
            var newProject = new Project
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                ProjectKey = dto.ProjectKey,
                ProjectUserRoles = new List<ProjectUserRole>
                {
                    new ProjectUserRole
                    {
                        UserId = _currentUserService.GetUserId().Value,
                        RoleId = (int)RoleType.Admin
                    }
                }
            };

            await _projectRepository.AddAsync(newProject);

            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.ProjectAdded);

            return new Result(ResultStatus.Error, Messages.ProjectCreateError);
        }

        public async Task<IResult> AddUserToProjectAsync(ProjectUserAddDTO dto)
        {
            var projectUser = _mapper.Map<ProjectUserRole>(dto);

            bool isExists = await _projectRepository.IsUserInProjectAsync(projectUser.ProjectId, projectUser.UserId);
            if (isExists)
                return new Result(ResultStatus.Error, Messages.ProjectAlreadyAssignedToUser);

            await _projectUserRoleRepository.AddAsync(projectUser);
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
            var relation = await _projectUserRoleRepository.FirstOrDefaultAsync(x => x.ProjectId == dto.ProjectId && x.UserId == dto.UserId);

            if (relation == null)
                return new Result(ResultStatus.Error, Messages.ProjectUserNotFound);

            await _projectUserRoleRepository.SoftDeleteAsync(relation);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.ProjectRemoveUser);

            return new Result(ResultStatus.Error, Messages.ProjectRemoveUserError);
        }

        #endregion
    }
}
