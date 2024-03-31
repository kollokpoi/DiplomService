using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DiplomService.Models.Users
{
    public class UserDeviceTokens
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public virtual MobileUser User { get; set; } = new();
        public string DeviceToken { get; set; } = string.Empty;
    }
}
