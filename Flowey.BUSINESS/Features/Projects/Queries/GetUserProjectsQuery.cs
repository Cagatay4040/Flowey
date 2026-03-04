using Flowey.BUSINESS.DTO.Project;
using Flowey.CORE.DataAccess.Abstract;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DATACCESS.Abstract;
using MediatR;
using AutoMapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.Features.Projects.Queries
{
    public class GetUserProjectsQuery : IRequest<IDataResult<List<ProjectGetDTO>>>
    {
    }

    public class GetUserProjectsQueryHandler : IRequestHandler<GetUserProjectsQuery, IDataResult<List<ProjectGetDTO>>>
    {
        private readonly IEntityRepository<Flowey.DOMAIN.Model.Concrete.ProjectUserRole> _projectUserRoleRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public GetUserProjectsQueryHandler(
            IEntityRepository<Flowey.DOMAIN.Model.Concrete.ProjectUserRole> projectUserRoleRepository,
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
