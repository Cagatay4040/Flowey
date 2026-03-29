using Flowey.CORE.Interfaces.Repositories;
using Flowey.CORE.Interfaces.UnitOfWork;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.SHARED.Constants;
using Flowey.SHARED.Enums;
using MediatR;

namespace Flowey.BUSINESS.Features.Users.Commands
{
    public class UpdateUserCommand : IRequest<IResult>
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public UpdateUserCommand(Guid userId, string email, string name, string surname)
        {
            UserId = userId;
            Email = email;
            Name = name;
            Surname = surname;
        }
    }

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, IResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetByIdAsync(request.UserId);

            if (existingUser == null)
                return new Result(ResultStatus.Error, Messages.UserNotFound);

            existingUser.Email = request.Email;
            existingUser.Name = request.Name;
            existingUser.Surname = request.Surname;

            await _userRepository.UpdateAsync(existingUser);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.UserUpdated);

            return new Result(ResultStatus.Error, Messages.UserUpdateError);
        }
    }
}
