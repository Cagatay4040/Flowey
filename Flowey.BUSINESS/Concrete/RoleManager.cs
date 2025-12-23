using AutoMapper;
using Flowey.BUSINESS.Abstract;
using Flowey.BUSINESS.Constants;
using Flowey.BUSINESS.DTO.Role;
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
    public class RoleManager : IRoleService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public RoleManager(IProjectRepository projectRepository, IMapper mapper, ICurrentUserService currentUserService)
        {
            _projectRepository = projectRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        #region Get Methods

        public async Task<IDataResult<RoleGetDTO>> GetUserRole(Guid projectId)
        {
            var projectUser = await _projectRepository.GetProjectUserAsync(projectId, _currentUserService.GetUserId().Value);

            if (projectUser == null)
                return new DataResult<RoleGetDTO>(ResultStatus.Error, Messages.UserNotProjectMember, null);

            var userRole = _mapper.Map<RoleGetDTO>(projectUser.Role);

            return new DataResult<RoleGetDTO>(ResultStatus.Success, userRole);
        }

        #endregion

        #region Insert Methods



        #endregion

        #region Update Methods



        #endregion

        #region Delete Methods



        #endregion
    }
}
