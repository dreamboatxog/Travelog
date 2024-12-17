using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Travelog.Contracts.CreatePlace;
using Travelog.Contracts.UpdatePlace;
using Travelog.Core.Abstractions;

namespace Travelog.Controllers
{
    [Route("api/places")]
    [ApiController]
    [Authorize]
    public class PlacesController : ControllerBase
    {
        private readonly IPlacesService _placeService;

        public PlacesController(IPlacesService placeService)
        {
            _placeService = placeService;
        }

        /// <summary>
        /// Создает новое место для текущего пользователя.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] PlaceCreateDTO placeCreateDTO,[FromQuery] bool returnCreated= false)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { Message = "User is not authenticated." });
            }

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var result = await _placeService.Create(userId.Value, placeCreateDTO, baseUrl);

            if (result.IsFailure)
            {
                return BadRequest(new { Error = result.Error });
            }

            if (returnCreated)
            {
                return Ok(result.Value); 
            }
            return Ok(new { Id = result.Value.Id });
        }

        /// <summary>
        /// Информация о месте по ID.
        /// </summary>
        [HttpGet("{placeId}")]
        public async Task<IActionResult> GetById(Guid placeId)
        {
            var result = await _placeService.GetById(placeId);

            if (result.IsFailure)
            {
                return NotFound(new { Error = result.Error });
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Получает все места текущего пользователя.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllForCurrentUser()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { Message = "User is not authenticated." });
            }

            var result = await _placeService.GetPlacesByUserId(userId.Value);

            if (result.IsFailure)
            {
                return NotFound(new { Error = result.Error });
            }

            return Ok(result.Value);
        }

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userIdClaim != null ? Guid.Parse(userIdClaim) : (Guid?)null;
        }

        /// <summary>
        /// Получает все места пользователя по нику.
        /// </summary>
        [HttpGet("user/{username}")]
        public async Task<IActionResult> GetByUserName(string username)
        {
            var result = await _placeService.GetPlacesByUserName(username);

            if (result.IsFailure)
            {
                return NotFound(new { Error = result.Error });
            }

            return Ok(result.Value);
        }


        /// <summary>
        /// Обновление места по Id.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlace(Guid id, [FromForm] PlaceUpdateDTO place, [FromQuery] bool returnUpdated = false)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { Message = "User is not authenticated." });
            }

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var result = await _placeService.UpdatePlaceAsync(id, place, userId.Value, baseUrl);

            if (result.IsFailure)
            {
                return NotFound(new { Error = result.Error });
            }

            if (returnUpdated)
            {
                return Ok(result.Value); // Возвращаем обновленный объект
            }

            return Ok(new { Success = true }); // Возвращаем только статус
        }

        /// <summary>
        /// Удаление места по Id.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlace(Guid id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { Message = "User is not authenticated." });
            }
            var result= await _placeService.DeletePlaceAsync(userId.Value,id);
            if (result.IsFailure)
                return NotFound(result.Error);
            return Ok();
        }

    }
}
