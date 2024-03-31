using DiplomService.Database;
using DiplomService.Models;
using DiplomService.Models.OrganizationFolder;
using DiplomService.Models.Users;
using DiplomService.Services;
using DiplomService.ViewModels;
using DiplomService.ViewModels.Admin;
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
                model.ClosestEvent = "Отсутсвуют предстоящие события";
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

        [HttpPost]
        public async Task<ActionResult> EditApplication(Guid Id, string responseMessage, string ApplicationApproved)
        {
            var model = await _context.OrganizationApplications.FirstAsync(x => x.Id == Id);
            model.ResponseMessage = responseMessage;

            if (ModelState.IsValid)
            {
                var applicationApproved = ApplicationApproved == "true";

                model.ApplicationApproved = applicationApproved;
                model.Checked = true;
                _context.Update(model);
                await _context.SaveChangesAsync();

                await HandleApplication(model, applicationApproved);

                return RedirectToAction(nameof(Applications));
            }

            return View(model);
        }
        async Task HandleApplication(OrganizationApplication model, bool applicationApproved)
        {
            var messageVM = new ApplicationEmailViewModel()
            {
                ApplicationApproved = model.ApplicationApproved,
                OrganizationName = model.OrganizationName,
                DateOfSend = model.DateOfSend,
                ResponseMessage = model.ResponseMessage,
                UserEmail = model.UserEmail,
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
                    Name = model.Name,
                    SecondName = model.SecondName,
                    LastName = model.LastName,
                    OrganizationLeader = true
                };

                var r = await _userManager.CreateAsync(organizationUser, password);
                await _userManager.AddToRoleAsync(organizationUser, "OrganizationUser");
            }

            var renderer = HttpContext.RequestServices.GetRequiredService<IRazorViewToStringRenderer>();
            var htmlMessage = await renderer.RenderViewToStringAsync("HtmlTemplates/ApplicationResponse", messageVM, HttpContext.RequestServices);

            SmtpService.SendApplicationResponse(htmlMessage, messageVM);
        }
    }
}
