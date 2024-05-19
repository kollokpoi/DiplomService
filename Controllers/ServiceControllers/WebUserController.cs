using DiplomService.Database;
using DiplomService.Models;
using DiplomService.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DiplomService.Controllers.ServiceControllers
{
    [Authorize(Roles = "WebUser")]
    public class WebUserController : CabinetController
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public WebUserController(ApplicationContext context, UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager) : base(context, userManager, signInManager, roleManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<ActionResult> Applications()
        {
            var webUser = await _userManager.GetUserAsync(User) as WebUser;

            if (webUser is null)
                return BadRequest();

            var model = _context.EventApplications.Where(x => x.ApplicationSender == webUser).ToList();


            return View(model);
        }
        public ActionResult About() => View();
    }
}
