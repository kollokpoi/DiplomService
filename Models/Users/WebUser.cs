using System.ComponentModel.DataAnnotations.Schema;

namespace DiplomService.Models.Users
{
    [Table("WebUsers")]
    public class WebUser : User
    {
        public string WorkingPlace { get; set; } = string.Empty;
        public virtual List<EventApplication> Applications { get; set; } = new();
    }
}
