using AutoMapper;
using Flowey.BUSINESS.Abstract;
using Flowey.BUSINESS.DTO.Notification;
using Flowey.CORE.Constants;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DATACCESS.Abstract;
using Flowey.DOMAIN.Model.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.Concrete
{
    public class UserNotificationManager : IUserNotificationService
    {
        private readonly IUserNotificationRepository _userNotificationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IRealTimeNotificationService _realTimeNotificationService;
        private readonly IUnitOfWork _unitOfWork;

        public UserNotificationManager(IUserNotificationRepository userNotificationRepository, IUserRepository userRepository, IMapper mapper, IRealTimeNotificationService realTimeNotificationService, IUnitOfWork unitOfWork)
        {
            _userNotificationRepository = userNotificationRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _realTimeNotificationService = realTimeNotificationService;
            _unitOfWork = unitOfWork;
        }


        #region Get Methods

        public async Task<IDataResult<List<UserNotificationGetDTO>>> GetUserNotificationsAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                return new DataResult<List<UserNotificationGetDTO>>(ResultStatus.Error, Messages.UserNotFound, new List<UserNotificationGetDTO>());

            var entityList = await _userNotificationRepository.GetUserNotifications(userId);
            var data = _mapper.Map<List<UserNotificationGetDTO>>(entityList);
            return new DataResult<List<UserNotificationGetDTO>>(ResultStatus.Success, data);
        }

        #endregion

        #region Insert Methods

        public async Task<IResult> AddUserNotificationAsync(UserNotificationAddDTO dto)
        {
            var user = await _userRepository.GetByIdAsync(dto.UserId);

            if (user == null)
                return new Result(ResultStatus.Error, Messages.UserNotFound);

            var userNotification = _mapper.Map<UserNotification>(dto);

            await _userNotificationRepository.AddAsync(userNotification);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
            {
                await _realTimeNotificationService.SendNotificationAsync(dto);
                return new Result(ResultStatus.Success, Messages.UserNotificationAdded);
            }

            return new Result(ResultStatus.Error, Messages.UserNotificationCreateError);
        }

        #endregion

        #region Update Methods

        public async Task<IResult> MarkAsReadAsync(Guid notificationId)
        {
            var notification = await _userNotificationRepository.GetByIdAsync(notificationId);

            if (notification == null)
                return new Result(ResultStatus.Error, Messages.UserNotificationNotFound);

            notification.IsRead = true;

            await _userNotificationRepository.UpdateAsync(notification);
            int effectedRow = await _unitOfWork.SaveChangesAsync();
            
            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.UserNotificationUpdated);

            return new Result(ResultStatus.Error, Messages.UserNotificationUpdateError);
        }

        public async Task<IResult> MarkAllAsReadAsync(Guid userId)
        {
            var unreadNotifications = await _userNotificationRepository.GetList(x => x.UserId == userId && !x.IsRead);

            if (unreadNotifications == null || !unreadNotifications.Any())
                return new Result(ResultStatus.Success, Messages.NoUnreadNotifications);

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
            }

            await _userNotificationRepository.UpdateRangeAsync(unreadNotifications);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.UserNotificationUpdated);

            return new Result(ResultStatus.Error, Messages.UserNotificationUpdateError);
        }

        #endregion

        #region Delete Methods



        #endregion
    }
}
