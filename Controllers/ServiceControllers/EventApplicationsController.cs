using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DiplomService.Database;
using DiplomService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using DiplomService.Models.Users;
using DiplomService.ViewModels.EventApplication;
using DiplomService.Services;
using DiplomService.ViewModels;
using Microsoft.Extensions.Logging;

namespace DiplomService.Controllers
{
    public class EventApplicationsController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<User> _userManager;

        public EventApplicationsController(ApplicationContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "WebUser")]
        public async Task<IActionResult> Create(int Id)
        {
            if (await _userManager.GetUserAsync(User) is not WebUser user)
                return BadRequest();

            var @event = await _context.Events.FirstOrDefaultAsync(x => x.Id == Id);

            if (@event == null)
                return NotFound();

            var model = new EventApplicationViewModel()
            {
                EventId = Id,
                ApplicationDatas = new()
                {
                    new()
                    {
                        Birthday = DateTime.Now.AddYears(-16),
                    }
                },
                DivisionsExist = @event.DivisionsExist,
                ApplicationSenderId = user.Id,
                Divisions = @event.Divisions
            };


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "WebUser")]
        public async Task<IActionResult> Create(EventApplicationViewModel model)
        {
            var @event = await _context.Events.FirstOrDefaultAsync(x => x.Id == model.EventId);
            if (@event == null)
                return NotFound();
            model.Divisions = @event.Divisions;

            if (ModelState.IsValid)
            {
                if (await _userManager.GetUserAsync(User) is not Models.Users.WebUser user)
                    return BadRequest();

                Dictionary<string, string> phones = new();
                Dictionary<string, string> mails = new();

                foreach (var item in model.ApplicationDatas)
                {
                    item.PhoneNumber = PhoneWorker.NormalizePhone(item.PhoneNumber);
                    if (!phones.ContainsKey(item.PhoneNumber))
                    {
                        phones.Add(item.PhoneNumber, item.SecondName + " " + item.Name);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Номера телефонов " + item.SecondName + " " + item.Name + " и " + phones.First(x => x.Key == item.PhoneNumber).Value + " совпадают");
                        return View(model);
                    }

                    if (!phones.ContainsKey(item.Email))
                    {
                        mails.Add(item.Email, item.SecondName + " " + item.Name);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Почтовые адреса " + item.SecondName + " " + item.Name + " и " + mails.First(x => x.Key == item.Email).Value + " совпадают");
                        return View(model);
                    }

                    var userByEmail = _context.Users.FirstOrDefault(x => x.PhoneNumber == item.PhoneNumber);
                    var userByPhone = _context.Users.FirstOrDefault(x => x.Email == item.Email);

                    if ((userByEmail != userByPhone) && (userByEmail!=null && userByPhone!=null))
                    {
                        ModelState.AddModelError("", "Пользователь с почтовым адресом " + item.Email + " уже зарегистрирован и имеет иной номер телефона");
                        return View(model);
                    }
                    if (!@event.DivisionsExist)
                    {
                        item.Division = @event.Divisions[0];
                    }
                }
                
                var application = new EventApplication
                {
                    ApplicationData = model.ApplicationDatas,
                    ApplicationSender = user,
                    Email = user.Email,
                    Event = @event,
                    EventId = model.EventId,
                    ApplicationSenderId = model.ApplicationSenderId,
                    Institution = model.Institution,
                    TimeOfSend = DateTime.Now,
                };

                await _context.AddAsync(application);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            
            return View(model);
        }

        [Authorize(Roles = "OrganizationUser")]
        public async Task<IActionResult> Edit(int Id)
        {
            var application = await _context.EventApplications.FirstOrDefaultAsync(x => x.Id == Id);

            if (application == null)
                return NotFound();

            if (!await CheckOrgUserAccess(application))
                return BadRequest();

            return View(application);
        }
        [Authorize(Roles = "WebUser")]
        public async Task<IActionResult> Details(int Id)
        {
            var userId = _userManager.GetUserId(User);
            var application = await _context.EventApplications.FirstOrDefaultAsync(x => x.Id == Id && x.ApplicationSenderId == userId);

            if (application == null)
                return NotFound();

            string status = "";

            if (application.Accepted != null)
                if (application.Accepted.Value)
                    status = "Принята";
                else
                    status = "Отклонена";
            else
                status = "На рассмотрении";

            ViewBag.Status = status;

            return View(application);
        }

        [Authorize(Roles = "OrganizationUser,WebUser")]
        public async Task<IActionResult> Index()
        {
            User? user = await _userManager.GetUserAsync(User);
            if (user == null)
                return BadRequest();
            List<EventApplication>? application = null;
            if (user is OrganizationUsers)
            {
                int organizationId = ((OrganizationUsers)user).OrganizationId;
                application = _context.Organizations
                    .Where(org => org.Id == organizationId)
                    .SelectMany(org => org.Events.SelectMany(ev => ev.EventApplications))
                    .Where(x=>x.Accepted==null).ToList();
            }
            else
            {
                application = _context.EventApplications.Where(x => x.ApplicationSenderId == user.Id).OrderBy(x => x.Accepted != null).ToList();
            }
             

            if (application == null)
                return NotFound();

            return View(application);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "OrganizationUser")]
        public async Task<IActionResult> Edit(int Id,string comment, string action)
        {
            var application = await _context.EventApplications.FirstOrDefaultAsync(x => x.Id == Id);

            if (application == null)
                return NotFound();

            if (!await CheckOrgUserAccess(application))
                return BadRequest();

            bool accepted = action == "success";
            application.Accepted = accepted;

            await _context.SaveChangesAsync();

            var model = new ViewModels.Email.EventApplicationViewModel
            {
                Accepted= accepted,
                Comment= comment,
                Email = application.Email,
                BaseUrl = $"{this.Request.Scheme}://{this.Request.Host}" + Url.Action("Index", "Home")
            };

            var renderer = HttpContext.RequestServices.GetRequiredService<IRazorViewToStringRenderer>();
            var htmlMessage = await renderer.RenderViewToStringAsync("HtmlTemplates/EventApplicationResponce", model, HttpContext.RequestServices);

            SmtpService.SendEventApplicationResponce(htmlMessage, model);
            await CreateUsers(application);
            return RedirectToAction("Index", "Cabinet");
        }

        public async Task<IActionResult> GetParticipantEntry(int Id,int Index)
        {
            var @event = await _context.Events.FirstOrDefaultAsync(x => x.Id == Id);

            if (@event == null)
                return NotFound();

            var model = new ApplicationDataViewModel
            {
                ApplicationDatas = new(),
                Divisions = @event.Divisions,
                DivisionsExist = @event.DivisionsExist,
            };
            ViewBag.Index = Index;
            return PartialView("_ApplicationDataEntry", model);
        }

        public async Task<WebUser?> FindByPhoneNumberAsync(string phoneNumber)
        {
            return await _context.WebUsers.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        }
        private async Task<bool> CheckOrgUserAccess(EventApplication application)
        {
            var user = await _userManager.GetUserAsync(User) as OrganizationUsers;
            if (user!=null && user.Organization.Events.Contains(application.Event))
            {
                return true;
            }
            return false;
        }
        private async Task CreateUsers(EventApplication application)
        {
            foreach (var item in application.ApplicationData)
            {
                MobileUser? user  = await _context.MobileUsers.FirstOrDefaultAsync(x=>x.Email==item.Email);

                if (user==null)
                {
                    user = new MobileUser()
                    {
                        UserName = item.PhoneNumber,
                        SecondName = item.SecondName,
                        Name = item.Name,
                        LastName = item.LastName,
                        PhoneNumber = item.PhoneNumber,
                        Email = item.Email,
                        Birthday = item.Birthday,
                        Course = item.Course,
                    };

                    var result = await _userManager.CreateAsync(user);
                    if (result == IdentityResult.Success)
                    {
                        await _userManager.AddToRoleAsync(user, "MobileUser");
                    }

                }

                if (item.Division != null)
                {
                    user.UserDivisions.Add(new DivisionUsers()
                    {
                        User = user,
                        Division = item.Division,
                        DivisionDirector = false
                    });
                }
                else
                {
                    user.UserDivisions.Add(new DivisionUsers()
                    {
                        Division = application.Event.Divisions[0],
                        User = user,
                        DivisionDirector = false
                    });
                }

            }
            
            await _context.SaveChangesAsync();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteData(int Id)
        {
            var applicationData = await _context.ApplicationData.FirstOrDefaultAsync(x => x.Id == Id);
            if (applicationData!=null)
            {
                _context.Remove(applicationData);
                await _context.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
