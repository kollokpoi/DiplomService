using DiplomService.Database;
using DiplomService.Models;
using DiplomService.Models.EventsFolder.Division;
using DiplomService.Models.Users;
using DiplomService.Services;
using DiplomService.ViewModels;
using DiplomService.ViewModels.Divisions;
using DiplomService.ViewModels.Email;
using DiplomService.ViewModels.EventApplication;
using DiplomService.ViewModels.Measures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace DiplomService.Controllers
{

    public class DivisionsController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<User> _userManager;
        public DivisionsController(ApplicationContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int id)
        {
            var @event = await _context.Events.FirstOrDefaultAsync(x => x.Id == id);
            if (@event == null)
                return BadRequest();

            return View(@event);
        }
        public async Task<IActionResult> Details(int id)
        {
            var division = await _context.Divisions.FirstOrDefaultAsync(x => x.Id == id);
            if (division == null)
                return NotFound();
            return View(division);
        }

        [Authorize(Roles = "OrganizationUser")]
        public async Task<IActionResult> Members(int id)
        {
            var division = await _context.Divisions.FirstOrDefaultAsync(x => x.Id == id);
            if (division is null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User) as OrganizationUsers;
            if (user is null)
                return BadRequest();

            if (!division.Event.Organizations.Contains(user.Organization))
                return Unauthorized();

            var model = division.DivisionMembers.ToList();
            ViewBag.divisionId = id;
            return View(model);
        }

        [Authorize(Roles = "OrganizationUser")]
        public async Task<IActionResult> AddDivisionMember(int id, bool leader = false)
        {
            var division = await _context.Divisions.FirstOrDefaultAsync(x => x.Id == id);
            if (division is null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User) as OrganizationUsers;
            if (user is null)
                return BadRequest();

            if (!division.Event.Organizations.Contains(user.Organization))
                return Unauthorized();
            var model = new ApplicationDataViewModel() { DivisionId = division.Id, DivisionLeader = leader };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "OrganizationUser")]
        public async Task<IActionResult> AddDivisionMember(ApplicationDataViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var division = await _context.Divisions.FirstOrDefaultAsync(x => x.Id == model.DivisionId);
            if (division is null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User) as OrganizationUsers;
            if (user is null)
                return BadRequest();

            if (!division.Event.Organizations.Contains(user.Organization))
                return Unauthorized();

            if (model.UserId is not null)
            {
                var divisionUser = await _userManager.FindByIdAsync(model.UserId);
                if (divisionUser is not null)
                {
                    if (!division.DivisionMembers.Any(x => x.User == divisionUser))
                    {
                        _context.DivisionUsers.Add(new()
                        {
                            Division = division,
                            User = divisionUser,
                            DivisionDirector = model.DivisionLeader
                        });
                        await _context.SaveChangesAsync();
                    }

                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                if (model.Name.Split().Length != 3)
                    ModelState.AddModelError("", "Укажите полное фио");
                if (model.PhoneNumber is null)
                    ModelState.AddModelError("", "Укажите номер телефона");
                if (model.Email is null)
                    ModelState.AddModelError("", "Укажите почтовый адрес");
                if (model.WorkingPlace is null)
                    ModelState.AddModelError("", "Укажите учебное заведение");
                if (_context.Users.Any(x => x.PhoneNumber == model.PhoneNumber))
                    ModelState.AddModelError("", "Пользователь с таким номером телефона уже зарегистрирован");

                if (!ModelState.IsValid || model.PhoneNumber is null || model.Email is null || model.Name.Split().Length != 3 || model.WorkingPlace is null)
                    return View(model);

                var nameParts = model.Name.Split();
                Models.User mobileUser;
                if (!model.DivisionLeader)
                {
                    if (model.Course is null)
                        ModelState.AddModelError("", "Укажите класс/курс");
                    if (model.Birthday is null)
                        ModelState.AddModelError("", "Укажите дату рождения");
                    if (!ModelState.IsValid || model.Birthday is null || model.Course is null)
                        return View(model);

                    mobileUser = new MobileUser()
                    {
                        Birthday = model.Birthday.Value,
                        Course = model.Course.Value,
                        Email = model.Email,
                        SecondName = nameParts[0],
                        Name = nameParts[1],
                        LastName = nameParts[2],
                        PhoneNumber = PhoneWorker.NormalizePhone(model.PhoneNumber),
                        UserName = PhoneWorker.NormalizePhone(model.PhoneNumber),
                        NormalizedUserName = PhoneWorker.NormalizePhone(model.PhoneNumber),
                        WorkingPlace = model.WorkingPlace,
                    };
                }
                else
                {

                    mobileUser = new WebUser()
                    {
                        Email = model.Email,
                        SecondName = nameParts[0],
                        Name = nameParts[1],
                        LastName = nameParts[2],
                        PhoneNumber = PhoneWorker.NormalizePhone(model.PhoneNumber),
                        UserName = model.Email,
                        NormalizedUserName = model.Email.ToUpper(),
                        WorkingPlace = model.WorkingPlace,
                    };
                }


                var createResult = await _userManager.CreateAsync(mobileUser);
                if (createResult.Succeeded)
                {
                    _context.DivisionUsers.Add(new()
                    {
                        Division = division,
                        User = mobileUser,
                        DivisionDirector = model.DivisionLeader
                    });
                    _context.SaveChanges();
                    if (model.DivisionLeader)
                    {
                        string password = Guid.NewGuid().ToString().Replace("-", string.Empty)[..5];
                        password += password.ToUpper() + '!' + "aWeA";

                        await _userManager.AddToRoleAsync(mobileUser, "WebUser");
                        await _userManager.AddPasswordAsync(mobileUser, password);


                        var messageVM = new ApplicationEmailViewModel()
                        {
                            OrganizationName = user.Organization.Name,
                            DateOfSend = DateTime.Now,
                            EmailToSend = mobileUser.Email,
                            Password = password,
                            PhoneNumber = mobileUser.PhoneNumber,
                            BaseUrl = $"{Request.Scheme}://{Request.Host}" + Url.Action("Login", "Account")
                        };

                        var renderer = HttpContext.RequestServices.GetRequiredService<IRazorViewToStringRenderer>();
                        var htmlMessage = await renderer.RenderViewToStringAsync("HtmlTemplates/DivisionLeaderRegistration", messageVM, HttpContext.RequestServices);
                        SmtpService.SendApplicationResponse(htmlMessage, messageVM);
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(mobileUser, "MobileUser");
                    }
                }
                else
                {
                    foreach (var item in createResult.Errors.Select(x => x.Description.ToString()))
                    {
                        ModelState.AddModelError("", item);
                    }
                    return View(model);
                }

            }

            return RedirectToAction(nameof(Members), new { division.Id });
        }



        [Authorize(Roles = "OrganizationUser")]
        public async Task<IActionResult> Create(int id)
        {
            var @event = await _context.Events.FirstOrDefaultAsync(x => x.Id == id);
            if (@event == null)
                return NotFound();

            var model = new DivisionViewModel()
            {
                StartDate = @event.DateOfStart,
                EndDate = @event.DateOfEnd,
                EventId = @event.Id,
            };

            foreach (var item in @event.Measures.Where(x => !x.SameForAll))
            {
                var measureModel = new MeasureViewModel()
                {
                    Id = item.Id,
                    SameForAll = false,
                    StartDate = @event.DateOfStart,
                    EndDate = @event.DateOfEnd,
                    EventId = @event.Id,
                    Descrition = item.Descrition,
                    Length = item.Length,
                    DivisionsExists = true,
                    Name = item.Name,
                    OneTime = item.OneTime,
                    Place = item.Place,
                    WeekDays = item.WeekDays,
                };
                measureModel.MeasureDays = new()
                {
                    new(){ DayNumber = 0},
                    new(){ DayNumber = 1},
                    new(){ DayNumber = 2},
                    new(){ DayNumber = 3},
                    new(){ DayNumber = 4},
                    new(){ DayNumber = 5},
                    new(){ DayNumber = 6},
                };
                measureModel.MeasureDates.Add(new()
                {
                    Datetime = @event.DateOfStart,
                    Place = ""
                });
                model.Measures.Add(measureModel);
            }
            model.DivisionLeaders.Add(new());
            return View(model);
        }
        [Authorize(Roles = "OrganizationUser")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id, DivisionViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var @event = await _context.Events.FirstOrDefaultAsync(x => x.Id == id);
            if (@event == null)
                return NotFound();

            if (model.StartDate < @event.DateOfStart)
                ModelState.AddModelError($"StartDate", $"Дата начала не раньше {@event.DateOfStart.ToShortDateString()}");
            if (@event.DateOfEnd is not null)
                if (model.EndDate > @event.DateOfEnd)
                    ModelState.AddModelError($"DateOfEnd", $"Дата начала не позже {@event.DateOfEnd.Value.ToShortDateString()}");

            if (!ModelState.IsValid)
                return View(model);

            var division = new Division()
            {
                Name = model.Name,
                Description = model.Description,
                Longitude = model.Longitude ?? 0,
                Latitude = model.Latitude ?? 0,
                PreviewImage = model.PriviewImage,
                DateOfStart = model.StartDate,
                DateOfEnd = model.EndDate,
                PlaceName = model.PlaceName,
                Event = @event,
                EventId = id,
            };

            @event.Divisions.Add(division);
            for (int i = 0; i < model.Measures.Count; i++)
            {
                var item = model.Measures[i];
                var measure = await _context.Measures.FirstOrDefaultAsync(x => x.Id == item.Id);

                if (measure == null)
                    continue;

                var measureDivisionInfo = new MeasureDivisionsInfo()
                {
                    Measure = measure,
                    Division = division,
                    SameForAll = false,
                    Length = item.Length,
                    Place = item.Place,
                    OneTime = item.OneTime,
                    WeekDays = item.WeekDays,
                    MeasureId = measure.Id
                };

                measure.MeasureDivisionsInfos.Add(measureDivisionInfo);
                division.MeasureDivisionsInfos.Add(measureDivisionInfo);

                if (item.OneTime)
                {
                    if (item.MeasureDates[0].Datetime < @event.DateOfStart)
                        ModelState.AddModelError($"Measures[{i}].MeasureDates[0].Datetime",$"Дата начала не раньше {@event.DateOfStart.ToShortDateString()}");
                    if(@event.DateOfEnd is not null)
                        if(item.MeasureDates[0].Datetime > @event.DateOfEnd)
                            ModelState.AddModelError($"Measures[{i}].MeasureDates[0].Datetime", $"Дата начала не позже {@event.DateOfEnd.Value.ToShortDateString()}");

                    if (!ModelState.IsValid)
                        return View(model);

                    measureDivisionInfo.MeasureDates.Add(new()
                    {
                        Datetime = item.MeasureDates[0].Datetime,
                        Place = item.Place
                    });
                }
                else
                {
                    if (item.WeekDays)
                    {
                        foreach (var dayItem in item.MeasureDays)
                        {
                            if (!dayItem.Checked)
                            {
                                continue;
                            }
                            var day = new MeasureDays()
                            {
                                DayNumber = dayItem.DayNumber,
                                TimeSpan = dayItem.TimeSpan,
                                Place = dayItem.Place,
                            };
                            measureDivisionInfo.MeasureDays.Add(day);
                        }
                    }
                    else
                    {
                        for (int j = 0; j < item.MeasureDates.Count; j++)
                        {
                            var dateItem = item.MeasureDates[j];

                            if (item.MeasureDates[j].Datetime < @event.DateOfStart)
                                ModelState.AddModelError($"Measures[{i}].MeasureDates[{j}].Datetime", $"Дата начала не раньше {@event.DateOfStart.ToShortDateString()}");
                            if (@event.DateOfEnd is not null)
                                if (item.MeasureDates[j].Datetime > @event.DateOfEnd)
                                    ModelState.AddModelError($"Measures[{i}].MeasureDates[{j}].Datetime", $"Дата начала не позже {@event.DateOfEnd.Value.ToShortDateString()}");

                            if (!ModelState.IsValid)
                                return View(model);

                            var date = new MeasureDates()
                            {
                                Datetime = dateItem.Datetime,
                                Place = dateItem.Place,
                            };
                            measureDivisionInfo.MeasureDates.Add(date);
                        }
                    }
                }
            }
            foreach (var item in model.DivisionLeaders)
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

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { Id = id });
        }


        [Authorize(Roles = "OrganizationUser")]
        public async Task<IActionResult> Edit(int id, int eventId)
        {
            var division = await _context.Divisions.FirstOrDefaultAsync(x => x.Id == id);

            if (division is null)
                return NotFound();

            var model = new DivisionViewModel()
            {
                Id = division.Id,
                Description = division.Description,
                Longitude = division.Longitude,
                Latitude = division.Latitude,
                PlaceName = division.PlaceName,
                PriviewImage = division.PreviewImage,
                EventId = eventId,
                StartDate = division.DateOfStart,
                EndDate = division.DateOfEnd,
                Name = division.Name,
            };

            foreach (var item in division.MeasureDivisionsInfos)
            {
                var measureModel = new MeasureViewModel()
                {
                    Id = item.Id,
                    Place = item.Place,
                    Name = item.Measure.Name,
                    Length = item.Length,
                    StartDate = division.Event.DateOfStart,
                    EndDate = division.Event.DateOfEnd,
                    WeekDays = item.WeekDays,
                    OneTime = item.OneTime,
                    SameForAll = item.SameForAll,
                };

                List<int> dayNumbers = new List<int>();

                foreach (var dayItem in item.MeasureDays)
                {
                    dayNumbers.Add(dayItem.DayNumber);
                    var measureDay = new MeasureDaysViewModel()
                    {
                        Id = dayItem.Id,
                        TimeSpan = dayItem.TimeSpan,
                        DayNumber = dayItem.DayNumber,
                        Place = dayItem.Place,
                        Checked = true
                    };
                    measureModel.MeasureDays.Add(measureDay);
                }

                for (int i = 0; i < 7; i++)
                {
                    if (!dayNumbers.Contains(i))
                    {
                        var measureDay = new MeasureDaysViewModel()
                        {
                            Id = 0,
                            TimeSpan = TimeSpan.Zero,
                            Place = "",
                            DayNumber = i,
                        };
                        measureModel.MeasureDays.Add(measureDay);
                    }
                }

                measureModel.MeasureDays = measureModel.MeasureDays.OrderBy(x => x.DayNumber).ToList();
                foreach (var dateItem in item.MeasureDates)
                {
                    var measureDate = new MeasureDatesViewModel()
                    {
                        Id = dateItem.Id,
                        Datetime = dateItem.Datetime,
                        Place = dateItem.Place,
                    };
                    measureModel.MeasureDates.Add(measureDate);
                }

                if (measureModel.MeasureDates.Count == 0)
                {
                    var measureDate = new MeasureDatesViewModel()
                    {
                        Datetime = DateTime.Now.Date,
                        Place = ""
                    };
                    measureModel.MeasureDates.Add(measureDate);
                }

                model.Measures.Add(measureModel);
            }

            var directors = division.DivisionMembers.Where(x => x.DivisionDirector);
            foreach (var item in directors)
            {
                model.DivisionLeaders.Add(new()
                {
                    Id = item.UserId,
                    UserName = item.User.GetFullName()
                });
            }

            return View(model);
        }
        [Authorize(Roles = "OrganizationUser")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DivisionViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var division = await _context.Divisions.FirstOrDefaultAsync(x => x.Id == id);
            if (division is null)
                return NotFound();

            division.Name = model.Name;
            division.Description = model.Description;
            division.Longitude = model.Longitude is null ? 0 : model.Longitude.Value;
            division.Latitude = model.Latitude is null ? 0 : model.Latitude.Value;
            division.DateOfStart = model.StartDate;
            division.DateOfEnd = model.EndDate;
            division.PlaceName = model.PlaceName;

            if (model.PriviewImage is not null)
                division.PreviewImage = model.PriviewImage;

            if (model.StartDate < division.Event.DateOfStart)
                ModelState.AddModelError($"StartDate", $"Дата начала не раньше {division.Event.DateOfStart.ToShortDateString()}");
            if (division.Event.DateOfEnd is not null)
                if (model.EndDate > division.Event.DateOfEnd)
                    ModelState.AddModelError($"DateOfEnd", $"Дата начала не позже {division.Event.DateOfEnd.Value.ToShortDateString()}");

            if (!ModelState.IsValid)
                return View(model);

            for (int i = 0; i < model.Measures.Count; i++)
            {
                var item = model.Measures[i];
                var measureDivisionInfo = await _context.MeasureDivisionsInfos.FirstOrDefaultAsync(x => x.Id == item.Id);
                if (measureDivisionInfo is null)
                    continue;

                measureDivisionInfo.OneTime = item.OneTime;
                measureDivisionInfo.WeekDays = item.WeekDays;
                measureDivisionInfo.Length = item.Length;
                measureDivisionInfo.SameForAll = false;

                if (item.OneTime)
                {
                    if (item.MeasureDates[0].Datetime < division.Event.DateOfStart)
                        ModelState.AddModelError($"Measures[{i}].MeasureDates[0].Datetime", $"Дата начала не раньше {division.Event.DateOfStart.ToShortDateString()}");
                    if (division.Event.DateOfEnd is not null)
                        if (item.MeasureDates[0].Datetime > division.Event.DateOfEnd)
                            ModelState.AddModelError($"Measures[{i}].MeasureDates[0].Datetime", $"Дата начала не позже {division.Event.DateOfEnd.Value.ToShortDateString()}");

                    if (!ModelState.IsValid)
                        return View(model);

                    var date = new MeasureDates();
                    if (measureDivisionInfo.MeasureDates.Count > 0)
                    {
                        date = measureDivisionInfo.MeasureDates[0];
                        for (int k = 1; k < measureDivisionInfo.MeasureDates.Count; k++)
                            _context.Remove(measureDivisionInfo.MeasureDates[i]);
                    }
                    measureDivisionInfo.MeasureDays.Clear();

                    date.MeasureDivisionsInfos = measureDivisionInfo;
                    date.MeasureDivisionsInfosId = measureDivisionInfo.Id;
                    date.Datetime = item.MeasureDates[0].Datetime;
                    date.Place = item.Place;

                    if (date.Id == 0)
                        measureDivisionInfo.MeasureDates.Add(date);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    if (item.WeekDays)
                    {
                        measureDivisionInfo.MeasureDates.Clear();
                        _context.RemoveRange(measureDivisionInfo.MeasureDates);

                        foreach (var dayItem in item.MeasureDays)
                        {
                            bool contains = dayItem.Id != 0;
                            if (!dayItem.Checked)
                            {
                                if (contains)
                                {
                                    var measureDay = measureDivisionInfo.MeasureDays.First(x => x.Id == dayItem.Id);
                                    measureDivisionInfo.MeasureDays.Remove(measureDay);
                                }
                                continue;
                            }

                            if (contains)
                            {
                                var measureDay = measureDivisionInfo.MeasureDays.FirstOrDefault(x => x.Id == dayItem.Id);
                                if (measureDay is null)
                                    continue;
                                measureDay.TimeSpan = dayItem.TimeSpan;
                                measureDay.Place = dayItem.Place;
                            }
                            else
                            {
                                var day = new MeasureDays()
                                {
                                    DayNumber = dayItem.DayNumber,
                                    TimeSpan = dayItem.TimeSpan,
                                    Place = dayItem.Place,
                                    MeasureDivisionsInfo = measureDivisionInfo
                                };
                                measureDivisionInfo.MeasureDays.Add(day);
                            }
                        }
                    }
                    else
                    {
                        measureDivisionInfo.MeasureDays.Clear();
                        _context.RemoveRange(measureDivisionInfo.MeasureDays);

                        var ids = new List<int>();

                        foreach (var dateItem in measureDivisionInfo.MeasureDates)
                        {
                            ids.Add(dateItem.Id);
                        }
                        for (int j = 0; j < item.MeasureDates.Count; j++)
                        {
                            var dateItem = item.MeasureDates[j];
                            bool contains = dateItem.Id != 0;
                            if (contains)
                            {
                                ids.Remove(dateItem.Id);
                                var measureDay = measureDivisionInfo.MeasureDates.FirstOrDefault(x => x.Id == dateItem.Id);
                                if (measureDay is null)
                                    continue;
                                measureDay.Datetime = dateItem.Datetime;
                                measureDay.Place = dateItem.Place;
                                if (dateItem.Datetime < division.Event.DateOfStart)
                                    ModelState.AddModelError($"Measures[{i}].MeasureDates[{j}].Datetime", $"Дата начала не раньше {division.Event.DateOfStart.ToShortDateString()}");
                                if (division.Event.DateOfEnd is not null)
                                    if (item.MeasureDates[0].Datetime > division.Event.DateOfEnd)
                                        ModelState.AddModelError($"Measures[{i}].MeasureDates[{j}].Datetime", $"Дата начала не позже {division.Event.DateOfEnd.Value.ToShortDateString()}");
                            }
                            else
                            {
                                var date = new MeasureDates()
                                {
                                    Datetime = dateItem.Datetime,
                                    Place = dateItem.Place,
                                    MeasureDivisionsInfos = measureDivisionInfo
                                };

                                if (dateItem.Datetime < division.Event.DateOfStart)
                                    ModelState.AddModelError($"Measures[{i}].MeasureDates[{j}].Datetime", $"Дата начала не раньше {division.Event.DateOfStart.ToShortDateString()}");
                                if (division.Event.DateOfEnd is not null)
                                    if (item.MeasureDates[0].Datetime > division.Event.DateOfEnd)
                                        ModelState.AddModelError($"Measures[{i}].MeasureDates[{j}].Datetime", $"Дата начала не позже {division.Event.DateOfEnd.Value.ToShortDateString()}");

                                measureDivisionInfo.MeasureDates.Add(date);
                            }
                        }
                        if (!ModelState.IsValid)
                            return View(model);

                        foreach (var dateItem in measureDivisionInfo.MeasureDates)
                        {
                            if (ids.Contains(dateItem.Id))
                            {
                                _context.Remove(dateItem);
                            }
                        }
                    }
                }
            }
            var divisionLeadersIds = division.DivisionMembers
                .Where(x => x.DivisionDirector)
                .Select(x => x.UserId);

            var divisionLeadersToDelete = divisionLeadersIds.Except(model.DivisionLeaders.Select(x => x.Id)).ToList();
            var divisionLeadersToAdd = model.DivisionLeaders.Select(x => x.Id).Except(divisionLeadersIds).ToList();

            foreach (var userId in divisionLeadersToDelete)
            {
                var divisionMemder = division.DivisionMembers.FirstOrDefault(x => x.UserId == userId);
                if (divisionMemder is not null)
                    division.DivisionMembers.Remove(divisionMemder);
                _context.SaveChanges();
            }

            foreach (var userId in divisionLeadersToAdd)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user is null)
                    continue;

                await _userManager.AddToRoleAsync(user, "MobileUser");

                if (!division.DivisionMembers.Any(x => x.User == user))
                {
                    division.DivisionMembers.Add(new()
                    {
                        Division = division,
                        DivisionDirector = true,
                        User = user
                    });
                }

            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { Id = division.EventId });
        }

        [HttpPost]
        [Authorize(Roles = "OrganizationUser")]
        public async Task<IActionResult> Delete(int id)
        {
            var division = await _context.Divisions.FirstOrDefaultAsync(x => x.Id == id);
            if (division is null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User) as OrganizationUsers;
            if (user is null)
                return BadRequest();

            if (!user.Organization.Events.Contains(division.Event))
                return Unauthorized();

            foreach (var item in division.MeasureDivisionsInfos)
                _context.Remove(item);

            await _context.SaveChangesAsync();
            _context.Remove(division);
            await _context.SaveChangesAsync();


            return RedirectToAction("Index", new { Id = division.EventId });
        }

        [HttpPost]
        [Authorize(Roles = "OrganizationUser")]
        public async Task<IActionResult> DeleteUserFromDivision(int id)
        {
            var division = await _context.DivisionUsers.FirstOrDefaultAsync(x => x.Id == id);
            if (division is null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User) as OrganizationUsers;
            if (user is null)
                return BadRequest();

            if (!user.Organization.Events.Contains(division.Division.Event))
                return Unauthorized();

            if (division.Division.DivisionMembers.Count > 1)
            {
                division.Division.DivisionMembers.Remove(division);
                await _context.SaveChangesAsync();
            }
            else
            {
                TempData["ErrorMessage"] = "Должен оставаться минимум один ответственный.";
            }

            return RedirectToAction(nameof(Members), new { Id = division.DivisionId });
        }

        private bool DivisionExists(int id)
        {
            return (_context.Divisions?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
