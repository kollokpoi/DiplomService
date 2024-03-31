using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DiplomService.Models
{
    public class Section
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = "";
        [Required]
        public string Description { get; set; } = "";
        public byte[]? Image { get; set; }

        public int NewsId { get; set; }
        [Required]
        [JsonIgnore]
        public virtual News News { get; set; } = new();

        [NotMapped]
        public string? MimeType { get { return GetImageMimeType(); } }
        private string GetImageMimeType()
        {
            if (Image == null) return "application/octet-stream";
            if (Image.Length < 4)
            {
                return "application/octet-stream"; // По умолчанию, если массив слишком короткий для определения
            }

            if (Image[0] == 0xFF && Image[1] == 0xD8 && Image[2] == 0xFF)
            {
                return "image/jpeg";
            }
            else if (Image[0] == 0x89 && Image[1] == 0x50 && Image[2] == 0x4E && Image[3] == 0x47)
            {
                return "image/png";
            }
            // Добавьте другие проверки для других форматов, если это необходимо

            return "application/octet-stream"; // Если формат неизвестен, вернуть по умолчанию
        }
    }
}
