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
    public class ChangePasswordCommand : IRequest<IResult>
    {
        public Guid UserId { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string NewPasswordConfirm { get; set; }

        public ChangePasswordCommand(Guid userId, string oldPassword, string newPassword, string newPasswordConfirm)
        {
            UserId = userId;
            OldPassword = oldPassword;
            NewPassword = newPassword;
            NewPasswordConfirm = newPasswordConfirm;
        }
    }

    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, IResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;

        public ChangePasswordCommandHandler(IUserRepository userRepository, IPasswordHasher<User> passwordHasher, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);

            if (user == null)
                return new Result(ResultStatus.Error, Messages.UserNotFound);

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, request.OldPassword);

            if (verificationResult == PasswordVerificationResult.Failed)
                return new Result(ResultStatus.Error, Messages.UserOldPasswordIncorrect);

            user.Password = _passwordHasher.HashPassword(user, request.NewPassword);

            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.UserPasswordChangeSuccess);

            return new Result(ResultStatus.Error, Messages.PasswordChangeFailed);
        }
    }
}
