using DiplomService.Database;
using DiplomService.Models;
using DiplomService.ViewModels.News;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DiplomService.Controllers
{
    public class NewsController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<User> _userManager;

        public NewsController(ApplicationContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int? id)
        {
            var model = new NewsListViewModel();

            if (id is null)
            {
                var list = await _context.News.Where(x => x.Event.PublicEvent).OrderByDescending(x=>x.DateTime).ToListAsync();
                model.News = list;
            }
            else
            {
                var @event = await _context.Events.FirstOrDefaultAsync(x => x.Id == id);
                model.EventId = id;

                if (@event != null)
                    model.EventName = @event.Name;

                model.News = await _context.News.Where(x => x.EventId == id).OrderByDescending(x => x.DateTime).ToListAsync();


                if (model == null)
                    return BadRequest();

                ViewBag.EventId = id;
            }
            
            return View(model);
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.News == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .FirstOrDefaultAsync(m => m.Id == id);
            if (news == null)
            {
                return NotFound();
            }
            if (await UserAccessed(news.Event))
                ViewBag.Editer = true;

            return View(news);
        }

        public async Task<IActionResult> Create(int id)
        {
            var @event = await _context.Events.FirstOrDefaultAsync(x => x.Id == id);
            if (@event is null)
                return NotFound();
            if (!await UserAccessed(@event))
                return Unauthorized();
         
            var model = new NewsViewModel()
            {
                EventId= id,
                EventName =  @event.Name
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewsViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var @event = await _context.Events.FirstOrDefaultAsync(x => x.Id == model.EventId);
            if (@event is null)
                return BadRequest();

            if (!await UserAccessed(@event))
                return Unauthorized();

            var newNews = new News()
            {
                DateTime = DateTime.Now,
                Description = model.Description,
                Event = @event,
                Image = model.PriviewImage,
                Title = model.Name,
                Sections = model.Sections is null ? null : new()
            };

            if (model.Sections is not null)
            {
                newNews.Sections = new();
                foreach (var item in model.Sections)
                {
                    if (item.Image is not null || item.Name!=string.Empty || item.Description!=string.Empty)
                    {
                        newNews.Sections.Add(new()
                        {
                            Title = item.Name,
                            Description = item.Description,
                            Image = item.Image,
                            News = newNews,
                        });
                    }
                }
            }

            _context.News.Add(newNews);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { Id = model.EventId});
        }

        public async Task<IActionResult> Edit(int id)
        {
            var news = await _context.News.FirstOrDefaultAsync(x => x.Id == id);
            if (news is null)
                return NotFound();

            if (!await UserAccessed(news.Event))
                return Unauthorized();

            var model = new NewsViewModel()
            {
                Id = id,
                EventId = id,
                EventName = news.Event.Name,
                PriviewImage = news.Image,
                Description= news.Description,
                Name = news.Title,
            };

            if (news.Sections is not null)
            {
                model.Sections = new();
                foreach (var item in news.Sections)
                {
                    model.Sections.Add(new()
                    {
                        Id= item.Id,
                        Name = item.Title,
                        Description = item.Description,
                        Image= item.Image,
                    });
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(NewsViewModel model)
        {
            var news = await _context.News.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (news is null)
                return NotFound();

            if (!await UserAccessed(news.Event))
                return Unauthorized();

            if (news.Image != model.PriviewImage && model.PriviewImage is not null)
                news.Image = model.PriviewImage;
            if (news.Title != model.Name)
                news.Title = model.Name;
            if (news.Description != model.Description)
                news.Description = model.Description;

            if (model.Sections is not null)
            {
                if (news.Sections is null)
                    news.Sections = new();

                foreach (var item in model.Sections)
                {
                    var newsSection = news.Sections.FirstOrDefault(x => x.Id == item.Id && item.Id!=0);
                    if (newsSection is not null)
                    {
                        if (item.ToDelete)
                            news.Sections.Remove(newsSection);
                        else
                        {
                            if (newsSection.Image != item.Image && item.Image is not null)
                                newsSection.Image = item.Image;
                            if (newsSection.Title != item.Name)
                                newsSection.Title = item.Name;
                            if (newsSection.Description != item.Description)
                                newsSection.Description = item.Description;
                        }
                    }
                    else
                    {
                        news.Sections.Add(new()
                        {
                            Title = item.Name,
                            Description = item.Description,
                            Image = item.Image,
                            News = news,
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new {Id = model.Id});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(Details), new { id });

            var news = await _context.News.FirstOrDefaultAsync(x=>x.Id==id);
            if (news is null)
                return BadRequest();

            if (!await UserAccessed(news.Event))
                return Unauthorized();

            _context.News.Remove(news);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { Id = news.EventId });
        }
        public  IActionResult GetParticipantEntry( int Index)
        {
            ViewBag.Index = Index;
            return PartialView("_NewsSectionEntry", Index);
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
    }
}
