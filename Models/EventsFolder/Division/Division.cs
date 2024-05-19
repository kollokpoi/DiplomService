using DiplomService.Models.EventsFolder.Division;
using DiplomService.Services;
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

        [Range(-180, 180)]
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
        public bool DivisionLeaderExist { get { return DivisionMembers.Any(x => x.DivisionDirector); } }

        [NotMapped]
        public string? MimeType { get { return ImageWorker.GetImageMimeType(PreviewImage); } }

    }
}
