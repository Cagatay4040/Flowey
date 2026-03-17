using Flowey.CORE.DTO.Task;
using Flowey.CORE.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowey.BUSINESS.Features.Tasks.Queries
{
    public class SearchProjectTasksQuery : IRequest<List<TaskSearchDTO>>
    {
        public string SearchTerm { get; set; }

        public SearchProjectTasksQuery(string searchTerm)
        {
            SearchTerm = searchTerm;
        }
    }

    public class SearchProjectTasksQueryHandler : IRequestHandler<SearchProjectTasksQuery, List<TaskSearchDTO>>
    {
        private readonly ITaskRepository _taskRepository;

        public SearchProjectTasksQueryHandler(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<List<TaskSearchDTO>> Handle(SearchProjectTasksQuery request, CancellationToken cancellationToken)
        {
            var term = request.SearchTerm.Trim().ToLower();

            var tasks = await _taskRepository.
                GetQueryable(u => u.TaskKey.ToLower().Contains(term) ||
                        u.Title.ToLower().Contains(term),
                        true
                        )
                .OrderBy(x => x.TaskKey)
                .ThenBy(x => x.Title)
                .Select(u => new TaskSearchDTO
                {
                    Id = u.Id,
                    TaskKey = u.TaskKey,
                    Title = u.Title
                })
                .Take(10)
                .ToListAsync();

            return tasks;
        }
    }
}
