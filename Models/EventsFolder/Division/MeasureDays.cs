using System.Text.Json.Serialization;

namespace DiplomService.Models.EventsFolder.Division
{
    public class MeasureDays
    {
        public int Id { get; set; }
        public TimeSpan TimeSpan { get; set; }
        public int DayNumber { get; set; }
        public int MeasureDivisionsInfoId { get; set; }
        [JsonIgnore]
        public virtual MeasureDivisionsInfo MeasureDivisionsInfo { get; set; } = new();
        public string? Place { get; set; }
    }
}
