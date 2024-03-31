using DiplomService.Models;
using System.ComponentModel.DataAnnotations;

namespace DiplomService.ViewModels.EventApplication
{
    public class EventApplicationViewModel
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string ApplicationSenderId { get; set; } = "";

        [Required(ErrorMessage = "Поле обязательно к заполнению")]
        public string Institution { get; set; } = "";
        public bool DivisionsExist { get; set; }
        public List<ApplicationData> ApplicationDatas { get; set; } = new();
        public List<Division> Divisions { get; set; } = new();
    }
}
