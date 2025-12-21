using AutoMapper;
using Flowey.BUSINESS.Abstract;
using Flowey.BUSINESS.Constants;
using Flowey.BUSINESS.DTO.User;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DATACCESS.Abstract;
using Flowey.DATACCESS.Concrete;
using Flowey.DOMAIN.Model.Concrete;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Flowey.BUSINESS.Concrete
{
    public class UserManager : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserManager(IMapper mapper, IPasswordHasher<User> passwordHasher, IUserRepository userRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        #region Get Methods

        public async Task<User> GetUserByEmailAsync(string email)
        {
            var userEntity = await _userRepository.FirstOrDefaultAsync(x => x.Email == email);
            return userEntity;
        }

        public async Task<bool> IsThisEmailUsedAsync(string email)
        {
            return await _userRepository.AnyAsync(x => x.Email == email);
        }

        #endregion

        #region Insert Methods

        public async Task<IResult> AddAsync(UserAddDTO dto)
        {
            if (!await IsThisEmailUsedAsync(dto.Email))
            {
                var user = _mapper.Map<User>(dto);
                user.Password = _passwordHasher.HashPassword(user, dto.Password);

                int effectedRow = await _userRepository.AddAsync(user);
                
                if (effectedRow > 0)
                    return new Result(ResultStatus.Success, Messages.UserAdded);

                return new Result(ResultStatus.Error, Messages.UserCreateError);
            }

            else
            {
                return new Result(ResultStatus.Error, Messages.UserEmailAlreadyUsed);
            }
        }

        #endregion

        #region Update Methods


        public async Task<IResult> UpdateAsync(UserUpdateDTO dto)
        {
            var existingUser = await _userRepository.GetByIdAsync(dto.Id);

            if (existingUser == null)
                return new Result(ResultStatus.Error, Messages.UserNotFound);

            _mapper.Map(dto, existingUser);

            int effectedRow = await _userRepository.UpdateAsync(existingUser);
            
            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.UserUpdated);

            return new Result(ResultStatus.Error, Messages.UserUpdateError);
        }

        public async Task<IResult> ChangePasswordAsync(UserPasswordChangeDTO dto)
        {
            var user = await _userRepository.GetByIdAsync(dto.UserId);

            if (user == null)
                return new Result(ResultStatus.Error, Messages.UserNotFound);

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, dto.OldPassword);

            if (verificationResult == PasswordVerificationResult.Failed)
                return new Result(ResultStatus.Error, Messages.UserOldPasswordIncorrect);

            string newPasswordHash = _passwordHasher.HashPassword(user, dto.NewPassword);

            user.Password = newPasswordHash;

            int effectedRow = await _userRepository.UpdateAsync(user);

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.UserPasswordChangeSuccess);

            return new Result(ResultStatus.Error, Messages.UserPasswordUpdateFailed);
        }

        #endregion

        #region Delete Methods

        public async Task<IResult> SoftDeleteAsync(UserDeleteDTO dto)
        {
            var existingUser = await _userRepository.GetByIdAsync(dto.Id);

            if (existingUser == null)
                return new Result(ResultStatus.Error, Messages.UserNotFound);

            _mapper.Map(dto, existingUser);

            int effectedRow = await _userRepository.SoftDeleteAsync(existingUser);

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.UserDeleted);

            return new Result(ResultStatus.Error, Messages.UserDeleteError);
        }   

        #endregion
    }
}
