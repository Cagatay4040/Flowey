using Flowey.CORE.Interfaces.Repositories;
using Flowey.CORE.Interfaces.Services;
using Flowey.CORE.Interfaces.UnitOfWork;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.SHARED.Constants;
using Flowey.SHARED.Enums;
using MediatR;

namespace Flowey.BUSINESS.Features.Users.Commands
{
    public class DeleteProfileImageCommand : IRequest<IDataResult<string>>
    {
    }

    public class DeleteProfileImageCommandHandler : IRequestHandler<DeleteProfileImageCommand, IDataResult<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IImageService _imageService;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteProfileImageCommandHandler(IUserRepository userRepository, ICurrentUserService currentUserService, IImageService imageService, ITokenService tokenService, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _currentUserService = currentUserService;
            _imageService = imageService;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
        }

        public async Task<IDataResult<string>> Handle(DeleteProfileImageCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(_currentUserService.GetUserId().Value);

            if (user == null)
                return new DataResult<string>(ResultStatus.Error, null, Messages.UserNotFound);

            if (string.IsNullOrEmpty(user.ProfileImageUrl))
                return new DataResult<string>(ResultStatus.Error, null, Messages.NoProfileImageToDelete);

            var isDeleted = await _imageService.DeleteImageAsync(user.ProfileImageUrl);

            if (isDeleted)
            {
                user.ProfileImageUrl = null;
                await _userRepository.UpdateAsync(user);
                int effectedRow = await _unitOfWork.SaveChangesAsync();

                if (effectedRow > 0)
                {
                    var newToken = _tokenService.GenerateToken(user);
                    return new DataResult<string>(ResultStatus.Success, newToken, Messages.ProfileImageDeleted);
                }
            }

            return new DataResult<string>(ResultStatus.Error, null, Messages.ProfileImageDeleteFailed);
        }
    }
}
