using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DiplomService.Models.Users
{
    [Table("OrganizationUsers")]
    public class OrganizationUsers : User
    {
        [Required]
        public bool OrganizationLeader { get; set; } = false;

        public int OrganizationId { get; set; }
        [Required]
        [ForeignKey("OrganizationId")]
        [JsonIgnore]
        public virtual Organization Organization { get; set; } = new();
    }
}
