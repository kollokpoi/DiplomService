using DiplomService.Models.Users;
using System.Net.Sockets;

namespace DiplomService.Models.ChatsFolder
{
    public class UserTcp
    {
        public User User { get; }
        public TcpClient Client { get; }
        public UserTcp(User webUser, TcpClient client)
        {
            User = webUser;
            Client = client;
        }
    }
}
