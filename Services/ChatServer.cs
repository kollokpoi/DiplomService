using DiplomService.Database;
using DiplomService.Models;
using DiplomService.Models.ChatsFolder;
using DiplomService.Models.Users;
using DiplomService.ViewModels.Chat;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using Message = DiplomService.Models.Message;

namespace DiplomService.Services
{
    public class ChatServer
    {
        private TcpListener listener;
        private readonly ApplicationContext _context;
        private List<UserTcp> UserTcps = new List<UserTcp>();
        public ChatServer(ApplicationContext context)
        {
            _context = context;
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Properties", "google-services.json");
            FirebaseApp.Create(new AppOptions() { Credential = GoogleCredential.FromFile(filePath) });
            listener = new TcpListener(IPAddress.Any, 3333);
        }

        public async void Start()
        {
            listener.Start();
            Console.WriteLine("Сервер запущен...");
            while (true)
            {
                TcpClient tcpClient = await listener.AcceptTcpClientAsync();
                _ = Task.Run(() => HandleClient(tcpClient));
            }
        }

        private async Task HandleClient(TcpClient userTcp)
        {
            var client = userTcp;
            var stream = client.GetStream();
            var reader = new StreamReader(stream);

            var line = reader.ReadLine();
            UserTcp clientTcp = null;
            if (line is not null)
            {
                try
                {
                    var user = await _context.Users.FirstOrDefaultAsync(x => x.SecurityStamp == line);
                    if (user is not null)
                    {
                        clientTcp = new UserTcp(user, userTcp);
                        UserTcps.Add(clientTcp);
                        Console.WriteLine($"Подключен {userTcp.Client.RemoteEndPoint}, {user.GetFullName()}");

                    }
                    else
                    {
                        Console.WriteLine("Ошибочная попытка входа");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
                
               
            }
            while (client.Connected)
            {
                try
                {
                    if (clientTcp is null)
                        break;
                    var message = await reader.ReadLineAsync();
                    if (message is null)
                        break;
                    if (message == "disconnect")
                        break;

                    MessageViewModel? messageViewModel = JsonSerializer.Deserialize<MessageViewModel>(message);

                    if (messageViewModel is not null)
                    {
                        Console.WriteLine($"Получено от клиента {client.Client.RemoteEndPoint}: {message}");



                        var chat = await GetChat(messageViewModel, clientTcp.User);
                        if (chat is not null)
                        {
                            chat.Messages.Add(new Message()
                            {
                                DateOfSend = DateTime.UtcNow,
                                Sender = clientTcp.User,
                                SenderId = clientTcp.User.Id,
                                Content = messageViewModel.message
                            });
                            await _context.SaveChangesAsync();

                            messageViewModel.chatId = chat.Id;

                            var secUser = chat.ChatMembers.FirstOrDefault(x => x.User != clientTcp.User);
                            if (secUser is not null)
                            {
                                PushPopUp(secUser.User, messageViewModel);
                                var secUserTcp = UserTcps.Where(x => x.User.Id == secUser.User.Id);
                                if (secUserTcp.Any())
                                {
                                    BroadcastMessage(messageViewModel, secUserTcp.ToList());
                                }
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при обработке сообщения от клиента {client.Client.RemoteEndPoint}: {ex.Message}");
                }
            }
            UserTcps.Remove(clientTcp);
        }

        private async void PushPopUp(User mobileUser, MessageViewModel message)
        {
            string title = "Новое сообщение ";
            var division = await _context.Divisions.FirstOrDefaultAsync(x => x.Id == message.divisionId);
            if (division is not null)
            {
                title += division.Name;
            }
            var tokens = mobileUser.DeviceTokens.Select(x => x.DeviceToken).ToList();
            if (tokens.Count == 0)
                return;
            var pushMesage = new FirebaseAdmin.Messaging.MulticastMessage()
            {
                Tokens = mobileUser.DeviceTokens.Select(x => x.DeviceToken).ToList(),
                Notification = new Notification
                {
                    Title = title,
                    Body = message.message
                },
                Data = { }
            };

            var response = await FirebaseMessaging.DefaultInstance.SendMulticastAsync(pushMesage);
        }

        private async void BroadcastMessage(MessageViewModel message, List<UserTcp> toWho)
        {

            string jsonString = JsonSerializer.Serialize(message);
            foreach (UserTcp userTcp in toWho)
            {
                var writer = new StreamWriter(userTcp.Client.GetStream());
                await writer.WriteLineAsync(jsonString);
                writer.Flush();
            }

        }
        private async Task<Chat?> GetChat(MessageViewModel messageViewModel, User sendedUser)
        {
            Chat? chat = null;
            if (messageViewModel.chatId is null)
            {
                var division = await _context.Divisions.FirstOrDefaultAsync(x => x.Id == messageViewModel.divisionId);
                if (division is not null)
                {

                    var lastDivisionChat = await _context.Chats.FirstOrDefaultAsync(x => x.DivisionId == division.Id && x.ChatMembers.Any(c => c.UserId == sendedUser.Id));
                    if (lastDivisionChat is not null)
                    {
                        chat = lastDivisionChat;
                    }
                    else
                    {
                        var divisionDirectors = division.DivisionMembers.Where(x => x.DivisionDirector);
                        if (divisionDirectors.Any())
                        {
                            var user = divisionDirectors.ToList()[Random.Shared.Next(0, divisionDirectors.Count())].User;
                            chat = new() { Division = division, DivisionId = division.Id, };
                            _context.Chats.Add(chat);
                            await _context.SaveChangesAsync();
                            _context.ChatMembers.Add(new()
                            {
                                User = user,
                                Chat = chat
                            });
                            _context.ChatMembers.Add(new()
                            {
                                User = sendedUser,
                                Chat = chat
                            });
                            await _context.SaveChangesAsync();
                        }
                    }
                }
            }
            else
                chat = await _context.Chats.FirstOrDefaultAsync(x => x.Id == messageViewModel.chatId);
            return chat;
        }
    }
}
