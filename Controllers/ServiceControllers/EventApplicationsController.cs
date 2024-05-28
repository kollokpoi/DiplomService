using DiplomService.Database;
using DiplomService.Models;
using DiplomService.Models.Users;
using DiplomService.Services;
using DiplomService.ViewModels.EventApplication;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

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

            if (@event is null)
                return NotFound();

            var model = new EventApplicationViewModel()
            {
                EventId = Id,
                DivisionsExist = @event.DivisionsExist,
                Divisions = @event.Divisions
            };
            model.ApplicationDatas.Add(new());

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "WebUser")]
        public async Task<IActionResult> Create(int id,List<ApplicationDataViewModel> applicationDatas)
        {
            var @event = await _context.Events.FirstOrDefaultAsync(x => x.Id == id);
            if (@event is null)
                return NotFound();

            var model = new EventApplicationViewModel()
            {
                EventId = @event.Id,
                DivisionsExist = @event.DivisionsExist,
                Divisions = @event.Divisions,
                ApplicationDatas = applicationDatas
            };

            if (!ModelState.IsValid)
                return View(model);

            if (await _userManager.GetUserAsync(User) is not Models.Users.WebUser user)
                return BadRequest();

            var application = new EventApplication
            {
                ApplicationSender = user,
                Email = user.Email,
                Event = @event,
                EventId = id,
                TimeOfSend = DateTime.Now,
                Institution = user.WorkingPlace,
            };

            for (int i = 0; i < applicationDatas.Count; i++)
            {
                var applicationData = new ApplicationData();
                if (applicationDatas[i].UserId is null)
                {
                    var nameParts = applicationDatas[i].Name.Split();
                    if (applicationDatas[i].Course is null)
                        ModelState.AddModelError($"ApplicationDatas[{i}].Course", "Укажите класс/курс");

                    if (applicationDatas[i].Email is null)
                        ModelState.AddModelError($"ApplicationDatas[{i}].Email", "Укажите почтовый адрес");
                    else if (_context.Users.Any(x => x.Email == applicationDatas[i].Email))
                        ModelState.AddModelError($"ApplicationDatas[{i}].Email", "Пользователь с указаной почтой уже зарегистрирован");
                    else if (applicationDatas.Any(x => x.Email is not null && x != applicationDatas[i] && applicationDatas[i].Email == x.Email))
                        ModelState.AddModelError($"ApplicationDatas[{i}].Email", "Дублирование электронной почты");

                    if (applicationDatas[i].Birthday is null)
                        ModelState.AddModelError($"ApplicationDatas[{i}].Birthday", "Укажите дату рождения");
                    if (applicationDatas[i].PhoneNumber is null)
                        ModelState.AddModelError($"ApplicationDatas[{i}].PhoneNumber", "Укажите номер телефона");
                    else if (_context.Users.Any(x => x.PhoneNumber == PhoneWorker.NormalizePhone(applicationDatas[i].PhoneNumber)))
                        ModelState.AddModelError($"ApplicationDatas[{i}].PhoneNumber", "Пользователь с указанным номером телефона зарегистрирован");
                    else if (applicationDatas.Any(x=> x.PhoneNumber is not null && x!= applicationDatas[i] && PhoneWorker.NormalizePhone(x.PhoneNumber) == PhoneWorker.NormalizePhone(applicationDatas[i].PhoneNumber)))
                        ModelState.AddModelError($"ApplicationDatas[{i}].PhoneNumber", "Дублирование номеров телефона");

                    if (nameParts.Length!=3)
                        ModelState.AddModelError($"ApplicationDatas[{i}].Name", "Укажите полное фио");


                    if (@event.DivisionsExist)
                    {
                        if (applicationDatas[i].DivisionId is null)
                            ModelState.AddModelError($"ApplicationDatas[{i}].DivisionId", "Укажите подразделение");
                        else
                        {
                            var division = await _context.Divisions.FirstOrDefaultAsync(x => x.Id == applicationDatas[i].DivisionId);
                            if (division is null)
                                ModelState.AddModelError($"ApplicationDatas[{i}].DivisionId", "Укажите подразделение");
                            else
                                applicationData.Division = division;
                        }
                            
                    }
                    else
                    {
                        applicationData.DivisionId = @event.Divisions[0].Id;
                        applicationData.Division = @event.Divisions.First();
                    }
                    


                    if (!ModelState.IsValid)
                        return View(model);

                    
                    applicationData.Application = application;
                    applicationData.Course = applicationDatas[i].Course ?? 0;
                    applicationData.Email = applicationDatas[i].Email ?? "";
                    applicationData.Birthday = applicationDatas[i].Birthday ?? DateTime.Now;
                    applicationData.Name = nameParts[1];
                    applicationData.SecondName = nameParts[0];
                    applicationData.LastName = nameParts[2];
                    applicationData.PhoneNumber = PhoneWorker.NormalizePhone(applicationDatas[i].PhoneNumber ?? "");

                    application.ApplicationData.Add(applicationData);
                }
                else
                {
                    var mobileUser = await _userManager.FindByIdAsync(applicationDatas[i].UserId) as MobileUser;
                    if (mobileUser is null)
                        ModelState.AddModelError($"ApplicationDatas[{i}].Name", "Выберите из списка");

                    if (!ModelState.IsValid || mobileUser is null)
                        return View(model);

                    applicationData.Application = application;
                    applicationData.Course = mobileUser.Course;
                    applicationData.Email = mobileUser.Email ?? "";
                    applicationData.Birthday = mobileUser.Birthday;
                    applicationData.Name = mobileUser.Name;
                    applicationData.SecondName = mobileUser.SecondName;
                    applicationData.LastName = mobileUser.LastName;
                    applicationData.PhoneNumber = mobileUser.PhoneNumber ?? "";
                    applicationData.User = mobileUser;
                    applicationData.UserId = mobileUser.Id;

                    if (@event.DivisionsExist)
                    {
                        if (applicationDatas[i].DivisionId is null)
                            ModelState.AddModelError($"ApplicationDatas[{i}].DivisionId", "Укажите подразделение");
                        else
                        {
                            var division = await _context.Divisions.FirstOrDefaultAsync(x => x.Id == applicationDatas[i].DivisionId);
                            if (division is null)
                                ModelState.AddModelError($"ApplicationDatas[{i}].DivisionId", "Укажите подразделение");
                            else
                                applicationData.Division = division;
                        }

                    }
                    else
                    {
                        applicationData.DivisionId = @event.Divisions[0].Id;
                        applicationData.Division = @event.Divisions.First();
                    }
                    applicationData.Application = application;
                    application.ApplicationData.Add(applicationData);
                }
            }
            
            await _context.AddAsync(application);
            await _context.SaveChangesAsync();
            
            return RedirectToAction("Index");

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
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "OrganizationUser")]
        public async Task<IActionResult> Edit(int Id, string comment, string action)
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
                Accepted = accepted,
                Comment = comment,
                Email = application.Email,
                BaseUrl = $"{this.Request.Scheme}://{this.Request.Host}" + Url.Action("Index", "Home")
            };

            var renderer = HttpContext.RequestServices.GetRequiredService<IRazorViewToStringRenderer>();
            var htmlMessage = await renderer.RenderViewToStringAsync("HtmlTemplates/EventApplicationResponce", model, HttpContext.RequestServices);

            SmtpService.SendEventApplicationResponce(htmlMessage, model);
            if (accepted)
                await CreateUsers(application);
            
            return RedirectToAction("Index", "Cabinet");
        }



        [Authorize(Roles = "WebUser")]
        public async Task<IActionResult> Details(int Id)
        {
            var userId = _userManager.GetUserId(User);
            var application = await _context.EventApplications.FirstOrDefaultAsync(
                x => x.Id == Id && x.ApplicationSenderId == userId);

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
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return BadRequest();

            List<EventApplication>? application = null;
            if (user is OrganizationUsers)
            {
                int organizationId = ((OrganizationUsers)user).OrganizationId;
                application = _context.Organizations
                    .Where(org => org.Id == organizationId)
                    .SelectMany(org => org.Events.SelectMany(ev => ev.EventApplications))
                    .Where(x => x.Accepted == null).ToList();
            }
            else
            {
                application = _context.EventApplications.Where(x => x.ApplicationSenderId == user.Id).OrderBy(x => x.Accepted != null).ToList();
            }


            if (application == null)
                return NotFound();

            return View(application);
        }



        public async Task<WebUser?> FindByPhoneNumberAsync(string phoneNumber)
        {
            return await _context.WebUsers.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        }
        private async Task<bool> CheckOrgUserAccess(EventApplication application)
        {
            var user = await _userManager.GetUserAsync(User) as OrganizationUsers;
            if (user != null && user.Organization.Events.Contains(application.Event))
            {
                return true;
            }
            return false;
        }
        private async Task CreateUsers(EventApplication application)
        {
            foreach (var item in application.ApplicationData)
            {
                if (item.User is not null)
                {
                    if (!_context.DivisionUsers.Any(x=>x.User==item.User && x.Division == item.Division))
                    {
                        _context.DivisionUsers.Add(new()
                        {
                            Division = item.Division,
                            User = item.User,
                        });
                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    var user = new MobileUser()
                    {
                        Birthday = item.Birthday,
                        Course = item.Course,
                        Email = item.Email,
                        NormalizedEmail = item.Email.ToUpper(),
                        LastName = item.LastName,
                        Name = item.Name,
                        SecondName = item.SecondName,
                        UserName = item.PhoneNumber,
                        NormalizedUserName = item.PhoneNumber,
                        PhoneNumber = item.PhoneNumber,
                    };
                    var regResult = await _userManager.CreateAsync(user);
                    if (regResult.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "MobileUser");
                        _context.DivisionUsers.Add(new()
                        {
                            Division = item.Division,
                            User = user
                        });
                        await _context.SaveChangesAsync();
                    }
                }

            }

            var divisions = application.ApplicationData.Select(x => x.Division).Distinct().ToList();
            foreach (var item in divisions)
            {
                if (!_context.DivisionUsers.Any(x => x.Division == item && x.UserId == application.ApplicationSenderId))
                {
                    _context.DivisionUsers.Add(new DivisionUsers()
                    {
                        Division = item,
                        DivisionDirector = false,
                        UserId = application.ApplicationSenderId
                    });
                }
            }


            await _context.SaveChangesAsync();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteData(int Id)
        {
            var applicationData = await _context.ApplicationData.FirstOrDefaultAsync(x => x.Id == Id);
            if (applicationData != null)
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

        public async Task<IActionResult> GetParticipantEntry(int Id, int Index)
        {
            var @event = await _context.Events.FirstOrDefaultAsync(x => x.Id == Id);

            if (@event == null)
                return NotFound();

            var model = new EventApplicationViewModel
            {
                ApplicationDatas = new(),
                Divisions = @event.Divisions,
                DivisionsExist = @event.DivisionsExist,
            };
            ViewBag.Index = Index;
            return PartialView("_ApplicationDataEntry", model);
        }
    }
}
