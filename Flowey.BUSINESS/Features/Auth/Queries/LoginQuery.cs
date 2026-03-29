using Flowey.CORE.Interfaces.Repositories;
using Flowey.CORE.Interfaces.Services;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DOMAIN.Model.Concrete;
using Flowey.SHARED.Constants;
using Flowey.SHARED.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Flowey.BUSINESS.Features.Auth.Queries
{
    public class LoginQuery : IRequest<IDataResult<string>>
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public LoginQuery(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }

    public class LoginQueryHandler : IRequestHandler<LoginQuery, IDataResult<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher<User> _passwordHasher;

        public LoginQueryHandler(IUserRepository userRepository, ITokenService tokenService, IPasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
        }

        public async Task<IDataResult<string>> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.FirstOrDefaultAsync(x => x.Email == request.Email);

            if (user == null)
                return new DataResult<string>(ResultStatus.Error, string.Empty, Messages.InvalidCredentials);

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, request.Password);

            if (verificationResult != PasswordVerificationResult.Success)
                return new DataResult<string>(ResultStatus.Error, string.Empty, Messages.InvalidCredentials);

            string token = _tokenService.GenerateToken(user);
            return new DataResult<string>(ResultStatus.Success, token, Messages.LoginSuccessful);
        }
    }
}
