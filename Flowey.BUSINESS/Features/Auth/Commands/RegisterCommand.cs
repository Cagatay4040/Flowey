using Flowey.CORE.Interfaces.Repositories;
using Flowey.CORE.Interfaces.UnitOfWork;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DOMAIN.Model.Concrete;
using Flowey.SHARED.Constants;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Flowey.BUSINESS.Features.Auth.Commands
{
    public class RegisterCommand : IRequest<IResult>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public RegisterCommand(string email, string password, string name, string surname)
        {
            Email = email;
            Password = password;
            Name = name;
            Surname = surname;
        }
    }

    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, IResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterCommandHandler(IUserRepository userRepository, IPasswordHasher<User> passwordHasher, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var user = new User
            {
                Email = request.Email,
                Name = request.Name,
                Surname= request.Surname,
            };

            user.Password = _passwordHasher.HashPassword(user, request.Password);

            if (await _userRepository.AnyAsync(x => x.Email == user.Email))
                return new Result(ResultStatus.Error, Messages.UserEmailAlreadyUsed);

            await _userRepository.AddAsync(user);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.UserAdded);

            return new Result(ResultStatus.Error, Messages.UserCreateError);
        }
    }
}
