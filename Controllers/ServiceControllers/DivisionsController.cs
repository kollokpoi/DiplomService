using DiplomService.Database;
using DiplomService.Models;
using DiplomService.Models.EventsFolder.Division;
using DiplomService.ViewModels;
using DiplomService.ViewModels.Divisions;
using DiplomService.ViewModels.Measures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiplomService.Controllers
{
    public class DivisionsController : Controller
    {
        private readonly ApplicationContext _context;

        public DivisionsController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: Divisions
        public async Task<IActionResult> Index(int id)
        {
            var @event = await _context.Events.FirstOrDefaultAsync(x => x.Id == id);
            if (@event == null)
                return BadRequest();

            return View(@event);
        }

        public async Task<IActionResult> Details(int? id)
        {
            var division = await _context.Divisions.FirstOrDefaultAsync(x => x.Id == id);
            if (division == null)
                return NotFound();
            return View(division);
        }

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

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id, DivisionViewModel model)
        {
            if (!ModelState.IsValid)
               return View(model);

            var @event = await _context.Events.FirstOrDefaultAsync(x => x.Id == id);
            if (@event == null)
                return NotFound();

            var division = new Division()
            {
                Name = model.Name,
                Description = model.Description,
                Longitude= model.Longitude?? 0,
                Latitude= model.Latitude ?? 0,
                PreviewImage = model.PriviewImage,
                DateOfStart = model.StartDate, 
                DateOfEnd = model.EndDate,
                Event = @event,
                EventId = id,
            };

            @event.Divisions.Add(division);
            foreach (var item in model.Measures)
            {
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
                        foreach (var dateItem in item.MeasureDates)
                        {
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

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { Id = id });
        }

        public async Task<IActionResult> Edit(int id, int eventId)
        {
            var division = await _context.Divisions.FirstOrDefaultAsync(x => x.Id == id);

            if (division == null)
                return NotFound();

            var model = new DivisionViewModel()
            {
                Id = division.Id,
                Description = division.Description,
                Longitude = division.Longitude,
                Latitude = division.Latitude,
                PlaceName= division.PlaceName,
                PriviewImage = division.PreviewImage,
                EventId = eventId,
                StartDate = division.DateOfStart,
                EndDate = division.DateOfEnd,
                Name = division.Name
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

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int eventId, DivisionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var division = await _context.Divisions.FirstOrDefaultAsync(x => x.Id == id);
                if (division == null)
                    return NotFound();

                division.Name = model.Name;
                division.Description = model.Description;
                division.Longitude = model.Longitude is null? 0: model.Longitude.Value;
                division.Latitude = model.Latitude is null ? 0 : model.Latitude.Value;
                division.DateOfStart = model.StartDate;
                division.DateOfEnd = model.EndDate;
                division.PlaceName = model.PlaceName;
                if (model.PriviewImage != null)
                {
                    division.PreviewImage = model.PriviewImage;
                }

                foreach (var item in model.Measures)
                {
                    var measureDivisionInfo = await _context.MeasureDivisionsInfos.FirstOrDefaultAsync(x => x.Id == item.Id);
                    if (measureDivisionInfo is null)
                        continue;

                    measureDivisionInfo.OneTime = item.OneTime;
                    measureDivisionInfo.WeekDays = item.WeekDays;
                    measureDivisionInfo.Length = item.Length;
                    measureDivisionInfo.SameForAll = false;

                    if (item.OneTime)
                    {
                        var date = new MeasureDates();
                        if (measureDivisionInfo.MeasureDates.Count > 0)
                        {
                            date = measureDivisionInfo.MeasureDates[0];
                            for (int i = 1; i < measureDivisionInfo.MeasureDates.Count; i++)
                                _context.Remove(measureDivisionInfo.MeasureDates[i]);
                        }

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

                            foreach (var dateItem in item.MeasureDates)
                            {
                                bool contains = dateItem.Id != 0;
                                if (contains)
                                {
                                    ids.Remove(dateItem.Id);
                                    var measureDay = measureDivisionInfo.MeasureDates.FirstOrDefault(x => x.Id == dateItem.Id);
                                    if (measureDay is null)
                                        continue;
                                    measureDay.Datetime = dateItem.Datetime;
                                    measureDay.Place = dateItem.Place;
                                }
                                else
                                {
                                    var date = new MeasureDates()
                                    {
                                        Datetime = dateItem.Datetime,
                                        Place = dateItem.Place,
                                        MeasureDivisionsInfos = measureDivisionInfo
                                    };

                                    measureDivisionInfo.MeasureDates.Add(date);
                                }
                            }

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
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { Id = eventId });
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int eventId, int id)
        {
            var division = await _context.Divisions.FirstOrDefaultAsync(x => x.Id == id);
            if (division is not null)
            {
                foreach (var item in division.MeasureDivisionsInfos)
                {
                    _context.Remove(item);
                }
                await _context.SaveChangesAsync();
                _context.Remove(division);
                await _context.SaveChangesAsync();
            }


            return RedirectToAction("Index", new { Id = eventId });
        }


        private bool DivisionExists(int id)
        {
            return (_context.Divisions?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
