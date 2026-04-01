using Flowey.CORE.Interfaces.Repositories;
using Flowey.CORE.Interfaces.Services;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.SHARED.Constants;
using Flowey.SHARED.Enums;
using MediatR;

namespace Flowey.BUSINESS.Features.Auth.Commands
{
    public class RefreshTokenCommand : IRequest<IDataResult<string>>
    {
    }

    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, IDataResult<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ITokenService _tokenService;

        public RefreshTokenCommandHandler(IUserRepository userRepository, ICurrentUserService currentUserService, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _currentUserService = currentUserService;
            _tokenService = tokenService;
        }

        public async Task<IDataResult<string>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(_currentUserService.GetUserId().Value);

            if (user == null)
                return new DataResult<string>(ResultStatus.Error, null, Messages.UserNotFound);

            var token = _tokenService.GenerateToken(user);
            return new DataResult<string>(ResultStatus.Success, token);
        }
    }
}
