using DiplomService.Database;
using DiplomService.Models;
using DiplomService.Models.Users;
using DiplomService.Services;
using DiplomService.ViewModels.AuthViewModels;
using DiplomService.ViewModels.Email;
using DiplomService.ViewModels.OrganizationViewModels;
using DiplomService.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiplomService.Controllers.ServiceControllers
{
    [Authorize(Roles = "OrganizationUser")]
    public class OrganizationUser : Controller
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<User> _userManager;

        public OrganizationUser(ApplicationContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Organization()
        {
            var organizationUser = await _userManager.GetUserAsync(User) as OrganizationUsers;
            if (organizationUser != null)
            {
                var organization = organizationUser.Organization;

                var model = new OrganizationViewModel()
                {
                    Organization = organization,
                    Editable = organizationUser.OrganizationLeader
                };
                return View(model);
            }

            return View();
        }

        public async Task<IActionResult> OrganizationEvents()
        {
            var organizationUser = await _userManager.GetUserAsync(User) as OrganizationUsers;

            if (organizationUser is null)
                return BadRequest();

            var organization = organizationUser.Organization;
            var model = _context.Events.Where(x => x.Organizations.Contains(organization)).ToList();

            return View(model);
        }

        public async Task<IActionResult> OrganizationUsers()
        {
            var organizationUser = await _userManager.GetUserAsync(User) as OrganizationUsers;

            if (organizationUser != null)
            {
                var organization = organizationUser.Organization;

                var model = organization.OrganizationUsers;
                model.OrderBy(x => x == organizationUser);
                return View(model);
            }
            return View();
        }
        public async Task<IActionResult> AddUser()
        {
            var organizationUser = await _userManager.GetUserAsync(User) as OrganizationUsers;

            if (organizationUser != null)
            {
                var organization = organizationUser.Organization;

                var model = new OrganizationUserViewModel()
                {
                    OrganizationId = organization.Id,
                };
                return View(model);
            }
            return View();
        }
        public IActionResult EndRegistration(int? id)
        {
            if (id == null || _context.Organizations == null)
            {
                return NotFound();
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(OrganizationUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    ModelState.AddModelError("", "Пользователь с таким почтовым адресом уже зарегистрирован");
                    return View(model);
                }
                if (_context.Users.Any(x => x.PhoneNumber == user.PhoneNumber))
                {
                    ModelState.AddModelError("", "Пользователь с таким номером телефона уже зарегистрирован");
                    return View(model);
                }

                var organization = await _context.Organizations.FirstOrDefaultAsync(o => o.Id == model.OrganizationId);
                if (organization == null)
                    return BadRequest();

                var organizationUser = new OrganizationUsers()
                {
                    LastName = model.LastName,
                    Name = model.Name,
                    SecondName = model.SecondName,
                    Email = model.Email,
                    NormalizedEmail = model.Email.ToUpper(),
                    UserName = model.Email,
                    NormalizedUserName = model.Email.ToUpper(),
                    OrganizationLeader = false,
                    Organization = organization,
                    PhoneNumber = model.PhoneNumber,
                    EmailConfirmed = true
                };

                string password = Guid.NewGuid().ToString().Replace("-", string.Empty)[..5];
                password += password.ToUpper() + '!' + "aWeA";

                var result = await _userManager.CreateAsync(organizationUser, password);

                if (!result.Succeeded)
                {
                    foreach (var item in result.Errors)
                    {
                        ViewBag.Message = item.Description;
                    }
                    return View(model);
                }

                await _userManager.AddToRoleAsync(organizationUser, "OrganizationUser");
                await _userManager.AddToRoleAsync(organizationUser, "MobileUser");

                var messageVm = new UserRegistratedEmailViewModel()
                {
                    Password = password,
                    Email = model.Email,
                    OrganizationName = organization.Name,
                };

                await HandleApplication(messageVm);

                model.Sended = true;
                ViewBag.Message = "Пользователь зарегистрирован. Пароль отправлен на указанную почту";
                return View(model);
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Organization(OrganizationViewModel model)
        {
            var organizationUser = await _userManager.GetUserAsync(User) as OrganizationUsers;
            if (organizationUser is null)
                return Unauthorized();
            var org = await _context.Organizations.FirstOrDefaultAsync(x => x.Id == model.Organization.Id);
            model.Editable = true;
            if (ModelState.IsValid && org != null)
            {

                if (!org.OrganizationUsers.Contains(organizationUser) && !organizationUser.OrganizationLeader)
                {
                    return Unauthorized();
                }

                if (model.Organization.Preview is not null)
                {
                    org.Preview = model.Organization.Preview;
                }
                else
                {
                    model.Organization.Preview = org.Preview;
                }


                org.Name = model.Organization.Name;
                org.Email = model.Organization.Email;
                org.Description = model.Organization.Description;
                org.ReadyToShow = true;
                _context.Update(org);
                await _context.SaveChangesAsync();
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EndRegistration(EndRegistrationViewModel organizationViewModel)
        {
            var user = await _userManager.GetUserAsync(User) as OrganizationUsers;
            if (user is null)
                return NotFound();

            var organization = user.Organization;
            await _userManager.AddToRoleAsync(user, "MobileUser");
            if (ModelState.IsValid)
            {
                organization.ReadyToShow = true;
                user.PhoneNumber = PhoneWorker.NormalizePhone(organizationViewModel.PhoneNumber);
                user.Name = organizationViewModel.Name;
                user.SecondName = organizationViewModel.SecondName;
                user.LastName = organizationViewModel.LastName;
                user.Image = organizationViewModel.Image;

                if (_context.Users.Any(x => x.PhoneNumber == user.PhoneNumber))
                {
                    ModelState.AddModelError("", "Пользователь с таким номером телефона уже зарегистрирован");
                    return View(organizationViewModel);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Organization", "OrganizationUser");
            }
            return View();
        }

        private async Task HandleApplication(UserRegistratedEmailViewModel messageVm)
        {
            messageVm.BaseUrl = $"{Request.Scheme}://{Request.Host}" + Url.Action("Login", "Account");

            var renderer = HttpContext.RequestServices.GetRequiredService<IRazorViewToStringRenderer>();
            var htmlMessage = await renderer.RenderViewToStringAsync("HtmlTemplates/RegistrateUser", messageVm, HttpContext.RequestServices);

            SmtpService.SendUserRegistration(htmlMessage, messageVm);
        }
    }
}
