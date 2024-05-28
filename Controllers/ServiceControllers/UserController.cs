using DiplomService.Database;
using DiplomService.Models;
using DiplomService.Models.Users;
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

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> List()
        {
            var model = _context.Users.ToList();
            var organizationUser = await _userManager.GetUserAsync(User) as Administrator;
            if (organizationUser is null)
                return BadRequest();

            model.Remove(organizationUser);
            return View(model);
        }
        [Authorize(Roles = "Administrator, OrganizationUser")]
        public async Task<IActionResult> Details(string Id)
        {
            var model = _context.Users.FirstOrDefault(x => x.Id == Id);
            if (model != null)
            {
                if (User.IsInRole("OrganizationUser"))
                {
                    var organizationUser = await _userManager.GetUserAsync(User) as OrganizationUsers;
                    if (organizationUser is null)
                        return BadRequest();

                    if (organizationUser == model)
                        return RedirectToAction("Edit", "Cabinet");


                    if (!organizationUser.Organization.OrganizationUsers.Contains(model))
                        return BadRequest();

                    if (organizationUser.OrganizationLeader)
                        ViewBag.organizationLeader = true;
                }
                return View(model);
            }
            return NotFound();
        }
        [Authorize(Roles = "OrganizationUser")]
        public async Task<IActionResult> DeleteUser(string Id)
        {
            var model = _context.OrganizationUsers.First(x => x.Id == Id);
            if (model != null)
            {
                var organizationUser = await _userManager.GetUserAsync(User) as OrganizationUsers;
                if (organizationUser is null)
                    return BadRequest();

                if (!organizationUser.Organization.OrganizationUsers.Contains(model))
                    return BadRequest();

                if (!organizationUser.OrganizationLeader)
                    return BadRequest();
                organizationUser.Organization.OrganizationUsers.Remove(model);

                await _userManager.SetLockoutEndDateAsync(model, DateTimeOffset.MaxValue);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Cabinet");
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> BlockUser(string id, string time)
        {
            time = time.ToLower();
            DateTimeOffset offset = DateTimeOffset.Now;
            if (time.Contains("day")) offset = offset.AddDays(1);
            else if (time.Contains("week")) offset = offset.AddDays(7);
            else if (time.Contains("mounth")) offset = offset.AddDays(31);
            else if (time.Contains("always")) offset = DateTimeOffset.MaxValue;
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
