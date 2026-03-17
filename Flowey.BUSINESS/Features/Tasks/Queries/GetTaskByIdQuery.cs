using AutoMapper;
using Flowey.CORE.DTO.Task;
using Flowey.CORE.Interfaces.Repositories;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using MediatR;

namespace Flowey.BUSINESS.Features.Tasks.Queries
{
    public class GetTaskByIdQuery : IRequest<IDataResult<TaskGetDTO>>
    {
        public Guid Id { get; set; }

        public GetTaskByIdQuery(Guid id)
        {
            Id = id;
        }
    }

    public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, IDataResult<TaskGetDTO>>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IMapper _mapper;

        public GetTaskByIdQueryHandler(ITaskRepository taskRepository, IMapper mapper)
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
        }

        public async Task<IDataResult<TaskGetDTO>> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _taskRepository.FirstOrDefaultAsync(x => x.Id == request.Id);
            var data = _mapper.Map<TaskGetDTO>(entity);
            return new DataResult<TaskGetDTO>(ResultStatus.Success, data);
        }
    }
}
