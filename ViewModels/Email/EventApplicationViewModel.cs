namespace DiplomService.ViewModels.Email
{
    public class EventApplicationViewModel
    {
        public string Comment { get; set; } = "";
        public string Email { get; set; } = "";
        public bool Accepted { get; set; }
        public string BaseUrl { get; set; } = "";
    }
}
