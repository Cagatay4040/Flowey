using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Flowey.BUSINESS.Abstract;
using Flowey.BUSINESS.Constants;
using Flowey.BUSINESS.DTO.User;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DOMAIN.Model.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Flowey.BUSINESS.Concrete
{
    public class AuthManager : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IUserService _userService;

        public AuthManager(IConfiguration configuration, IPasswordHasher<User> passwordHasher, IUserService userService)
        {
            _configuration = configuration;
            _passwordHasher = passwordHasher;
            _userService = userService;
        }

        #region Get Methods

        public string GetToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
            };

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
                return new DataResult<string>(ResultStatus.Error, Messages.InvalidCredentials);

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, dto.Password);

            if (verificationResult != PasswordVerificationResult.Success)
                return new DataResult<string>(ResultStatus.Error, Messages.InvalidCredentials);

            string token = GetToken(user);
            return new DataResult<string>(ResultStatus.Success, Messages.LoginSuccessful, token);
        }

        #endregion

        #region Insert Methods



        #endregion

        #region Update Methods



        #endregion

        #region Delete Methods



        #endregion
    }
}
