using Flowey.CORE.DataAccess.Abstract;
using Flowey.CORE.Interfaces.Repositories;
using Flowey.CORE.Interfaces.Services;
using Flowey.CORE.Interfaces.UnitOfWork;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.SHARED.Constants;
using MediatR;

namespace Flowey.BUSINESS.Features.Users.Commands
{
    public class DeleteProfileImageCommand : IRequest<IResult>
    {
    }

    public class DeleteProfileImageCommandHandler : IRequestHandler<DeleteProfileImageCommand, IResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IImageService _imageService;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteProfileImageCommandHandler(IUserRepository userRepository, ICurrentUserService currentUserService, IImageService imageService, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _currentUserService = currentUserService;
            _imageService = imageService;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult> Handle(DeleteProfileImageCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(_currentUserService.GetUserId().Value);

            if (user == null)
                return new Result(ResultStatus.Error, Messages.UserNotFound);

            if (string.IsNullOrEmpty(user.ProfileImageUrl))
                return new Result(ResultStatus.Error, Messages.NoProfileImageToDelete);

            var isDeleted = await _imageService.DeleteImageAsync(user.ProfileImageUrl);

            if (isDeleted)
            {
                user.ProfileImageUrl = null;
                await _userRepository.UpdateAsync(user);
                int effectedRow = await _unitOfWork.SaveChangesAsync();

                if (effectedRow > 0)
                    return new Result(ResultStatus.Success, Messages.ProfileImageDeleted);
            }

            return new Result(ResultStatus.Error, Messages.ProfileImageDeleteFailed);
        }
    }
}
