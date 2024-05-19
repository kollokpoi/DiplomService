using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiplomService.Models.Users
{
    [Table("MobileUsers")]
    public class MobileUser : User
    {
        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; } = default(DateTime);
        public int Course { get; set; } = 0;
        public string WorkingPlace { get; set; } = string.Empty;
    }
}
