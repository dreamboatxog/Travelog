using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travelog.Contracts.User
{
    public record ChangePasswordDTO
    {
        [Required(ErrorMessage = "Исходный пароль обязателен.")]
        public string OldPassword { get; set; } = null!;

        [Required(ErrorMessage = "Новый пароль обязателен.")]
        [StringLength(100, ErrorMessage = "Пароль должен быть от 6 до 100 символов.", MinimumLength = 6)]
        public string NewPassword { get; set; } = null!;
    }
}
