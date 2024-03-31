using DiplomService.Services.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DiplomService.Models
{
    public class ApplicationData
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Поле обязательно к заполнению")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Поле обязательно к заполнению")]
        public string? SecondName { get; set; }

        [Required(ErrorMessage = "Поле обязательно к заполнению")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Поле обязательно к заполнению")]
        public DateTime Birthday { get; set; }

        [Required(ErrorMessage = "Поле номера телефона обязательно для заполнения")]
        [RegularExpression(@"^(?:\+7|8)\d{10}$", ErrorMessage = "Некорректный номер телефона")]
        public string PhoneNumber { get; set; } = "";

        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Некорректный адрес")]
        [Required(ErrorMessage = "Поле обязательно к заполнению")]
        public string Email { get; set; } = "";

        [Range(1, 11, ErrorMessage = "Недопустимый курс")]
        [Required(ErrorMessage = "Поле обязательно к заполнению")]
        public int Course { get; set; }

        public bool DivisionDirector { get; set; } = false;
        [Required]
        public int ApplicationId { get; set; }

        [ForeignKey("ApplicationId")]
        [JsonIgnore]
        public virtual EventApplication? Application { get; set; }


        [Required]
        public int DivisionId { get; set; }

        [ForeignKey("DivisionId")]
        [JsonIgnore]
        public virtual Division? Division { get; set; }
    }
}
