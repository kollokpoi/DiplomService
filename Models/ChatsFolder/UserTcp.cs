using DiplomService.Models.Users;
using System.Net.Sockets;

namespace DiplomService.Models.ChatsFolder
{
    public class UserTcp
    {
        public MobileUser User { get;}
        public TcpClient Client { get;}
        public UserTcp(MobileUser webUser, TcpClient client) 
        { 
            User = webUser;
            Client = client;
        }
    }
}
