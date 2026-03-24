using Flowey.CORE.DataAccess.Abstract;
using Flowey.CORE.Interfaces.Repositories;
using Flowey.CORE.Interfaces.Services;
using Flowey.CORE.Interfaces.UnitOfWork;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.SHARED.Constants;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Flowey.BUSINESS.Features.Users.Commands
{
    public class UpdateProfileImageCommand : IRequest<IDataResult<string>>
    {
        public IFormFile File { get; set; }

        public UpdateProfileImageCommand(IFormFile file)
        {
            File = file;
        }
    }

    public class UpdateProfileImageCommandHandler : IRequestHandler<UpdateProfileImageCommand, IDataResult<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IImageService _imageService;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProfileImageCommandHandler(IUserRepository userRepository, ICurrentUserService currentUserService, IImageService imageService, ITokenService tokenService, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _currentUserService = currentUserService;
            _imageService = imageService;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
        }

        public async Task<IDataResult<string>> Handle(UpdateProfileImageCommand request, CancellationToken cancellationToken)
        {
            if (request.File == null || request.File.Length == 0)
                return new DataResult<string>(ResultStatus.Error, Messages.ImageNotSelected, null);

            var extension = Path.GetExtension(request.File.FileName).ToLowerInvariant();

            if (!FileSettings.AllowedProfileImageExtensions.Contains(extension))
                return new DataResult<string>(ResultStatus.Error, Messages.InvalidImageFormat, null);

            if (request.File.Length > FileSettings.MaxFileSystemBytes)
                return new DataResult<string>(ResultStatus.Error, Messages.FileSizeExceeded, null);

            var user = await _userRepository.GetByIdAsync(_currentUserService.GetUserId().Value);

            if (user == null) 
                return new DataResult<string>(ResultStatus.Error, Messages.UserNotFound, null);

            var imageUrl = await _imageService.UploadImageAsync(request.File);
            if (string.IsNullOrEmpty(imageUrl))
                return new DataResult<string>(ResultStatus.Error, Messages.ImageUploadFailed, null);

            if (!string.IsNullOrEmpty(user.ProfileImageUrl))
                await _imageService.DeleteImageAsync(user.ProfileImageUrl);

            user.ProfileImageUrl = imageUrl;
            await _userRepository.UpdateAsync(user);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
            {
                var newToken = _tokenService.GenerateToken(user);
                return new DataResult<string>(ResultStatus.Success, Messages.ProfileImageUpdated, newToken);
            }

            return new DataResult<string>(ResultStatus.Error, Messages.ProfileImageUpdateFailed, null);
        }
    }
}
