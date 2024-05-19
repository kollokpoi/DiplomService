using DiplomService.Services;
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
        public string? MimeType { get { return ImageWorker.GetImageMimeType(Image); } }
    }
}
