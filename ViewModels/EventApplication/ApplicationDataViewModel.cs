using DiplomService.Models;

namespace DiplomService.ViewModels.EventApplication
{
    public class ApplicationDataViewModel
    {
        public ApplicationData ApplicationDatas { get; set; } = new();
        public int EventId { get; set; }
        public bool DivisionsExist { get; set; } = true;
        public List<Division> Divisions { get; set; } = new();
    }
}
