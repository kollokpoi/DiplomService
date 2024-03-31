using DiplomService.Database;
using DiplomService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DiplomService.Controllers.ServiceControllers
{
    public class UserController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(ApplicationContext context, UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "Administrator")]
        public IActionResult List()
        {
            var model = new List<User>();
            model.AddRange(_context.Administrators.ToList());
            model.AddRange(_context.OrganizationUsers.ToList());
            model.AddRange(_context.WebUsers.ToList());
            model.AddRange(_context.MobileUsers.ToList());
            return View(model);
        }
        public IActionResult Details(string Id)
        {
            var model = _context.Users.First(x => x.Id == Id);
            if (model != null)
            {
                return View(model);
            }
            return NotFound();
        }
        public IActionResult Edit()
        {
            return View();
        }
        public IActionResult Delete()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> BlockUser(string id, string time)
        {
            time = time.ToLower();
            DateTimeOffset offset = DateTimeOffset.Now;
            if (time.Contains("day"))
            {
                offset = DateTimeOffset.Now.AddDays(1);
            }
            else if (time.Contains("week"))
            {
                offset = DateTimeOffset.Now.AddDays(7);
            }
            else if (time.Contains("mounth"))
            {
                offset = DateTimeOffset.Now.AddDays(31);
            }
            else if (time.Contains("always"))
            {
                offset = DateTimeOffset.MaxValue;
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.SetLockoutEndDateAsync(user, offset);
                return RedirectToAction(nameof(List));
            }

            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UnlockUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.SetLockoutEndDateAsync(user, null);
                return RedirectToAction(nameof(List));
            }

            return View();
        }
    }
}
