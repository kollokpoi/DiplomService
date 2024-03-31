namespace DiplomService.ViewModels.Email
{
    public class ApplicationEmailViewModel
    {
        public string OrganizationName { get; set; } = "";
        public string UserEmail { get; set; } = "";
        public DateTime DateOfSend { get; set; } = DateTime.Now;
        public bool ApplicationApproved { get; set; } = false;
        public string Password { get; set; } = "";
        public string? ResponseMessage { get; set; }
        public string BaseUrl { get; set; } = "";
    }
}
