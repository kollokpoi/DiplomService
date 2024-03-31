using DiplomService.Models.Users;
using Microsoft.EntityFrameworkCore;



namespace DiplomService.Models.ChatsFolder
{
    [PrimaryKey(nameof(ChatId), nameof(UserId))]
    public class ChatMember
    {
        public int ChatId { get; set; }
        public virtual Chat Chat { get; set; } = new();
        public string UserId { get; set; } = string.Empty;
        public virtual MobileUser User { get; set; } = new();
    }
}
