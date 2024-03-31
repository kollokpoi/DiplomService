using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DiplomService.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        public string SenderId { get; set; } = string.Empty;
        [Required]
        [JsonIgnore]
        public virtual User Sender { get; set; } = new User();

        [Required]
        public DateTime DateOfSend { get; set; } = DateTime.Now;

        [Required]
        public string Content { get; set; } = "";

    }
}
