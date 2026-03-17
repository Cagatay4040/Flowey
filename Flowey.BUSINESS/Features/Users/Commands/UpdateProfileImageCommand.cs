using Flowey.CORE.DataAccess.Abstract;
using Flowey.CORE.Interfaces.Repositories;
using Flowey.CORE.Interfaces.Services;
using Flowey.CORE.Interfaces.UnitOfWork;
using Flowey.CORE.Result.Concrete;
using Flowey.SHARED.Constants;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Flowey.BUSINESS.Features.Users.Commands
{
    public class UpdateProfileImageCommand : IRequest<CORE.Result.Abstract.IResult>
    {
        public IFormFile File { get; set; }

        public UpdateProfileImageCommand(IFormFile file)
        {
            File = file;
        }
    }

    public class UpdateProfileImageCommandHandler : IRequestHandler<UpdateProfileImageCommand, CORE.Result.Abstract.IResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IImageService _imageService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProfileImageCommandHandler(IUserRepository userRepository, ICurrentUserService currentUserService, IImageService imageService, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _currentUserService = currentUserService;
            _imageService = imageService;
            _unitOfWork = unitOfWork;
        }

        public async Task<CORE.Result.Abstract.IResult> Handle(UpdateProfileImageCommand request, CancellationToken cancellationToken)
        {
            if (request.File == null || request.File.Length == 0)
                return new Result(ResultStatus.Error, Messages.ImageNotSelected);

            var extension = Path.GetExtension(request.File.FileName).ToLowerInvariant();

            if (!FileSettings.AllowedProfileImageExtensions.Contains(extension))
                return new Result(ResultStatus.Error, Messages.InvalidImageFormat);

            if (request.File.Length > FileSettings.MaxFileSystemBytes)
                return new Result(ResultStatus.Error, Messages.FileSizeExceeded);

            var user = await _userRepository.GetByIdAsync(_currentUserService.GetUserId().Value);

            if (user == null) 
                return new Result(ResultStatus.Error, Messages.UserNotFound);

            var imageUrl = await _imageService.UploadImageAsync(request.File);
            if (string.IsNullOrEmpty(imageUrl))
                return new Result(ResultStatus.Error, Messages.ImageUploadFailed);

            if (!string.IsNullOrEmpty(user.ProfileImageUrl))
                await _imageService.DeleteImageAsync(user.ProfileImageUrl);

            user.ProfileImageUrl = imageUrl;
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return new Result(ResultStatus.Success, Messages.ProfileImageUpdated);
        }
    }
}
