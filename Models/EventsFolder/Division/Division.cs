using DiplomService.Models.EventsFolder.Division;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DiplomService.Models
{
    public class Division
    {
        [Key]
        public int Id { get; set; }

        public byte[]? PreviewImage { get; set; } = null;

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = "";

        [MaxLength(250)]
        public string? Description { get; set; }

        [Required]
        public DateTime DateOfStart { get; set; }

        public DateTime? DateOfEnd { get; set; } = null;

        [Range(-180,180)]
        public double Longitude { get; set; }
        [Range(-90, 90)]
        public double Latitude { get; set; }
        public string PlaceName { get; set; } = "";

        public int EventId { get; set; }

        [JsonIgnore]
        public virtual Event Event { get; set; } = new();
        [JsonIgnore]
        public virtual List<MeasureDivisionsInfo> MeasureDivisionsInfos { get; set; } = new();
        [JsonIgnore]
        public virtual List<DivisionUsers> DivisionMembers { get; set; } = new();
        [NotMapped]
        public bool DivisionLeaderExist { get { return DivisionMembers.Any(x => x.DivisionDirector); }}

        [NotMapped]
        public string? MimeType { get { return GetImageMimeType(); } }
        private string GetImageMimeType()
        {
            if (PreviewImage == null) return "application/octet-stream";
            if (PreviewImage.Length < 4)
            {
                return "application/octet-stream"; // По умолчанию, если массив слишком короткий для определения
            }

            if (PreviewImage[0] == 0xFF && PreviewImage[1] == 0xD8 && PreviewImage[2] == 0xFF)
            {
                return "image/jpeg";
            }
            else if (PreviewImage[0] == 0x89 && PreviewImage[1] == 0x50 && PreviewImage[2] == 0x4E && PreviewImage[3] == 0x47)
            {
                return "image/png";
            }
            // Добавьте другие проверки для других форматов, если это необходимо

            return "application/octet-stream"; // Если формат неизвестен, вернуть по умолчанию
        }
    }
}
