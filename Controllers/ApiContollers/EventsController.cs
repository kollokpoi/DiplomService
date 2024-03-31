using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DiplomService.Database;
using DiplomService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using DiplomService.Models.Users;

namespace DiplomService.Controllers.ApiContollers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<User> _userManager;

        public EventsController(ApplicationContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Events
        [HttpGet("GetEvents")]
        public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
        {
            if (_context.Events == null)
            {
                return NotFound();
            }
            return await _context.Events.Where(x=>x.ReadyToShow && x.PublicEvent).ToListAsync();
        }

        [HttpGet("GetEventsForUser")]
        public async Task<ActionResult<IEnumerable<Event>>> GetEventsForUser()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                var events = _context.MobileUsers
                    .Where(mu => mu == user)
                    .SelectMany(mu => mu.UserDivisions)
                    .Select(du => du.Division.Event)
                    .Where(e=> e.ReadyToShow)
                    .Distinct()
                    .ToList();
                return Ok(events);
            }
        }

        [HttpGet("GetEvent/{id}")]
        public async Task<ActionResult<Event>> GetEvent(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            if (_context.Events == null)
            {
                return NotFound();
            }
            var @event = await _context.Events.FindAsync(id);

            if (@event == null)
            {
                return NotFound();
            }
            @event.Divisions = @event.Divisions.Where(div => div.DivisionMembers.Any(dm => dm.UserId == user.Id)).ToList();
            return @event;
        }
    }
}
