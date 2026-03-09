using AutoMapper;
using Flowey.BUSINESS.Abstract;
using Flowey.CORE.Constants;
using Flowey.BUSINESS.DTO.User;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DATACCESS.Abstract;
using Flowey.DOMAIN.Model.Concrete;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Flowey.BUSINESS.Concrete
{
    public class UserManager : IUserService, IInternalUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public UserManager(IMapper mapper, IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        #region Get Methods

        async Task<User> IInternalUserService.GetUserByIdAsync(Guid id)
        {
            var userEntity = await _userRepository.FirstOrDefaultAsync(x => x.Id == id);
            return userEntity;
        }

        async Task<User> IInternalUserService.GetUserByEmailAsync(string email)
        {
            var userEntity = await _userRepository.FirstOrDefaultAsync(x => x.Email == email);
            return userEntity;
        }

        public async Task<bool> IsThisEmailUsedAsync(string email)
        {
            return await _userRepository.AnyAsync(x => x.Email == email);
        }

        public async Task<List<UserSelectDTO>> SearchUsersAsync(string searchTerm)
        {
            var term = searchTerm.Trim().ToLower();

            var users = await _userRepository.
                GetQueryable(u =>  u.Name.ToLower().Contains(term) ||
                        u.Surname.ToLower().Contains(term) ||
                        u.Email.ToLower().Contains(term),
                        true
                        )
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Surname)
                .Select(u => new UserSelectDTO 
                {
                    Id = u.Id,
                    FullName = $"{u.Name} {u.Surname}", 
                    Email = u.Email,
                })
                .Take(10)
                .ToListAsync();

            return users;
        }

        #endregion

        #region Insert Methods

        async Task<IResult> IInternalUserService.AddAsync(User user)
        {
            if (await IsThisEmailUsedAsync(user.Email))
                return new Result(ResultStatus.Error, Messages.UserEmailAlreadyUsed);

            await _userRepository.AddAsync(user);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.UserAdded);

            return new Result(ResultStatus.Error, Messages.UserCreateError);
        }

        #endregion

        #region Update Methods

        public async Task<IResult> UpdateAsync(UserUpdateDTO dto)
        {
            var existingUser = await _userRepository.GetByIdAsync(dto.Id);

            if (existingUser == null)
                return new Result(ResultStatus.Error, Messages.UserNotFound);

            _mapper.Map(dto, existingUser);

            return await ((IInternalUserService)this).UpdateAsync(existingUser);
        }

        async Task<IResult> IInternalUserService.UpdateAsync(User user)
        {
            await _userRepository.UpdateAsync(user);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.UserUpdated);

            return new Result(ResultStatus.Error, Messages.UserUpdateError);
        }

        #endregion

        #region Delete Methods

        public async Task<IResult> SoftDeleteAsync(UserDeleteDTO dto)
        {
            var existingUser = await _userRepository.GetByIdAsync(dto.Id);

            if (existingUser == null)
                return new Result(ResultStatus.Error, Messages.UserNotFound);

            _mapper.Map(dto, existingUser);

            await _userRepository.SoftDeleteAsync(existingUser);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.UserDeleted);

            return new Result(ResultStatus.Error, Messages.UserDeleteError);
        }

        #endregion
    }
}
