using Flowey.CORE.DTO.User;
using Flowey.CORE.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowey.BUSINESS.Features.Users.Queries
{
    public class SearchUsersQuery : IRequest<List<UserSelectDTO>>
    {
        public string SearchTerm { get; set; }

        public SearchUsersQuery(string searchTerm)
        {
            SearchTerm = searchTerm;
        }
    }

    public class SearchUsersQueryHandler : IRequestHandler<SearchUsersQuery, List<UserSelectDTO>>
    {
        private readonly IUserRepository _userRepository;

        public SearchUsersQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<UserSelectDTO>> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.SearchTerm))
                return new List<UserSelectDTO>();

            var term = request.SearchTerm.Trim().ToLower();

            var users = await _userRepository.
                GetQueryable(u => u.Name.ToLower().Contains(term) ||
                        u.Surname.ToLower().Contains(term) ||
                        u.Email.ToLower().Contains(term),
                        true
                        )
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Surname)
                .Select(u => new UserSelectDTO
                {
                    Id = u.Id,
                    FullName = $"{u.Name} {u.Surname}",
                    Email = u.Email,
                    ProfileImageUrl = u.ProfileImageUrl
                })
                .Take(10)
                .ToListAsync();

            return users;
        }
    }
}
