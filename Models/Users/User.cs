using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiplomService.Models
{
    /// <summary>
    /// Модель User служит для хранения данных о пользователях системы
    /// </summary>
    [Table("Users")]
    public class User : IdentityUser
    {
        [Required]
        public string? Name { get; set; }

        [Required]
        public string? SecondName { get; set; }

        [Required]
        public string? LastName { get; set; }

        public byte[]? Image { get; set; }

        public string GetFullName()=> $"{SecondName} {Name} {LastName}";
    }
}
