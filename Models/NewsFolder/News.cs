using DiplomService.Services;
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
        public string? MimeType { get { return ImageWorker.GetImageMimeType(Image); } }
    }
}
