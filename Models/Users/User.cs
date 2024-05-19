using DiplomService.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiplomService.Models
{
    /// <summary>
    /// Модель User служит для хранения данных о пользователях системы
    /// </summary>
    [Table("Users")]
    public class User : IdentityUser
    {
        public string? Name { get; set; }

        public string? SecondName { get; set; }

        public string? LastName { get; set; }

        public byte[]? Image { get; set; }
        public virtual List<UserDeviceTokens> DeviceTokens { get; set; } = new();
        public virtual List<DivisionUsers> UserDivisions { get; set; } = new();

        public string GetFullName() => $"{SecondName} {Name} {LastName}";
    }
}
