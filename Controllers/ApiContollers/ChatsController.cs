using DiplomService.Database;
using DiplomService.Models;
using DiplomService.Models.Users;
using DiplomService.ViewModels.Chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using System;

namespace DiplomService.Controllers.ApiContollers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "MobileUser")]
    public class ChatsController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<User> _userManager;
        
        public ChatsController(ApplicationContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        [HttpGet("GetUserChats")]
        public async Task<IActionResult> GetUserChats()
        {
            var user = await _userManager.GetUserAsync(User) as MobileUser;
            if (user is null)
                return Unauthorized();

            List<Chat> chats = _context.Chats.Where(x => x.ChatMembers.Any(x=>x.User==user)).ToList();
            var viewModel = new List<ChatViewModel>();
            foreach (Chat chat in chats)
            {
                var modelItem = new ChatViewModel()
                {
                    Id = chat.Id,
                    Title = chat.Division.Name,
                    Image = chat.Division.PreviewImage,
                    DivisionId = chat.DivisionId
                };
                if (chat.Messages.Count>0)
                {
                    modelItem.Messages.Add(new MessageViewModel()
                    {
                        chatId = chat.Id,
                        divisionId = chat.Division.Id,
                        dateTime = chat.Messages.Last().DateOfSend,
                        message = chat.Messages.Last().Content
                    });
                }

                viewModel.Add(modelItem);
            }
            return Ok(viewModel);
        }

        [HttpGet("GetChat/{id}")]
        public async Task<IActionResult> GetChat(int id)
        {
            var user = await _userManager.GetUserAsync(User) as MobileUser;
            if (user is null)
                return Unauthorized();

            var chat = await _context.Chats.FirstOrDefaultAsync(x => x.Id == id);
            if (chat is null)
                return NotFound();
            var secondUser = chat.ChatMembers.Select(x => x.User).FirstOrDefault(x=>x.Id!=user.Id);
            if (secondUser is null)
                return BadRequest();

            var viewModel = new ChatViewModel()
            {
                Id = chat.Id,
                Title = chat.Division.Name,
                Image = chat.Division.PreviewImage,
                DivisionId = chat.DivisionId,
                opponentName = secondUser.GetFullName()
            };
           
            foreach (var item in chat.Messages.OrderBy(x=>x.DateOfSend))
            {
                viewModel.Messages.Add(new MessageViewModel()
                {
                    chatId = chat.Id,
                    divisionId = chat.Division.Id,
                    dateTime = item.DateOfSend,
                    message = item.Content,
                    selfSend = item.Sender == user
                });
            }
            return Ok(viewModel);
        }
        [HttpGet("GetChatByDivision/{divisionId}")]
        public async Task<IActionResult> GetChatByDivision(int divisionId)
        {
            var user = await _userManager.GetUserAsync(User) as MobileUser;
            if (user is null)
                return Unauthorized();

            var division = await _context.Divisions.FirstOrDefaultAsync(x => x.Id == divisionId);
            if (division is not null)
            {
                Chat? chat = null;
                var lastDivisionChat = await _context.Chats.FirstOrDefaultAsync(x => x.DivisionId == division.Id && x.ChatMembers.Any(c => c.UserId == user.Id));
                if (lastDivisionChat is not null)
                {
                    chat = lastDivisionChat;
                }
                else
                {
                    var divisionDirectors = division.DivisionMembers.Where(x => x.DivisionDirector);
                    if (divisionDirectors.Any())
                    {
                        var secondUser = divisionDirectors.ToList()[Random.Shared.Next(0, divisionDirectors.Count())].User;
                        chat = new() { Division = division, DivisionId = division.Id, };
                        _context.Chats.Add(chat);
                        await _context.SaveChangesAsync();
                        _context.ChatMembers.Add(new()
                        {
                            User = secondUser,
                            Chat = chat
                        });
                        _context.ChatMembers.Add(new()
                        {
                            User = user,
                            Chat = chat
                        });
                        await _context.SaveChangesAsync();
                    }
                }
                if (chat is null)
                    return BadRequest();

                var secndUser = chat.ChatMembers.Select(x => x.User).FirstOrDefault(x => x.Id != user.Id);
                if (secndUser is null)
                    return BadRequest();

                var viewModel = new ChatViewModel()
                {
                    Id = chat.Id,
                    Title = chat.Division.Name,
                    Image = chat.Division.PreviewImage,
                    DivisionId = chat.DivisionId,
                    opponentName = secndUser.GetFullName()
                };

                foreach (var item in chat.Messages.OrderBy(x => x.DateOfSend))
                {
                    viewModel.Messages.Add(new MessageViewModel()
                    {
                        chatId = chat.Id,
                        divisionId = chat.Division.Id,
                        dateTime = item.DateOfSend,
                        message = item.Content,
                        selfSend = item.Sender == user
                    });
                }
                return Ok(viewModel);
            }
            return BadRequest();
        }
    }
}
