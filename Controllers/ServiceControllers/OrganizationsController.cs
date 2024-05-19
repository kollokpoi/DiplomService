using DiplomService.Database;
using DiplomService.ViewModels.OrganizationViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiplomService.Controllers.ServiceControllers
{
    public class OrganizationsController : Controller
    {
        private readonly ApplicationContext _context;

        public OrganizationsController(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return _context.Organizations != null ?
                        View(await _context.Organizations.Where(x => x.ReadyToShow).ToListAsync()) :
                        Problem("Entity set 'ApplicationContext.Organizations'  is null.");
        }

        public async Task<IActionResult> Details(int? id)
        {
            var organization = await _context.Organizations.FirstOrDefaultAsync(x => x.Id == id);
            if (organization is null)
                return NotFound();

            OrganizationViewModel organizationViewModel = new()
            {
                Organization = organization,
            };

            return View(organizationViewModel);
        }

        [Authorize(Roles = "OrganizationUser")]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "OrganizationUser")]
        public async Task<IActionResult> Create(OrganizationViewModel organizationViewModel)
        {
            var organization = organizationViewModel.Organization;
            if (ModelState.IsValid)
            {
                _context.Add(organization);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(organization);
        }


        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id, string? returnUrl)
        {
            if (id == null || _context.Organizations == null)
            {
                return NotFound();
            }

            var Organization = await _context.Organizations.FirstOrDefaultAsync(x => x.Id == id);
            if (Organization == null)
            {
                return NotFound();
            }
            ViewBag.ReturnUrl = returnUrl;
            return View(Organization);
        }

        // POST: Chats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int id, string? returnUrl)
        {
            if (_context.Organizations == null)
            {
                return Problem("Entity set 'ApplicationContext.Organizations'  is null.");
            }
            var Organization = await _context.Organizations.FirstOrDefaultAsync(x => x.Id == id);
            if (Organization != null)
            {
                _context.Organizations.Remove(Organization);
            }

            await _context.SaveChangesAsync();
            if (returnUrl != null)
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction(nameof(Index));
        }


        private bool OrganizationExists(int id)
        {
            return (_context.Organizations?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
