using DiplomService.Database;
using DiplomService.Models;
using DiplomService.Models.EventsFolder.Division;
using DiplomService.Models.Users;
using DiplomService.ViewModels.Event;
using DiplomService.ViewModels.Measures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DiplomService.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EventsController(ApplicationContext context, UserManager<User> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Events.Where(x => x.ReadyToShow && x.PublicEvent && (x.DateOfEnd == null || x.DateOfEnd > DateTime.Now)).ToListAsync());
        }
        public async Task<IActionResult> Details(int id)
        {
            var @event = await _context.Events.FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
                return NotFound();

            EventViewModel eventViewModel = new()
            {
                Id = @event.Id,
                Name = @event.Name,
                Description = @event.Description,
                DateOfStart = @event.DateOfStart,
                DateOfEnd = @event.DateOfEnd,
                PriviewImage = @event.PriviewImage,
                PublicEvent = @event.PublicEvent,
                Divisions = @event.Divisions,
                Measures = @event.Measures,
                DivisionsExist = @event.DivisionsExist,
                Organizations = @event.Organizations,
            };
            if (await UserAccessed(@event))
                ViewBag.Editer = true;
            if (!@event.DivisionsExist)
            {
                eventViewModel.MeasuresViewModel = GetEventMeasures(@event);
            }
            return View(eventViewModel);
        }
        [Authorize(Roles = "OrganizationUser")]
        public IActionResult GetUserSuggestTemplate(int index)
        {
            return PartialView("UserSuggestTemplate", index);
        }


        [Authorize(Roles = "OrganizationUser")]
        public IActionResult Create(int? id)
        {
            if (id is not null)
            {
                return RedirectToAction(nameof(Edit), new { id = id });
            }
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "OrganizationUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventViewModel EventViewModel)
        {
            if (ModelState.IsValid)
            {
                var newEvent = new Event()
                {
                    Name = EventViewModel.Name,
                    Description = EventViewModel.Description,
                    DateOfStart = EventViewModel.DateOfStart,
                    DateOfEnd = EventViewModel.DateOfEnd,
                    PriviewImage = EventViewModel.PriviewImage,
                    PublicEvent = EventViewModel.PublicEvent,
                    DivisionsExist = EventViewModel.DivisionsExist,
                };
                _context.Events.Add(newEvent);

                var organizationUser = await _userManager.GetUserAsync(User) as OrganizationUsers;

                if (organizationUser != null)
                {
                    var organization = organizationUser.Organization;

                    if (organization != null)
                        newEvent.Organizations.Add(organization);
                }

                if (!newEvent.DivisionsExist)
                {
                    if (EventViewModel.PlaceName == string.Empty || EventViewModel.Latitude is null || EventViewModel.Longitude is null)
                    {
                        ModelState.AddModelError("", "Укажите место проведения");
                        return View(EventViewModel);
                    }
                    var division = new Division
                    {
                        DateOfStart = EventViewModel.DateOfStart,
                        Name = "mainDivision",
                        Event = newEvent,
                        Longitude = EventViewModel.Longitude.Value,
                        Latitude = EventViewModel.Latitude.Value,
                        PlaceName = EventViewModel.PlaceName ?? ""
                    };
                    foreach (var item in EventViewModel.DivisionLeaders)
                    {
                        var user = await _userManager.FindByIdAsync(item.Id);
                        if (user is null)
                            continue;

                        await _userManager.AddToRoleAsync(user, "MobileUser");

                        division.DivisionMembers.Add(new()
                        {
                            Division = division,
                            DivisionDirector = true,
                            User = user
                        });

                    }
                    newEvent.Divisions.Add(division);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Index", controllerName: "Measures", new { Id = newEvent.Id });
            }
            return View(EventViewModel);
        }


        [Authorize(Roles = "OrganizationUser")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Events == null)
            {
                return NotFound();
            }

            var @event = await _context.Events.FindAsync(id);

            if (@event == null)
                return NotFound();
            if (!await UserAccessed(@event))
                return BadRequest();
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var organizationUser = await _context.OrganizationUsers.FirstOrDefaultAsync(x => x.Id == currentUserId);

            if (organizationUser != null)
            {
                var organization = organizationUser.Organization;

                if (organization != null)
                    if (!@event.Organizations.Contains(organization))
                        return Forbid();

            }
            EventViewModel eventViewModel = new()
            {
                Id = @event.Id,
                Name = @event.Name,
                Description = @event.Description,
                DateOfStart = @event.DateOfStart,
                DateOfEnd = @event.DateOfEnd,
                PriviewImage = @event.PriviewImage,
                PublicEvent = @event.PublicEvent,
                Divisions = @event.Divisions,
                Measures = @event.Measures,
                DivisionsExist = @event.DivisionsExist,
            };

            if (!@event.DivisionsExist)
            {
                eventViewModel.Latitude = @event.Divisions[0].Latitude;
                eventViewModel.Longitude = @event.Divisions[0].Longitude;
                eventViewModel.PlaceName = @event.Divisions[0].PlaceName;
                var directors = @event.Divisions[0].DivisionMembers.Where(x => x.DivisionDirector);
                foreach (var item in directors)
                {
                    eventViewModel.DivisionLeaders.Add(new()
                    {
                        Id = item.UserId,
                        UserName = item.User.GetFullName()
                    });
                }
            }

            return View(eventViewModel);
        }
        [HttpPost]
        [Authorize(Roles = "OrganizationUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EventViewModel EventViewModel)
        {
            if (id != EventViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var @event = await _context.Events.FirstOrDefaultAsync(x => x.Id == id);

                if (@event == null)
                    return NotFound();
                if (!await UserAccessed(@event))
                    return BadRequest();

                @event.Name = EventViewModel.Name;
                @event.Description = EventViewModel.Description;
                @event.DateOfStart = EventViewModel.DateOfStart;
                @event.DateOfEnd = EventViewModel.DateOfEnd;
                @event.PublicEvent = EventViewModel.PublicEvent;

                if (EventViewModel.PriviewImage != null)
                    @event.PriviewImage = EventViewModel.PriviewImage;

                if (@event.DivisionsExist != EventViewModel.DivisionsExist)
                {

                    foreach (var item in @event.Measures)
                        _context.RemoveRange(item.MeasureDivisionsInfos);

                    @event.Measures.Clear();
                    await _context.SaveChangesAsync();

                    if (@event.EventApplications != null)
                        @event.EventApplications.Clear();
                    await _context.SaveChangesAsync();

                    @event.Divisions.Clear();
                    await _context.SaveChangesAsync();

                    @event.DivisionsExist = EventViewModel.DivisionsExist;

                    if (!@event.DivisionsExist)
                    {
                        if (EventViewModel.PlaceName == string.Empty || EventViewModel.Latitude is null || EventViewModel.Longitude is null)
                        {
                            ModelState.AddModelError("", "Укажите место проведения");
                            return View(EventViewModel);
                        }

                        var division = new Division
                        {
                            DateOfStart = EventViewModel.DateOfStart,
                            Name = "mainDivision",
                            Event = @event,
                            Longitude = EventViewModel.Longitude.Value,
                            Latitude = EventViewModel.Latitude.Value,
                            PlaceName = EventViewModel.PlaceName ?? ""
                        };
                        foreach (var item in EventViewModel.DivisionLeaders)
                        {
                            var user = await _userManager.FindByIdAsync(item.Id);
                            if (user is null)
                                continue;

                            await _userManager.AddToRoleAsync(user, "MobileUser");

                            division.DivisionMembers.Add(new()
                            {
                                Division = division,
                                DivisionDirector = true,
                                User = user
                            });

                        }
                        @event.Divisions.Add(division);

                    }
                }
                else if (!@event.DivisionsExist)
                {
                    if (EventViewModel.PlaceName == string.Empty || EventViewModel.Latitude is null || EventViewModel.Longitude is null)
                    {
                        ModelState.AddModelError("", "Укажите место проведения");
                        return View(EventViewModel);
                    }

                    @event.Divisions[0].Longitude = EventViewModel.Longitude.Value;
                    @event.Divisions[0].Latitude = EventViewModel.Latitude.Value;
                    @event.Divisions[0].PlaceName = EventViewModel.PlaceName ?? "";

                    var divisionLeadersIds = @event.Divisions[0].DivisionMembers
                        .Where(x => x.DivisionDirector)
                        .Select(x => x.UserId);

                    var divisionLeadersToDelete = divisionLeadersIds.Except(EventViewModel.DivisionLeaders.Select(x => x.Id)).ToList();
                    var divisionLeadersToAdd = EventViewModel.DivisionLeaders.Select(x => x.Id).Except(divisionLeadersIds).ToList();

                    foreach (var userId in divisionLeadersToDelete)
                    {
                        var divisionMemder = @event.Divisions[0].DivisionMembers.FirstOrDefault(x => x.UserId == userId);
                        if (divisionMemder != null)
                            @event.Divisions[0].DivisionMembers.Remove(divisionMemder);
                        _context.SaveChanges();
                    }
                    foreach (var userId in divisionLeadersToAdd)
                    {
                        var user = await _userManager.FindByIdAsync(userId);
                        if (user is null)
                            continue;

                        await _userManager.AddToRoleAsync(user, "MobileUser");

                        if (!@event.Divisions[0].DivisionMembers.Any(x => x.User == user))
                        {
                            @event.Divisions[0].DivisionMembers.Add(new()
                            {
                                Division = @event.Divisions[0],
                                DivisionDirector = true,
                                User = user
                            });
                        }

                    }

                }
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var organizationUser = await _context.OrganizationUsers.FirstOrDefaultAsync(x => x.Id == currentUserId);

                if (organizationUser != null)
                {
                    var organization = organizationUser.Organization;

                    if (organization != null)
                    {
                        if (!@event.Organizations.Contains(organization))
                        {
                            @event.Organizations.Add(organization);
                        }
                    }
                }

                await _context.SaveChangesAsync();

                return RedirectToAction("Index", controllerName: "Measures", new { Id = @event.Id });
            }
            return View(EventViewModel);
        }


        [Authorize(Roles = "OrganizationUser")]
        public async Task<IActionResult> EditUser(string id, int eventId)
        {
            var @event = await _context.Events.FirstOrDefaultAsync(x => x.Id == eventId);
            if (@event is null)
                return NotFound();

            if (!await UserAccessed(@event))
                return BadRequest();

            var user = await _context.MobileUsers.FirstOrDefaultAsync(x => x.Id == id);
            if (user is null)
                return NotFound();

            user.UserDivisions = user.UserDivisions.Where(x => x.Division.Event == @event).ToList();
            ViewBag.eventId = eventId;
            return View(user);
        }
        [HttpPost]
        [Authorize(Roles = "OrganizationUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(int eventId, MobileUser user)
        {
            var @event = await _context.Events.FirstOrDefaultAsync(x => x.Id == eventId);
            if (@event is null)
                return NotFound();

            if (!await UserAccessed(@event))
                return BadRequest();

            foreach (var item in user.UserDivisions)
            {
                var userDivision = await _context.DivisionUsers.FirstOrDefaultAsync(x => x.Id == item.Id);
                if (userDivision is not null)
                {
                    if (userDivision.DivisionDirector != item.DivisionDirector)
                    {
                        userDivision.DivisionDirector = item.DivisionDirector;
                        await _context.SaveChangesAsync();
                    }

                }
            }

            return RedirectToAction(nameof(EventUsers), new { id = eventId });
        }



        [Authorize(Roles = "OrganizationUser")]
        public async Task<IActionResult> Delete(int? id, string? returnUrl)
        {
            if (id == null || _context.Events == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .FirstOrDefaultAsync(m => m.Id == id);


            if (@event is null)
                return NotFound();

            if (!await UserAccessed(@event))
                return BadRequest();

            await RemoveEvent(@event);

            if (returnUrl is null)
                return RedirectToAction(nameof(Index));
            return Redirect(returnUrl);

        }

        [Authorize(Roles = "OrganizationUser")]
        public async Task<IActionResult> Publish(int id)
        {
            var @event = await _context.Events.FirstOrDefaultAsync(x => x.Id == id);
            if (@event != null)
            {
                string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", "CreatedImage.png");
                if (@event.PublicEvent)
                {
                    var createNews = new News()
                    {
                        Event = @event,
                        EventId = @event.Id,
                        DateTime = DateTime.Now,
                        Description = $"Новое событие {@event.Name} добавлено. Нажмите для подробностей",
                        Image = System.IO.File.ReadAllBytes(imagePath),
                        Title = $"Новое событие {@event.Name}",
                    };
                    @event.News.Add(createNews);
                }

                @event.ReadyToShow = true;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return BadRequest();
            }
        }

        [Authorize(Roles = "OrganizationUser")]
        public async Task<IActionResult> EventUsers(int id)
        {
            var @event = await _context.Events.FirstOrDefaultAsync(x => x.Id == id);
            if (@event is null)
                return NotFound();

            if (!await UserAccessed(@event))
                return BadRequest();

            ViewBag.eventId = @event.Id;
            if (@event.DivisionsExist)
            {
                var model = @event.Divisions;

                return View("DivisionsUsers", model);
            }
            else
            {
                var model = @event.Divisions[0].DivisionMembers.Select(x => x.User);

                return View("NoDivisionsUsers", model);
            }
        }


        [HttpPost]
        [Authorize(Roles = "OrganizationUser")]
        public async Task<IActionResult> RemoveFromDivision(int divisionId, int eventId)
        {
            var @event = await _context.Events.FirstOrDefaultAsync(x => x.Id == eventId);
            if (@event is null)
                return NotFound();

            if (!await UserAccessed(@event))
                return BadRequest();

            var userDivision = await _context.DivisionUsers.FirstOrDefaultAsync(x => x.Id == divisionId);
            if (userDivision is null)
                return NotFound();

            _context.DivisionUsers.Remove(userDivision);
            await _context.SaveChangesAsync();

            return Ok();
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

            @event.Divisions.ForEach(d=>_context.ApplicationData.RemoveRange(_context.ApplicationData.Where(x=>x.DivisionId==d.Id)));
            await _context.SaveChangesAsync();

            @event.Divisions.Clear();
            await _context.SaveChangesAsync();

            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();
        }
        private async Task<bool> UserAccessed(Event @event)
        {
            if (User.IsInRole("OrganizationUser"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user is not null)
                    if (@event.Organizations.Contains(((Models.Users.OrganizationUsers)user).Organization))
                        return true;
                return false;
            }
            return false;
        }

        private List<EventMeasuresViewModel> GetEventMeasures(Event @event)
        {
            List<EventMeasuresViewModel> viewModel = new List<EventMeasuresViewModel>();
            List<MeasureDivisionsInfo> measuresForDivision = new List<MeasureDivisionsInfo>();
            for (int i = 0; i < @event.Divisions.Count; i++)
            {
                if (i == 0)
                {
                    measuresForDivision.AddRange(@event.Measures.Where(x => x.SameForAll)
                          .SelectMany(x => x.MeasureDivisionsInfos).ToList());
                }
                measuresForDivision.AddRange(@event.Divisions[i].MeasureDivisionsInfos);
            }

            foreach (var measure in measuresForDivision)
            {
                var viewModelItem = new EventMeasuresViewModel()
                {
                    Id = measure.Id,
                    EventName = measure.Measure.Name,
                };
                if (measure.WeekDays)
                {
                    viewModelItem.DateTime = ApiContollers.MeasuresController.GetNearestDate(measure.MeasureDays);
                }
                else
                {
                    viewModelItem.DateTime = ApiContollers.MeasuresController.GetNearestDate(measure.MeasureDates);
                }
                viewModelItem.Icon = measure.Measure.Icon;
                viewModelItem.Length = measure.Length;
                if (viewModelItem.DateTime>DateTime.Now)
                {
                    viewModel.Add(viewModelItem);
                }
            }

            viewModel = viewModel.OrderBy(x => x.DateTime).ToList();
            return viewModel;
        }
    }
}
