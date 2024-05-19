using DiplomService.Database;
using DiplomService.Models;
using DiplomService.Models.OrganizationFolder;
using DiplomService.Models.Users;
using DiplomService.Services;
using DiplomService.ViewModels.AuthViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DiplomService.Controllers.ServiceControllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(ApplicationContext context, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public ActionResult Login() => View();
        public ActionResult Registration() => View();
        public ActionResult OrganizationRegistration() => View();



        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, true, false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user != null)
                    {

                        if (await _userManager.IsInRoleAsync(user, "OrganizationUser"))
                        {
                            if (user is not OrganizationUsers realUser)
                                return BadRequest();

                            var organization = realUser.Organization;
                            if (organization != null)
                            {
                                if (!organization.ReadyToShow)
                                {
                                    return RedirectToAction("EndRegistration", "OrganizationUser", new { organization.Id });
                                }
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    if (result.IsLockedOut)
                    {
                        var user = await _userManager.FindByEmailAsync(model.Email);
                        if (user != null)
                            if (user.LockoutEnd != null)
                                ModelState.AddModelError("", $"Вы заблокированы до {user.LockoutEnd.Value.Date.ToShortDateString()}");

                    }
                    else
                        ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Registration(RegistrationViewModel registrationViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = new WebUser()
                {
                    Email = registrationViewModel.Email,
                    UserName = registrationViewModel.Email,
                    NormalizedEmail = registrationViewModel.Email.ToUpper(),
                    NormalizedUserName = registrationViewModel.Email.ToUpper(),
                    Name = registrationViewModel.Name,
                    SecondName = registrationViewModel.SecondName,
                    LastName = registrationViewModel.LastName,
                    PhoneNumber = PhoneWorker.NormalizePhone(registrationViewModel.PhoneNumber),
                    WorkingPlace = registrationViewModel.WorkingPlace,
                };

                if (_context.Users.Any(x => x.PhoneNumber == user.PhoneNumber))
                {
                    ModelState.AddModelError("", "Пользователь с таким номером телефона уже зарегистрирован");
                    return View(registrationViewModel);
                }
                    

                var result = await _userManager.CreateAsync(user, registrationViewModel.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "WebUser");
                    await _signInManager.SignInAsync(user, false);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(registrationViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> OrganizationRegistration(OrganizationRegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var application = new OrganizationApplication()
                {
                    OrganizationName = model.OrganizationName,
                    OrganizationEmail = model.OrganizationEmail,
                    UserEmail = model.UserEmail,
                    Message = model.Message,
                };

                if (_context.Organizations.Any(x => x.Email == application.OrganizationEmail))
                {
                    ModelState.AddModelError("", "Организация с таким почтовым адресом уже существует");
                    return View(model);
                }
                if (_context.Users.Any(x => x.Email == application.UserEmail))
                {
                    ModelState.AddModelError("", "Данная почта занята другим пользователем");
                    return View(model);
                }

                await _context.OrganizationApplications.AddAsync(application);
                await _context.SaveChangesAsync();

                ViewBag.Message = $"Спасибо за оставленную заявку. В скором времени вы получите ответ на {model.OrganizationEmail}.";
                model.Sended = true;

                return View(model);
            }

            return View(model);

        }
    }
}
