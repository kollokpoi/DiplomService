using DiplomService.Services.Attributes;
using System.ComponentModel.DataAnnotations;

namespace DiplomService.ViewModels.User
{
    public class OrganizationUserViewModel
    {
        public string Id { get; set; } = "";
        [Display(Name = "Имя")]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        public string Name { get; set; } = "";
        [Display(Name = "Фамилия")]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        public string SecondName { get; set; } = "";
        [Display(Name = "Отчество")]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        public string LastName { get; set; } = "";
        [Display(Name = "Почтовый адрес")]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        [EmailAddress(ErrorMessage = "Неподходящий формат")]

        public string Email { get; set; } = "";
        [Display(Name = "Номер телефона")]
        [RussianPhoneNumber(ErrorMessage = "Неподходящий формат")]
        public string? PhoneNumber { get; set; } = "";

        public bool Sended { get; set; } = false;
        public int OrganizationId { get; set; }
    }
}
