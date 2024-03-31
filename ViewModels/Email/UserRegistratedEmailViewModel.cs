namespace DiplomService.ViewModels.Email
{
    public class UserRegistratedEmailViewModel
    {
        public string Password { get; set; } = "";
        public string Email { get; set; } = "";
        public string? OrganizationName { get; set; } = "";
        public string? BaseUrl { get; set; } = "";
    }
}
