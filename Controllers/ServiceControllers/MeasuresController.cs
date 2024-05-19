using DiplomService.Database;
using DiplomService.Models;
using DiplomService.Models.EventsFolder.Division;
using DiplomService.ViewModels;
using DiplomService.ViewModels.Measures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiplomService.Controllers
{
    public class MeasuresController : Controller
    {
        private readonly ApplicationContext _context;

        public MeasuresController(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int Id)
        {
            var @event = await _context.Events.FirstOrDefaultAsync(x => x.Id == Id);
            if (@event == null)
                return BadRequest();
            ViewBag.eventId = @event.Id;
            return View(@event);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Measures == null)
            {
                return NotFound();
            }

            var measure = await _context.Measures
                .Include(m => m.Event)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (measure == null)
            {
                return NotFound();
            }

            return View(measure);
        }

        public async Task<IActionResult> Create(int id)
        {
            var @event = await _context.Events.FirstOrDefaultAsync(@event => @event.Id == id);
            if (@event == null)
                return NotFound();

            var model = new MeasureViewModel();
            model.EventId = id;
            model.EndDate = @event.DateOfEnd;
            model.StartDate = @event.DateOfStart;
            model.DivisionsExists = @event.DivisionsExist;
            model.MeasureDates.Add(new()
            {
                Datetime = @event.DateOfStart,
                Place = ""
            });
            for (int i = 0; i <= 6; i++)
            {
                model.MeasureDays.Add(new()
                {
                    Checked = false,
                    DayNumber = i
                });
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MeasureViewModel measure)
        {
            if (ModelState.IsValid)
            {
                var @event = await _context.Events.FirstOrDefaultAsync(x => x.Id == measure.EventId);
                if (@event == null)
                    return NotFound();
                var newMeasure = new Measure();

                newMeasure.EventId = measure.EventId;
                newMeasure.Event = @event;
                newMeasure.Name = measure.Name;
                newMeasure.Descrition = measure.Descrition;
                newMeasure.Length = measure.Length;
                newMeasure.Place = measure.Place;
                newMeasure.SameForAll = measure.SameForAll;
                newMeasure.WeekDays = measure.WeekDays;
                newMeasure.OneTime = measure.OneTime;
                newMeasure.Icon = measure.Icon;

                bool checkDates = true;
                if (measure.DivisionsExists)
                {
                    if (measure.SameForAll)
                    {
                        var measureDivisionInfo = new MeasureDivisionsInfo()
                        {
                            SameForAll = measure.SameForAll,
                            Measure = newMeasure,
                            OneTime = measure.OneTime,
                            WeekDays = measure.WeekDays,
                        };
                        foreach (var item in measure.MeasureDates)
                        {
                            var date = new MeasureDates()
                            {
                                Datetime = item.Datetime,
                                Place = item.Place,
                            };
                            measureDivisionInfo.MeasureDates.Add(date);
                        }
                        newMeasure.MeasureDivisionsInfos.Add(measureDivisionInfo);
                    }
                    else
                    {
                        checkDates = false;
                    }

                }
                else
                {
                    var measureDivisionInfo = new MeasureDivisionsInfo()
                    {
                        SameForAll = measure.SameForAll,
                        Measure = newMeasure,
                        OneTime = measure.OneTime,
                        WeekDays = measure.WeekDays,
                    };

                    if (measure.OneTime)
                    {
                        measureDivisionInfo.MeasureDates.Add(new()
                        {
                            Datetime = measure.MeasureDates[0].Datetime,
                            Place = measure.Place
                        });
                    }
                    else
                    {
                        if (measure.WeekDays)
                        {
                            foreach (var item in measure.MeasureDays)
                            {
                                if (!item.Checked)
                                {
                                    continue;
                                }
                                var day = new MeasureDays()
                                {
                                    DayNumber = item.DayNumber,
                                    TimeSpan = item.TimeSpan,
                                    Place = item.Place,
                                };
                                measureDivisionInfo.MeasureDays.Add(day);
                            }
                        }
                        else
                        {
                            foreach (var item in measure.MeasureDates)
                            {
                                var date = new MeasureDates()
                                {
                                    Datetime = item.Datetime,
                                    Place = item.Place,
                                };
                                measureDivisionInfo.MeasureDates.Add(date);
                            }
                        }
                    }
                    measureDivisionInfo.Division = @event.Divisions[0];
                    newMeasure.MeasureDivisionsInfos.Add(measureDivisionInfo);
                }

                if (measure.MeasureDays.Count == 0 && measure.MeasureDates.Count == 0 && checkDates)
                {
                    ModelState.AddModelError(string.Empty, "Добавьте хотя бы одну дату");
                    return View(measure);
                }
                @event.Measures.Add(newMeasure);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { Id = measure.EventId });
            }
            return View(measure);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Measures == null)
            {
                return NotFound();
            }
            var Measure = await _context.Measures.FirstOrDefaultAsync(measure => measure.Id == id);
            if (Measure == null)
                return BadRequest();
            var model = new MeasureViewModel()
            {
                Descrition = Measure.Descrition,
                Name = Measure.Name,
                Icon = Measure.Icon,
                DivisionsExists = Measure.Event?.DivisionsExist ?? true,
                SameForAll = Measure.SameForAll,
                EventId = Measure.EventId,
                StartDate = Measure.Event.DateOfStart,
                EndDate = Measure.Event.DateOfEnd,
                Id = Measure.Id,
                Length = Measure.Length,
                OneTime = Measure.OneTime,
                WeekDays = Measure.WeekDays,
                Place = Measure.Place,
            };
            List<int> dayNumbers = new List<int>();


            if (Measure.MeasureDivisionsInfos.Count > 0)
            {
                var measureDivisionInfo = Measure.MeasureDivisionsInfos[0];

                foreach (var item in measureDivisionInfo.MeasureDays)
                {

                    var modelDay = new MeasureDaysViewModel();
                    modelDay.Id = item.Id;
                    modelDay.TimeSpan = item.TimeSpan;
                    modelDay.Place = item.Place;
                    modelDay.Checked = true;
                    modelDay.DayNumber = item.DayNumber;
                    model.MeasureDays.Add(modelDay);
                    dayNumbers.Add(item.DayNumber);
                }

                model.MeasureDays = model.MeasureDays.OrderBy(x => x.DayNumber).ToList();

                foreach (var item in measureDivisionInfo.MeasureDates)
                {
                    var measureDay = new MeasureDatesViewModel()
                    {
                        Id = item.Id,
                        Datetime = item.Datetime,
                        Place = item.Place,
                    };
                    model.MeasureDates.Add(measureDay);
                }
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
                    model.MeasureDays.Add(measureDay);
                }
            }
            model.MeasureDays = model.MeasureDays.OrderBy(x => x.DayNumber).ToList();
            if (model.MeasureDates.Count == 0)
            {
                var measureDate = new MeasureDatesViewModel()
                {
                    Datetime = DateTime.Now.Date,
                    Place = ""
                };
                model.MeasureDates.Add(measureDate);
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MeasureViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var measure = await _context.Measures.FirstOrDefaultAsync(m => m.Id == id);
                if (measure == null)
                    return NotFound();

                measure.Name = model.Name;
                measure.Descrition = model.Descrition;
                measure.Length = model.Length;
                measure.Place = model.Place;
                measure.SameForAll = model.SameForAll;
                measure.WeekDays = model.WeekDays;
                measure.OneTime = model.OneTime;
                measure.Icon = model.Icon;

                if (model.DivisionsExists)
                {
                    if (model.SameForAll)
                    {
                        var measureDivision = new MeasureDivisionsInfo();
                        if (measure.MeasureDivisionsInfos.Count > 0)
                        {
                            measureDivision = measure.MeasureDivisionsInfos[0];
                        }

                        List<int> ids = new List<int>();
                        foreach (var item in measureDivision.MeasureDates)
                        {
                            ids.Add(item.Id);
                        }

                        foreach (var item in model.MeasureDates)
                        {
                            if (item.Id != 0)
                            {
                                ids.Remove(item.Id);
                                var date = measureDivision.MeasureDates.FirstOrDefault(x => x.Id == item.Id);
                                if (date == null)
                                    continue;
                                date.Datetime = item.Datetime;
                                date.Place = item.Place;
                            }
                            else
                            {
                                var date = new MeasureDates()
                                {
                                    Datetime = item.Datetime,
                                    Place = item.Place,
                                    MeasureDivisionsInfos = measureDivision
                                };
                                measureDivision.MeasureDates.Add(date);
                            }
                        }

                        foreach (var item in ids)
                        {
                            var measureDate = measureDivision.MeasureDates.FirstOrDefault(x => x.Id == item);
                            if (measureDate != null)
                                measureDivision.MeasureDates.Remove(measureDate);
                        }
                        if (measureDivision.Id == 0)
                        {
                            measureDivision.SameForAll = true;
                            measure.MeasureDivisionsInfos.Add(measureDivision);
                        }
                    }
                }
                else
                {
                    var measureDivision = new MeasureDivisionsInfo();
                    if (measure.MeasureDivisionsInfos.Count > 0)
                    {
                        measureDivision = measure.MeasureDivisionsInfos[0];
                    }

                    if (model.OneTime)
                    {
                        var date = measureDivision.MeasureDates.FirstOrDefault();
                        if (date == null)
                        {
                            date = new()
                            {
                                Datetime = model.MeasureDates[0].Datetime,
                                Place = model.Place
                            };
                            measureDivision.MeasureDates.Add(date);
                        }
                        else
                        {
                            date.Datetime = model.MeasureDates[0].Datetime;
                            date.Place = model.Place;
                        }

                        var dates = measureDivision.MeasureDates.Where(x => x.Id != date.Id);
                        measureDivision.MeasureDays.Clear();
                        _context.RemoveRange(dates);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        if (model.WeekDays)
                        {
                            measureDivision.MeasureDates.Clear();
                            foreach (var item in model.MeasureDays)
                            {
                                bool contains = item.Id != 0;
                                if (!item.Checked)
                                {
                                    if (contains)
                                    {
                                        var measureDay = measureDivision.MeasureDays.First(x => x.Id == item.Id);
                                        measureDivision.MeasureDays.Remove(measureDay);
                                    }
                                    continue;
                                }

                                if (contains)
                                {
                                    var measureDay = measureDivision.MeasureDays.First(x => x.Id == item.Id);
                                    if (measureDay is null)
                                        continue;
                                    measureDay.TimeSpan = item.TimeSpan;
                                    measureDay.Place = item.Place;
                                }
                                else
                                {
                                    var day = new MeasureDays()
                                    {
                                        DayNumber = item.DayNumber,
                                        TimeSpan = item.TimeSpan,
                                        Place = item.Place,
                                        MeasureDivisionsInfo = measureDivision
                                    };
                                    measureDivision.MeasureDays.Add(day);

                                }
                            }
                        }
                        else
                        {
                            var ids = new List<int>();

                            foreach (var dateItem in measureDivision.MeasureDates)
                            {
                                ids.Add(dateItem.Id);
                            }

                            measureDivision.MeasureDays.Clear();
                            foreach (var item in model.MeasureDates)
                            {
                                bool contains = item.Id != 0;
                                if (contains)
                                {
                                    ids.Remove(item.Id);
                                    var measureDay = measureDivision.MeasureDates.First(x => x.Id == item.Id);
                                    if (measureDay is null)
                                        continue;
                                    measureDay.Datetime = item.Datetime;
                                    measureDay.Place = item.Place;
                                }
                                else
                                {
                                    var date = new MeasureDates()
                                    {
                                        Datetime = item.Datetime,
                                        Place = item.Place,
                                        MeasureDivisionsInfos = measureDivision
                                    };

                                    measureDivision.MeasureDates.Add(date);
                                }

                            }

                            foreach (var dateItem in measureDivision.MeasureDates)
                            {
                                if (ids.Contains(dateItem.Id))
                                {
                                    _context.Remove(dateItem);
                                }
                            }
                        }
                    }
                    if (measureDivision.Id == 0)
                    {
                        measureDivision.SameForAll = true;
                        measure.MeasureDivisionsInfos.Add(measureDivision);
                    }
                }

                bool checkDates = true;

                if (!measure.SameForAll && model.DivisionsExists)
                {
                    checkDates = false;
                }

                if (measure.MeasureDivisionsInfos[0].MeasureDays.Count == 0 && measure.MeasureDivisionsInfos[0].MeasureDates.Count == 0 && checkDates)
                {
                    ModelState.AddModelError(string.Empty, "Добавьте хотя бы одну дату");
                    return View(model);
                }
                _context.SaveChanges();
                return RedirectToAction(nameof(Index), new { Id = model.EventId });
            }
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Measures == null)
            {
                return NotFound();
            }

            var measure = await _context.Measures.FirstOrDefaultAsync(m => m.Id == id);
            if (measure == null)
            {
                return NotFound();
            }

            _context.RemoveRange(measure.MeasureDivisionsInfos);
            await _context.SaveChangesAsync();
            _context.Measures.Remove(measure);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { Id = measure.EventId });
        }

        private bool MeasureExists(int id)
        {
            return (_context.Measures?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
