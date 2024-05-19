using DiplomService.Database;
using DiplomService.Models;
using DiplomService.Models.OrganizationFolder;
using DiplomService.Models.Users;
using DiplomService.Services;
using DiplomService.ViewModels.Admin;
using DiplomService.ViewModels.AuthViewModels;
using DiplomService.ViewModels.DeleteItem;
using DiplomService.ViewModels.Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiplomService.Controllers.ServiceControllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : CabinetController
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AdminController(ApplicationContext context, UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager) : base(context, userManager, signInManager, roleManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public ActionResult Dashboard()
        {
            var model = new AdminDashboardViewModel
            {
                OrganizationsCount = _context.Organizations.Count(),
                UsersCount = _context.Users.Count(),
                EventsCount = _context.Events.Count(),
                FutureEventsCount = _context.Events.Where(x => x.DateOfStart > DateTime.Now).Count(),
                EnjoyersCount = _context.MobileUsers.Count(),
                ApplicationsCount = _context.OrganizationApplications.Where(x => !x.Checked).Count(),
            };

            var currentDate = DateTime.Now;

            var nearestEvent = _context.Events
                .Where(e => e.DateOfStart > currentDate)
                .OrderBy(e => e.DateOfStart)
                .FirstOrDefault();

            if (nearestEvent != null)
            {
                model.ClosestEvent = nearestEvent.Name;
            }
            else
            {
                model.ClosestEvent = "Отсутствуют предстоящие события";
            }

            var lastNews = _context.News.OrderBy(n => n.DateTime);
            if (lastNews.Any())
            {
                model.LastNewsName = lastNews.Last().Title;
            }
            else
            {
                model.LastNewsName = "Отсутсвуют новости";
            }

            return View(model);
        }
        public ActionResult Applications()
        {
            var model = _context.OrganizationApplications.OrderBy(x => x.Checked).ToList();
            return View(model);
        }
        public ActionResult Events()
        {
            var model = _context.Events.ToList();
            return View(model);
        }
        public ActionResult News()
        {
            var model = _context.News.ToList();
            return View(model);
        }
        public ActionResult Organizations()
        {
            var model = _context.Organizations.ToList();
            return View(model);
        }
        public async Task<IActionResult> EditApplication(Guid id)
        {
            if (_context.OrganizationApplications.Any())
            {
                var model = await _context.OrganizationApplications.FirstAsync(x => x.Id == id);

                if (model != null)
                {
                    return View(model);
                }
            }

            return NotFound();
        }
        public ActionResult AddAdmin()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> AddAdmin(AdministratorRegistrationViewModel registrationViewModel)
        {
            if (!ModelState.IsValid)
                return View(registrationViewModel);

            var user = new Administrator()
            {
                Email = registrationViewModel.Email,
                UserName = registrationViewModel.Email,
                NormalizedEmail = registrationViewModel.Email.ToUpper(),
                NormalizedUserName = registrationViewModel.Email.ToUpper(),
                Name = registrationViewModel.Name,
                SecondName = registrationViewModel.SecondName,
                LastName = registrationViewModel.LastName,
                PhoneNumber = PhoneWorker.NormalizePhone(registrationViewModel.PhoneNumber),
            };

            if (_context.Users.Any(x => x.PhoneNumber == user.PhoneNumber))
            {
                ModelState.AddModelError("", "Пользователь с таким номером телефона уже зарегистрирован");
                return View(registrationViewModel);
            }


            string password = Guid.NewGuid().ToString().Replace("-", string.Empty)[..5];
            password += password.ToUpper() + '!' + "aWeA";

            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Administrator");

                var messageVM = new UserRegistratedEmailViewModel()
                {
                    BaseUrl = $"{Request.Scheme}://{Request.Host}" + Url.Action("Login", "Account"),
                    Email = registrationViewModel.Email,
                    Password = password,
            
                };

                var renderer = HttpContext.RequestServices.GetRequiredService<IRazorViewToStringRenderer>();
                var htmlMessage = await renderer.RenderViewToStringAsync("HtmlTemplates/RegistrateUser", messageVM, HttpContext.RequestServices);

                SmtpService.SendAdminPassword(htmlMessage, messageVM);
                ViewBag.Sucess = "Пользователь зарегистрирован";
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(registrationViewModel);
        }


        [HttpPost]
        public async Task<ActionResult> EditApplication(Guid Id, string responseMessage, bool applicationApproved)
        {
            var model = await _context.OrganizationApplications.
                FirstOrDefaultAsync(x => x.Id == Id);

            if (model is null)
                return NotFound();

            model.ResponseMessage = responseMessage;

            if (ModelState.IsValid)
            {
                model.ApplicationApproved = applicationApproved;
                model.Checked = true;

                _context.Update(model);
                await _context.SaveChangesAsync();

                await HandleApplication(model, applicationApproved);

                return RedirectToAction(nameof(Applications));
            }

            return View(model);
        }

        private async Task HandleApplication(OrganizationApplication model, bool applicationApproved)
        {
            var messageVM = new ApplicationEmailViewModel()
            {
                ApplicationApproved = model.ApplicationApproved,
                OrganizationName = model.OrganizationName,
                DateOfSend = model.DateOfSend,
                ResponseMessage = model.ResponseMessage,
                EmailToSend = model.UserEmail
            };

            if (applicationApproved)
            {
                string password = Guid.NewGuid().ToString().Replace("-", string.Empty)[..5];

                password += password.ToUpper() + '!' + "aWeA";

                messageVM.BaseUrl = $"{Request.Scheme}://{Request.Host}" + Url.Action("Login", "Account");
                messageVM.Password = password;

                var organization = new Organization()
                {
                    Email = model.OrganizationEmail,
                    Name = model.OrganizationName,
                };

                await _context.Organizations.AddAsync(organization);
                await _context.SaveChangesAsync();

                var organizationUser = new OrganizationUsers()
                {
                    Email = model.UserEmail,
                    UserName = model.UserEmail,
                    Organization = organization,
                    OrganizationLeader = true
                };

                var r = await _userManager.CreateAsync(organizationUser, password);
                await _userManager.AddToRoleAsync(organizationUser, "OrganizationUser");
            }

            var renderer = HttpContext.RequestServices.GetRequiredService<IRazorViewToStringRenderer>();
            var htmlMessage = await renderer.RenderViewToStringAsync("HtmlTemplates/ApplicationResponse", messageVM, HttpContext.RequestServices);

            SmtpService.SendApplicationResponse(htmlMessage, messageVM);
        }


        public async Task<ActionResult> DeleteEvent(int id)
        {
            var @event = await _context.Events.FirstOrDefaultAsync(x => x.Id == id);
            if (@event is null)
                return NotFound();

            return View(@event);
        }
        [HttpPost]
        public async Task<ActionResult> DeleteEvent(int id, string reason)
        {
            var @event = await _context.Events.FirstOrDefaultAsync(x => x.Id == id);
            if (@event is null)
                return NotFound();

            if (!ModelState.IsValid)
                return View(@event);

            HandleEventDelete(@event, reason);
            await RemoveEvent(@event);

            if (!_context.Events.Any(x => x.Id == id))
            {
                ViewBag.ErrorMessage = "Произошла ошибка";
            }
            else
            {
                ViewBag.Message = "Удаление выполнено успешно";
            }

            return View(@event);
        }


        public async Task<ActionResult> DeleteOrganization(int id)
        {
            var @event = await _context.Organizations.FirstOrDefaultAsync(x => x.Id == id);
            if (@event is null)
                return NotFound();

            return View(@event);
        }
        [HttpPost]
        public async Task<ActionResult> DeleteOrganization(int id, string reason)
        {
            var organization = await _context.Organizations.FirstOrDefaultAsync(x => x.Id == id);
            if (organization is null)
                return NotFound();

            if (!ModelState.IsValid)
                return View(organization);

            HandleOrganization(organization, reason);

            _context.Organizations.Remove(organization);
            await _context.SaveChangesAsync();

            if (!_context.Events.Any(x => x.Id == id))
            {
                ViewBag.ErrorMessage = "Произошла ошибка";
            }
            else
            {
                ViewBag.Message = "Удаление выполнено успешно";
            }

            return View(organization);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteNews(int id)
        {
            var news = await _context.News.FirstOrDefaultAsync(x => x.Id == id);
            if (news is null) return NotFound();

            _context.News.Remove(news);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(News));
        }

        private async void HandleEventDelete(Event @event, string reason)
        {
            string? emailToSend = @event.Organizations.Select(x => x.Email).FirstOrDefault(x => x != string.Empty);

            if (emailToSend is null) return;

            DeleteItemViewModel viewModel = new()
            {
                ItemType = DeleteItemViewModel.ItemTypes.Event,
                Reason = reason,
                ItemName = @event.Name,
                EmailToSend = emailToSend,
            };

            var renderer = HttpContext.RequestServices.GetRequiredService<IRazorViewToStringRenderer>();
            var htmlMessage = await renderer.RenderViewToStringAsync("HtmlTemplates/DeleteItem", viewModel, HttpContext.RequestServices);

            SmtpService.SendRemoveReason(htmlMessage, viewModel);
        }

        private async void HandleOrganization(Organization organization, string reason)
        {
            string? emailToSend = organization.OrganizationUsers.Select(x => x.Email).FirstOrDefault(x => x != string.Empty);

            if (emailToSend is null) return;

            DeleteItemViewModel viewModel = new()
            {
                ItemType = DeleteItemViewModel.ItemTypes.Organization,
                Reason = reason,
                ItemName = organization.Name,
                EmailToSend = emailToSend,
            };

            var renderer = HttpContext.RequestServices.GetRequiredService<IRazorViewToStringRenderer>();
            var htmlMessage = await renderer.RenderViewToStringAsync("HtmlTemplates/DeleteItem", viewModel, HttpContext.RequestServices);

            SmtpService.SendRemoveReason(htmlMessage, viewModel);
        }


        private async Task RemoveEvent(Event @event)
        {
            foreach (var item in @event.Measures)
            {
                _context.MeasureDivisionsInfos.RemoveRange(item.MeasureDivisionsInfos);
                await _context.SaveChangesAsync();
            }

            @event.Measures.Clear();
            await _context.SaveChangesAsync();

            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();
        }
        private async Task RemoveOrganization(Organization organization)
        {
            _context.Organizations.Remove(organization);
            await _context.SaveChangesAsync();
        }
    }
}
