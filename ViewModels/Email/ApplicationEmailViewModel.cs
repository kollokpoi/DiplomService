namespace DiplomService.ViewModels.Email
{
    public class ApplicationEmailViewModel
    {
        public string OrganizationName { get; set; } = "";
        public DateTime DateOfSend { get; set; } = DateTime.Now;
        public bool ApplicationApproved { get; set; } = false;
        public string Password { get; set; } = "";
        public string? ResponseMessage { get; set; }
        public string BaseUrl { get; set; } = "";
        public string EmailToSend { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
