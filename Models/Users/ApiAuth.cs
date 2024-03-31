namespace DiplomService.Models.Users
{
    public class ApiAuth
    {
        public string Phone { get; set; } = "";
        public string Code { get; set; } = "";
        public string DeviceToken { get; set; } = string.Empty;
    }
}
