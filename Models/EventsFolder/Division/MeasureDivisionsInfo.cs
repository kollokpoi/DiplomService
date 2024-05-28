using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DiplomService.Models.EventsFolder.Division
{
    public class MeasureDivisionsInfo
    {
        public int Id { get; set; }
        public bool OneTime { get; set; } = true;
        public bool WeekDays { get; set; } = false;
        public TimeSpan Length { get; set; } = TimeSpan.Zero;
        public string Place { get; set; } = "";
        public bool SameForAll { get; set; } = false;

        public int? DivisionId { get; set; }
        [JsonIgnore]
        public virtual Models.Division? Division { get; set; } = null;
        [NotMapped]
        public byte[]? Image { get => Measure.Icon; }
        public int MeasureId { get; set; }
        [JsonIgnore]
        public virtual Measure Measure { get; set; } = new();

        public virtual List<MeasureDates> MeasureDates { get; set; } = new();
        public virtual List<MeasureDays> MeasureDays { get; set; } = new();
    }
}
