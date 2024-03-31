using System.ComponentModel.DataAnnotations;

namespace DiplomService.ViewModels.AuthViewModels
{
    public class RegistrationViewModel
    {
        [Required(ErrorMessage = "Не указано имя")]
        [Display(Name = "Имя")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Не указана фамилия")]
        [Display(Name = "Фамилия")]
        public string? SecondName { get; set; }

        [Required(ErrorMessage = "Не указано отчество")]
        [Display(Name = "Отчество")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Не указана почта")]
        [EmailAddress(ErrorMessage = "Некорректный адрес")]
        [Display(Name = "Адрес электронной почты")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Некорректный адрес")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        [MinLength(8, ErrorMessage = "Минимальная длина - 8 символов")]
        public string Password { get; set; } = "";

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
        public string? ConfirmPassword { get; set; }
    }
}
