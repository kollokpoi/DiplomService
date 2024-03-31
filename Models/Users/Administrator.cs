using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiplomService.Models.Users
{
    [Table("Administrators")]
    public class Administrator : User
    {
        public string HashPassword(string password)
        {
            PasswordHasher<User> hasher = new PasswordHasher<User>();
            return hasher.HashPassword(this, password);
        }
    }
}
