using DiplomService.Services;

namespace DiplomService.ViewModels.Measures
{
    public class EventMeasuresViewModel
    {
        public int Id { get; set; }
        public byte[]? Icon { get; set; }
        public string EventName { get; set; } = "";
        public DateTime DateTime { get; set; }
        public bool SameForAll { get; set; }
        public TimeSpan Length { get; set; } = TimeSpan.Zero;
        public string? MimeType { get { return ImageWorker.GetImageMimeType(Icon); } }
    }
}
