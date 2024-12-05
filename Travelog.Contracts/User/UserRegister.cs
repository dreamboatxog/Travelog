using System.ComponentModel.DataAnnotations;

namespace Travelog.Contracts.User
{
    public record UserRegister
    {
        [Required(ErrorMessage = "Имя пользователя обязательно.")]
        [StringLength(50, ErrorMessage = "Имя пользователя должно быть от 3 до 50 символов.", MinimumLength = 3)]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "Email обязателен.")]
        [EmailAddress(ErrorMessage = "Некорректный формат email.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Пароль обязателен.")]
        [StringLength(100, ErrorMessage = "Пароль должен быть от 6 до 100 символов.", MinimumLength = 6)]
        public string Password { get; set; } = null!;
    }
}
