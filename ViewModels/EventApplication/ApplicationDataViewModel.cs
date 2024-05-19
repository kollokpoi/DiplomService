using DiplomService.Services.Attributes;
using System.ComponentModel.DataAnnotations;

namespace DiplomService.ViewModels.EventApplication
{
    public class ApplicationDataViewModel
    {
        public string? UserId { get; set; }
        public int? DivisionId { get; set; }

        [Required(ErrorMessage = "Поле обязательно к заполнению")]
        public string Name { get; set; } = string.Empty;

        public DateTime? Birthday { get; set; }
        [RussianPhoneNumber(ErrorMessage = "Укажите действительный номер")]
        public string? PhoneNumber { get; set; } = "";

        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Некорректный адрес")]
        public string? Email { get; set; } = "";

        [Range(1, 11, ErrorMessage = "Недопустимый курс")]
        public int? Course { get; set; }

        public string? WorkingPlace { get; set; } = string.Empty;
        public bool DivisionLeader { get; set; }
    }
}
