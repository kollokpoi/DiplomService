using DiplomService.Database;
using DiplomService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiplomService.Controllers.ApiContollers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DivisionsController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<User> _userManager;

        public DivisionsController(ApplicationContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        [HttpGet("GetDivisions")]
        public async Task<ActionResult<IEnumerable<Division>>> GetDivisions()
        {
            if (_context.Divisions == null)
            {
                return NotFound();
            }
            return await _context.Divisions.ToListAsync();
        }
        [HttpGet("GetDivisionForUser/{eventId}")]
        public async Task<ActionResult<IEnumerable<Division>>> GetDivisionForUser(int eventId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return NotFound();
            }
            var divisions = await _context.Divisions
                .Where(d => d.EventId == eventId)
                .Where(d => d.DivisionMembers.Any(du => du.UserId == user.Id))
                .ToListAsync();

            return Ok(divisions);
        }
        // GET: api/Divisions/5
        [HttpGet("GetDivision/{id}")]
        public async Task<ActionResult> GetDivision(int id)
        {
            
            Console.WriteLine("Запрос получен в: " + DateTime.Now.Hour + ":" + 
                DateTime.Now.Minute + ":" + DateTime.Now.Second + ":" + DateTime.Now.Millisecond);
            if (_context.Divisions == null)
            {
                return NotFound();
            }
            var division = await _context.Divisions.FirstOrDefaultAsync(x=>x.Id==id);

            if (division == null)
            {
                return NotFound();
            }
            Console.WriteLine("Ответ отправлен в: " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" 
                + DateTime.Now.Second + ":" + DateTime.Now.Millisecond);
            return Ok(division);
        }
    }
}
