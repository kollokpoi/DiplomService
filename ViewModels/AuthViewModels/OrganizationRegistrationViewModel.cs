using System.ComponentModel.DataAnnotations;

namespace DiplomService.ViewModels.AuthViewModels
{
    public class OrganizationRegistrationViewModel
    {
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [MaxLength(40)]
        [Display(Name = "Название вашей организации.")]
        public string OrganizationName { get; set; } = "";

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [MaxLength(40)]
        [Display(Name = "Почтовый адрес вашей организации.")]
        [EmailAddress(ErrorMessage = "Пожалуйста, введите корректный email.")]
        public string OrganizationEmail { get; set; } = "";

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [MaxLength(40)]
        [Display(Name = "Ваш личный почтовый адрес. На него придет ответ на заявку.")]
        [EmailAddress(ErrorMessage = "Пожалуйста, введите корректный email.")]
        public string UserEmail { get; set; } = "";


        [Display(Name = "Комментарий к заявке.")]
        public string? Message { get; set; }

        public bool Sended { get; set; } = false;
    }
}
