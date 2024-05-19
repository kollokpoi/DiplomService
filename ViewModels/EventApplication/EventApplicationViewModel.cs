using DiplomService.Models;
using System.ComponentModel.DataAnnotations;

namespace DiplomService.ViewModels.EventApplication
{
    public class EventApplicationViewModel
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public bool DivisionsExist { get; set; }
        public List<ApplicationDataViewModel> ApplicationDatas { get; set; } = new();
        public List<Division> Divisions { get; set; } = new();
    }
}
