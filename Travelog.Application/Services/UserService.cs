using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Travelog.Core;
using Travelog.Core.Abstractions;
using Travelog.Core.Models;

namespace Travelog.Application.Services
{
    public class UserService : IUsersService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<User> _passwordHasher;
        public UserService(IUsersRepository usersRepository, IConfiguration configuration)
        {
            _usersRepository = usersRepository;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<Result<string>> Register(string UserName, string Email, string Password)
        {
            if(await _usersRepository.IsUserNameTakenAsync(UserName))
            {
                return Result.Failure<string>("Имя пользователя занято.");
            }
            // Проверка на уникальность Email через репозиторий
            if (await _usersRepository.IsEmailTakenAsync(Email))
            {
                return Result.Failure<string>("Пользователь с таким Email уже существует.");
            }

            // Хэширование пароля
            var hashedPassword = _passwordHasher.HashPassword(null, Password);

            // Создание пользователя
            var result = User.Create(UserName, Email, hashedPassword);

            if (result.IsFailure)
            {
                return Result.Failure<string>(result.Error);
            }
            // Сохранение пользователя
            Guid userid = await _usersRepository.AddAsync(result.Value);
            return Result.Success("Пользователь зарегистрирован, id: " + userid);
        }

        public async Task<Result<string>> Login(string email, string password)
        {
            var user = await _usersRepository.GetByEmailAsync(email);
            if (user == null)
            {
                return Result.Failure<string>("Пользователь не найден.");
            }

            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (passwordVerificationResult != PasswordVerificationResult.Success)
            {
                return Result.Failure<string>("Неверный пароль.");
            }

            var token = GenerateJwtToken(user.Id, user.UserName);
            return Result.Success(token);
        }

        public string GenerateJwtToken(Guid userId, string username)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, username)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<bool> Update(User user)
        {
            return await _usersRepository.UpdateAsync(user);
        }

        public async Task<bool> Delete(Guid id)
        {
            return await _usersRepository.DeleteAsync(id);
        }

        public async Task<Result<IEnumerable<User>>> SearchUsersByUserName(string username)
        {
            var users = await _usersRepository.SearchUsersByUserNameAsync(username);
            if (users == null || !users.Any())
            {
                return Result.Failure<IEnumerable<User>>("Пользователи не найдены.");
            }
            return Result.Success(users);
        }

    }
}
