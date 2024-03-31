using System.ComponentModel.DataAnnotations.Schema;

namespace DiplomService.Models.Users
{
    [Table("WebUsers")]
    public class WebUser : User
    {

        public virtual List<EventApplication> Applications { get; set; } = new();
    }
}
