namespace DiplomService.ViewModels.News
{
    public class NewsListViewModel
    {
        public int? EventId { get; set; }
        public string? EventName { get; set; }
        public List<Models.News> News { get; set; } = new List<Models.News>();
    }
}
