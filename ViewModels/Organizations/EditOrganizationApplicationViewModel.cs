using System.ComponentModel.DataAnnotations;

namespace DiplomService.ViewModels.OrganizationViewModels
{
    public class EditOrganizationApplicationViewModel
    {
        [Required]
        public Guid Id { get; set; }

        public string OrganizationName { get; set; } = "";
        public string OrganizationEmail { get; set; } = "";
        public string UserEmail { get; set; } = "";
        public DateTime DateOfSend { get; set; } = DateTime.Now;
        public string? Message { get; set; }

        [Required]
        [Display(Name = "Принять заявление")]
        public bool ApplicationApproved { get; set; } = false;

        [Required(ErrorMessage = "Введите комментарий")]
        [Display(Name = "Ответ пользователю:")]
        public string? ResponseMessage { get; set; }
    }
}
