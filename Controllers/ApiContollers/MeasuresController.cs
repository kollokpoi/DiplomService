using DiplomService.Database;
using DiplomService.Models;
using DiplomService.Models.EventsFolder.Division;
using DiplomService.ViewModels.Measures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiplomService.Controllers.ApiContollers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "MobileUser")]
    public class MeasuresController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<User> _userManager;

        public MeasuresController(ApplicationContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Measures
        [HttpGet("GetMeasures")]
        public async Task<ActionResult> GetMeasures()
        {
            if (_context.Measures == null)
            {
                return NotFound();
            }
            return Ok(await _context.Measures.ToListAsync());
        }

        [HttpGet("GetMeasuresForUser")]
        public async Task<ActionResult> GetMeasuresForUser()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                var divisions = await _context.Users
                    .Where(u => u == user)
                    .SelectMany(u => u.UserDivisions)
                    .Select(du => du.Division).ToListAsync();

                var measures = divisions.SelectMany(x => x.Event.Measures).Where(x => x.SameForAll).SelectMany(x => x.MeasureDivisionsInfos).Distinct().ToList();
                measures.AddRange(divisions.SelectMany(x => x.MeasureDivisionsInfos).ToList());


                List<MeasureDates> measureDates = new List<MeasureDates>();
                List<EventMeasuresViewModel> viewModel = new List<EventMeasuresViewModel>();
                foreach (var item in measures)
                {
                    if (!item.WeekDays)
                    {
                        measureDates.AddRange(item.MeasureDates);
                    }
                    else
                    {
                        measureDates.AddRange(GetNearestDayOfWeek(item.MeasureDays));
                    }

                }
                measureDates = measureDates.OrderBy(x => x.Datetime).ToList();
                foreach (var item in measureDates)
                {
                    viewModel.Add(new EventMeasuresViewModel()
                    {
                        Id = item.MeasureDivisionsInfosId,
                        SameForAll = item.MeasureDivisionsInfos.SameForAll,
                        EventName = item.MeasureDivisionsInfos.Measure.Name,
                        DateTime = item.Datetime,
                        Icon = item.MeasureDivisionsInfos.Measure.Icon
                    });
                }

                return Ok(viewModel);
            }
        }

        [HttpPost("GetMeasuresDivision")]
        public async Task<ActionResult> GetMeasuresDivision([FromBody] List<int> divisions)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return NotFound();
            }
            else
            {
                List<EventMeasuresViewModel> viewModel = new List<EventMeasuresViewModel>();
                List<MeasureDivisionsInfo> measuresForDivision = new List<MeasureDivisionsInfo>();
                for (int i = 0; i < divisions.Count; i++)
                {
                    var division = await _context.Divisions.FirstOrDefaultAsync(x => x.Id == divisions[i]);
                    if (division == null)
                        return NotFound();
                    if (i == 0)
                    {
                        measuresForDivision.AddRange(division.Event.Measures.Where(x => x.SameForAll)
                              .SelectMany(x => x.MeasureDivisionsInfos).ToList());
                    }
                    measuresForDivision.AddRange(division.MeasureDivisionsInfos);
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
                        viewModelItem.DateTime = GetNearestDate(measure.MeasureDays);
                    }
                    else
                    {
                        viewModelItem.DateTime = GetNearestDate(measure.MeasureDates);
                    }
                    viewModelItem.Icon = measure.Measure.Icon;
                    viewModel.Add(viewModelItem);
                }

                viewModel = viewModel.OrderBy(x => x.DateTime).ToList();
                return Ok(viewModel);
            }
        }

        // GET: api/Measures/5
        [HttpGet("GetMeasure/{id}")]
        public async Task<ActionResult<MeasureDivisionsInfo>> GetMeasure(int id)
        {
            if (_context.Measures == null)
            {
                return NotFound();
            }
            var measure = await _context.MeasureDivisionsInfos.FindAsync(id);

            if (measure == null)
            {
                return NotFound();
            }
            measure.Length = measure.Measure.Length;
            measure.Place = measure.Measure.Place;
            if (measure.WeekDays)
                measure.MeasureDays = measure.MeasureDays.OrderBy(x => x.DayNumber).ToList();
            else
                measure.MeasureDates = measure.MeasureDates.OrderBy(x => x.Datetime).ToList();

            return Ok(measure);
        }

        public static DateTime GetNearestDate(List<MeasureDates> measureDates)
        {
            DateTime currentDate = DateTime.Now;
            var validDates = measureDates
                .Where(date => date.Datetime > currentDate)
                .OrderBy(date => Math.Abs((date.Datetime - currentDate).TotalDays))
                .FirstOrDefault();

            return validDates != null ? validDates.Datetime : DateTime.MinValue;
        }
        public static DateTime GetNearestDate(List<MeasureDays> measureDays)
        {
            DateTime currentDate = DateTime.Now;
            int currentDayOfWeek = (int)currentDate.DayOfWeek;

            var sortedDatesList = measureDays
                .Select(day =>
                {
                    var nextDay = currentDate.Date.AddDays((day.DayNumber - currentDayOfWeek + 7) % 7);
                    var dateTime = nextDay.Add(day.TimeSpan);
                    return new MeasureDates
                    {
                        Datetime = dateTime,
                        Place = day.Place,
                        MeasureDivisionsInfos = day.MeasureDivisionsInfo
                    };
                })
                .Where(date => date.Datetime > currentDate)
                .OrderBy(date => date.Datetime)
                .ToList();
            return sortedDatesList.FirstOrDefault()?.Datetime ?? DateTime.MinValue;
        }
        public static List<MeasureDates> GetNearestDayOfWeek(List<MeasureDays> measureDays)
        {
            DateTime currentDate = DateTime.Now;
            int currentDayOfWeek = (int)currentDate.DayOfWeek;

            var sortedDatesList = measureDays
                .Select(day =>
                {
                    var nextDay = currentDate.Date.AddDays((day.DayNumber - currentDayOfWeek + 7) % 7);
                    var dateTime = nextDay.Add(day.TimeSpan);

                    return new MeasureDates
                    {
                        Datetime = dateTime,
                        Place = day.Place,
                        MeasureDivisionsInfos = day.MeasureDivisionsInfo
                    };
                })
                .Where(date => date.Datetime > currentDate)
                .OrderBy(date => date.Datetime)
                .ToList();
            return sortedDatesList;
        }
    }
}
