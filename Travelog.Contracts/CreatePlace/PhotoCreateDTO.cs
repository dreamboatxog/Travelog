using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Travelog.Contracts.CreatePlace
{
    public record PhotoCreateDTO
    {
        [Required(ErrorMessage = "Отсутвует файл иозбражения.")]
        public IFormFile File { get; set; }
        public string Description { get; set; }
    }
}
