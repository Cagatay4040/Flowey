using AutoMapper;
using Flowey.BUSINESS.Abstract;
using Flowey.BUSINESS.DTO.User;
using Flowey.CORE.Constants;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DOMAIN.Model.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.Concrete
{
    public class AuthManager : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IInternalUserService _userService;
        private readonly IMapper _mapper;

        public AuthManager(IConfiguration configuration, IPasswordHasher<User> passwordHasher, IInternalUserService userService, IMapper mapper)
        {
            _configuration = configuration;
            _passwordHasher = passwordHasher;
            _userService = userService;
            _mapper = mapper;
        }

        #region Get Methods

        public string GetToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Surname, user.Surname),
            };

            if (user.PremiumExpirationDate.HasValue)
                claims.Add(new Claim("PremiumExpireDate", user.PremiumExpirationDate.Value.ToString("o")));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthConfig:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenString;
        }

        public async Task<IDataResult<string>> LoginAsync(UserLoginDTO dto)
        {
            var user = await _userService.GetUserByEmailAsync(dto.Email);

            if (user == null)
                return new DataResult<string>(ResultStatus.Error, Messages.InvalidCredentials, string.Empty);

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, dto.Password);

            if (verificationResult != PasswordVerificationResult.Success)
                return new DataResult<string>(ResultStatus.Error, Messages.InvalidCredentials, string.Empty);

            string token = GetToken(user);
            return new DataResult<string>(ResultStatus.Success, Messages.LoginSuccessful, token);
        }

        #endregion

        #region Insert Methods

        public async Task<IResult> RegisterAsync(UserAddDTO dto)
        {
            var user = _mapper.Map<User>(dto);
            user.Password = _passwordHasher.HashPassword(user, dto.Password);

            return await _userService.AddAsync(user);
        }

        #endregion

        #region Update Methods

        public async Task<IResult> ChangePasswordAsync(UserPasswordChangeDTO dto)
        {
            var user = await _userService.GetUserByIdAsync(dto.UserId);

            if (user == null)
                return new Result(ResultStatus.Error, Messages.UserNotFound);

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, dto.OldPassword);

            if (verificationResult == PasswordVerificationResult.Failed)
                return new Result(ResultStatus.Error, Messages.UserOldPasswordIncorrect);

            user.Password = _passwordHasher.HashPassword(user, dto.NewPassword);

            return await _userService.UpdateAsync(user);
        }

        #endregion

        #region Delete Methods



        #endregion
    }
}
