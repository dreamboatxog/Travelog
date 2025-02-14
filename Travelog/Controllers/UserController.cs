﻿using Travelog.Core.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Travelog.Core.Models;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using Travelog.Contracts.User;
using Microsoft.AspNetCore.Authorization;
using Travelog.Application.Services;
using System.Security.Claims;
namespace Travelog.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController:ControllerBase
    {
        private IUsersService _usersService;
        public UserController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        /// <summary>
        /// Регистрация
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegister registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _usersService.Register(registerDto.UserName, registerDto.Email, registerDto.Password);

            if (result.IsFailure)
            {
                return BadRequest(new { Message = result.Error });
            }

            return Ok(new { Message = result.Value });
        }

        /// <summary>
        /// Логин
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLogin loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _usersService.Login(loginDto.Email, loginDto.Password);

            if (result.IsFailure)
            {
                return Unauthorized(new { Message = result.Error });
            }

            return Ok(new { Token = result.Value });
        }


        /// <summary>
        /// Поиск пользователей по юзернейму
        /// </summary>
        [HttpGet("search")]
        [Authorize]
        public async Task<IActionResult> SearchUsersByUserName([FromQuery] string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest(new { Message = "Юзернейм не может быть пустым." });
            }

            var result = await _usersService.SearchUsersByUserName(username);

            if (result.IsFailure)
            {
                return NotFound(new { Message = result.Error });
            }

            // Возвращаем список пользователей, найденных по нику
            return Ok(result.Value.Select(u => new { u.UserName, u.Email }));
        }

        /// <summary>
        /// Сменить парол.
        /// </summary>
        [HttpPut("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO changePasswordDTO)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { Message = "User is not authenticated." });
            }
            var result = await _usersService.ChangePassword(userId.Value, changePasswordDTO);
            if (result.IsFailure)
                return NotFound(result.Error);
            return Ok();
        }
        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userIdClaim != null ? Guid.Parse(userIdClaim) : (Guid?)null;
        }
    }
}
