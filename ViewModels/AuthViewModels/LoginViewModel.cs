using System.ComponentModel.DataAnnotations;

namespace DiplomService.ViewModels.AuthViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Пожалуйста, введите корректный email.")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Введите пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; } = "";

        [Display(Name = "Запомнить?")]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
