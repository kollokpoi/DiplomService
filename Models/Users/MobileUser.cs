using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiplomService.Models.Users
{
    [Table("MobileUsers")]
    public class MobileUser : User
    {
        [Required]
        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }
        public int Course { get; set; }
        public virtual List<UserDeviceTokens> DeviceTokens { get; set; } = new();
        public virtual List<DivisionUsers> UserDivisions { get; set; } = new();
    }
}
