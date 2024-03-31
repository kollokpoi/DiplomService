using DiplomService.Models.EventsFolder.Division;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DiplomService.Models
{
    public class Measure
    {
        [Key]
        public int Id { get; set; }
        public byte[]? Icon { get; set; }
        [Required(ErrorMessage = "Не указано имя")]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Не указано место проведения")]
        public string Place { get; set; } = "";

        public string? Descrition { get; set; } = "";

        public TimeSpan Length { get; set; } = TimeSpan.Zero;

        [Required(ErrorMessage = "Пропущен пункт")]
        public bool SameForAll { get; set; } = true;

        [Required]
        public bool OneTime { get; set; } = false;
        [Required]
        public bool WeekDays { get; set; } = false;

        public int EventId { get; set; }

        [Required]
        [ForeignKey("EventId")]
        [JsonIgnore]
        public virtual Event? Event { get; set; } = new();
        public virtual List<MeasureDivisionsInfo> MeasureDivisionsInfos { get; set; } = new();
    }
}
