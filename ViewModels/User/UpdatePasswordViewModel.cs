using System.ComponentModel.DataAnnotations;

namespace DiplomService.ViewModels.User
{
    public class UpdatePasswordViewModel
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Текущий пароль")]
        [MinLength(8, ErrorMessage = "Минимальная длина - 8 символов")]
        public string OldPassword { get; set; } = "";

        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль")]
        [MinLength(8, ErrorMessage = "Минимальная длина - 8 символов")]
        public string Password { get; set; } = "";

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
        public string? ConfirmPassword { get; set; }
    }
}
