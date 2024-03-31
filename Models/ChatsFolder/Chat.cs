using DiplomService.Models.ChatsFolder;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DiplomService.Models
{
    public class Chat
    {
        [Key]
        public int Id { get; set; }
        public int DivisionId { get; set; }
        public virtual Division Division { get; set; } = new();

        public virtual List<ChatMember> ChatMembers { get; set; } = [];
        public virtual List<Message> Messages { get; set; } = [];
    }
}
