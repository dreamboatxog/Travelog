using System.ComponentModel.DataAnnotations;

namespace Travelog.Contracts.CreatePlace
{
    public record PlaceCreateDTO
    {
        [Required(ErrorMessage = "Не указано название")]
        [StringLength(100, ErrorMessage = "Название должно быть от 1 до 100 символов.", MinimumLength = 1)]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required(ErrorMessage = "Координаты отсутсвуют.")]
        public double Latitude { get; set; }
        [Required(ErrorMessage = "Координаты отсутсвуют.")]
        public double Longitude { get; set; }
        public List<PhotoCreateDTO> Photos { get; set; } = new List<PhotoCreateDTO>();
    }
}
