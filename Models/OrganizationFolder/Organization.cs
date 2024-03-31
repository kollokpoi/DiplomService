using DiplomService.Models.Users;
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
        public string? Name { get; set; }

        [Required(ErrorMessage = "Укажите почту")]
        [MaxLength(40)]
        public string? Email { get; set; }


        public string? Description { get; set; }

        [Required]
        public bool ReadyToShow { get; set; } = false;

        public byte[]? Preview { get; set; } = null;

        [NotMapped]
        public string? MimeType { get { return GetImageMimeType(); } }

        private string GetImageMimeType()
        {
            if (Preview == null) return "application/octet-stream";
            if (Preview.Length < 4)
            {
                return "application/octet-stream"; // По умолчанию, если массив слишком короткий для определения
            }

            if (Preview[0] == 0xFF && Preview[1] == 0xD8 && Preview[2] == 0xFF)
            {
                return "image/jpeg";
            }
            else if (Preview[0] == 0x89 && Preview[1] == 0x50 && Preview[2] == 0x4E && Preview[3] == 0x47)
            {
                return "image/png";
            }
            // Добавьте другие проверки для других форматов, если это необходимо

            return "application/octet-stream"; // Если формат неизвестен, вернуть по умолчанию
        }

        [JsonIgnore]
        public virtual List<OrganizationUsers> OrganizationUsers { get; set; } = new List<OrganizationUsers>();
        [JsonIgnore]
        public virtual List<Event> Events { get; set; } = new List<Event>();
    }
}
