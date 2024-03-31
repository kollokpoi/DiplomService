using DiplomService.Database;
using DiplomService.Models;
using DiplomService.Models.Users;
using DiplomService.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DiplomService.Controllers.ServiceControllers
{
    [Authorize]
    public class CabinetController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public CabinetController(ApplicationContext context, UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<ActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                var model = new UserViewModel
                {
                    Id = user.Id,
                    Name = user.Name,
                    SecondName = user.SecondName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Image = user.Image
                };
                return View(model);
            }
        }
        public ActionResult Index()
        {
            var us = User;
            var u = User.Identity;
            var cl = us.Claims.First(x=>x.Type == "AspNet.Identity.SecurityStamp");
            var r = _userManager.GetUsersForClaimAsync(cl).Result;
            if (User.IsInRole("Administrator"))
            {
                return RedirectToAction("Dashboard", "Admin");
            }
            else if (User.IsInRole("WebUser"))
            {
                return RedirectToAction("Edit");
            }
            else if (User.IsInRole("OrganizationUser"))
            {
                return RedirectToAction("Organization", "OrganizationUser");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public async Task<ActionResult> EditLogin()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                var model = new UserViewModel
                {
                    Name = user.Name,
                    SecondName = user.SecondName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                };
                return View(model);
            }
        }

        [Authorize]
        public async Task<ActionResult> EditPassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                var model = new UpdatePasswordViewModel
                {
                    Id= user.Id,
                };
                return View(model);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> EditPassword(UpdatePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user is null || user.Id != model.Id)
                return NotFound();

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.Password);

            if (result.Succeeded)
            {
                ViewBag.SuccessMessage = "Пароль успешно изменен";
                return View(model);
            }
            else
            {
                ModelState.AddModelError("","Ошибка обновления пароля");
                return View(model);
            }
                
        }


        [HttpPost]
        public async Task<ActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public async Task<ActionResult> Edit(UserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByIdAsync(model.Id);

            if (user != null)
            {
                user.Name = model.Name;
                user.SecondName = model.SecondName;
                user.LastName = model.LastName;
                user.Image = model.Image;

                if (model.Email != user.Email)
                {

                }
                if (model.PhoneNumber != user.PhoneNumber)
                {

                }

                _context.Update(user);
                await _context.SaveChangesAsync();
                ViewBag.Result = "Изменения сохранены";
                return View(model);
            }
            else
            {
                ViewBag.Error = "Произошла ошибка";
                return View(model);
            }
        }
    }
}
