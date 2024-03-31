using DiplomService.Database;
using DiplomService.Models;
using DiplomService.Models.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiplomService.Controllers.ApiContollers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController(ApplicationContext context) : ControllerBase
    {
        private readonly ApplicationContext _context = context;

        [HttpGet("GetEventNews/{eventId}")]
        public async Task<IActionResult> GetEventNews(int eventId)
        {
            var @event = await _context.Events.FirstOrDefaultAsync(x => x.Id == eventId);
            if (@event is null)
                return NotFound();

            return Ok(@event.News);
        }

        [HttpGet("GetNews/{id}")]
        public async Task<IActionResult> GetNews(int id)
        {
            var news = await _context.News.FirstOrDefaultAsync(x => x.Id == id);
            if (news is null) return NotFound();
            return Ok(news);
        }
    }
}
