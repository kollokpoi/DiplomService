using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DiplomService.Models
{
    public class News
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Заголовок обязателен к заполнению для новости.")]
        public string Title { get; set; } = "";
        [Required(ErrorMessage = "Описание новости обязательно к заполнению.")]
        public string Description { get; set; } = "";
        [Required]
        public DateTime DateTime { get; set; } = DateTime.Now;

        public string Author { get; set; } = "";
        public byte[]? Image { get; set; } = null;

        public virtual List<Section>? Sections { get; set; } = null;

        public int? EventId { get; set; }
        [JsonIgnore]
        public virtual Event Event { get; set; } = new();

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
