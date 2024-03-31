using DiplomService.Models.Users;
using Microsoft.Build.Framework;
using System.Text.Json.Serialization;

namespace DiplomService.Models
{
    public class DivisionUsers
    {
        public int Id { get; set; }
        public int DivisionId { get; set; }
        [Required]
        [JsonIgnore]
        public virtual Division Division { get; set; } = new();
        public string UserId { get; set; } = "";
        [Required]
        [JsonIgnore]
        public virtual MobileUser User { get; set; } = new();
        [Required]
        public bool DivisionDirector { get; set; } = false;
    }
}
