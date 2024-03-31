

using System.ComponentModel.DataAnnotations;

namespace DiplomService.ViewModels.AuthViewModels
{
    public class LoginUpdateViewModel
    {
        [Required(ErrorMessage = "Логин обязателен к заполнению.")]
        public string Login { get; set; } = "";

        [Required(ErrorMessage = "Код обязателен к заполнению.")]
        public int Code { get; set; }
    }
}
