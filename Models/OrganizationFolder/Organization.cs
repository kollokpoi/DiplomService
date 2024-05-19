using DiplomService.Models.Users;
using DiplomService.Services;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DiplomService.Models
{
    public class Organization
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Укажите название")]
        [MaxLength(40)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Укажите почту")]
        [MaxLength(40)]
        public string? Email { get; set; }


        public string? Description { get; set; }

        [Required]
        public bool ReadyToShow { get; set; } = false;

        public byte[]? Preview { get; set; } = null;

        [NotMapped]
        public string? MimeType { get { return ImageWorker.GetImageMimeType(Preview); } }

        [JsonIgnore]
        public virtual List<OrganizationUsers> OrganizationUsers { get; set; } = new List<OrganizationUsers>();
        [JsonIgnore]
        public virtual List<Event> Events { get; set; } = new List<Event>();
    }
}
