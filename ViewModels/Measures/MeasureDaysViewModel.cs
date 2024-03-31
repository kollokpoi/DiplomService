namespace DiplomService.ViewModels
{
    public class MeasureDaysViewModel
    {
        public int Id { get; set; }
        public TimeSpan TimeSpan { get; set; } = TimeSpan.Zero;
        public bool Checked { get; set; } = false;
        public int DayNumber { get; set; }
        public string? Place { get; set; }
    }
}
