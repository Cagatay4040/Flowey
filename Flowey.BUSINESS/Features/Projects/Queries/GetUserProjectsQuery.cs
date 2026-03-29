using AutoMapper;
using Flowey.CORE.DataAccess.Abstract;
using Flowey.CORE.DTO.Project;
using Flowey.CORE.Interfaces.Repositories;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DOMAIN.Model.Concrete;
using Flowey.SHARED.Enums;
using MediatR;

namespace Flowey.BUSINESS.Features.Projects.Queries
{
    public class GetUserProjectsQuery : IRequest<IDataResult<List<ProjectGetDTO>>>
    {
    }

    public class GetUserProjectsQueryHandler : IRequestHandler<GetUserProjectsQuery, IDataResult<List<ProjectGetDTO>>>
    {
        private readonly IEntityRepository<ProjectUserRole> _projectUserRoleRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public GetUserProjectsQueryHandler(
            IEntityRepository<ProjectUserRole> projectUserRoleRepository,
            IMapper mapper,
            ICurrentUserService currentUserService)
        {
            _projectUserRoleRepository = projectUserRoleRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<IDataResult<List<ProjectGetDTO>>> Handle(GetUserProjectsQuery request, CancellationToken cancellationToken)
        {
            var entityList = await _projectUserRoleRepository.GetList(
                x => x.UserId == _currentUserService.GetUserId().Value, 
                true, 
                null, 
                x => x.Project);
            
            var data = _mapper.Map<List<ProjectGetDTO>>(entityList);
            return new DataResult<List<ProjectGetDTO>>(ResultStatus.Success, data);
        }
    }
}
