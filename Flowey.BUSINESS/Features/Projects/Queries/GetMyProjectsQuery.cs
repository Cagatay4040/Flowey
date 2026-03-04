using Flowey.BUSINESS.DTO.Project;
using Flowey.CORE.DataAccess.Abstract;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DATACCESS.Abstract;
using Flowey.CORE.Enums;
using MediatR;
using AutoMapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.Features.Projects.Queries
{
    public class GetMyProjectsQuery : IRequest<IDataResult<List<ProjectGetDTO>>>
    {
    }

    public class GetMyProjectsQueryHandler : IRequestHandler<GetMyProjectsQuery, IDataResult<List<ProjectGetDTO>>>
    {
        private readonly IEntityRepository<Flowey.DOMAIN.Model.Concrete.ProjectUserRole> _projectUserRoleRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public GetMyProjectsQueryHandler(
            IEntityRepository<Flowey.DOMAIN.Model.Concrete.ProjectUserRole> projectUserRoleRepository,
            IMapper mapper,
            ICurrentUserService currentUserService)
        {
            _projectUserRoleRepository = projectUserRoleRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<IDataResult<List<ProjectGetDTO>>> Handle(GetMyProjectsQuery request, CancellationToken cancellationToken)
        {
            var entityList = await _projectUserRoleRepository.GetList(
                x => x.UserId == _currentUserService.GetUserId().Value && x.RoleId == RoleType.Admin, 
                true, 
                null, 
                x => x.Project);
                
            var data = _mapper.Map<List<ProjectGetDTO>>(entityList);
            return new DataResult<List<ProjectGetDTO>>(ResultStatus.Success, data);
        }
    }
}
