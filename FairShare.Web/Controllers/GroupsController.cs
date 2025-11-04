using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FairShare.Web.Data;
using FairShare.Web.Models;

namespace FairShare.Web.Controllers
{
    public class GroupsController : Controller
    {
        private readonly ApplicationDbContext _ctx;
        public GroupsController(ApplicationDbContext ctx) { _ctx = ctx; }

        public async Task<IActionResult> Index() =>
            View(await _ctx.Groups.Include(g => g.Members).ToListAsync());

        public IActionResult Create() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Group group)
        {
            if (!ModelState.IsValid) return View(group);
            group.CreatedUtc = DateTime.UtcNow;
            _ctx.Groups.Add(group);
            await _ctx.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var group = await _ctx.Groups.FindAsync(id);
            if (group == null) return NotFound();
            return View(group);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Group group)
        {
            if (id != group.Id) return BadRequest();
            if (!ModelState.IsValid) return View(group);
            _ctx.Update(group);
            await _ctx.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var group = await _ctx.Groups
                .Include(g => g.Members).ThenInclude(m => m.User)
                .Include(g => g.Expenses).ThenInclude(e => e.PaidByUser)
                .FirstOrDefaultAsync(g => g.Id == id);
            if (group == null) return NotFound();
            return View(group);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var group = await _ctx.Groups.FindAsync(id);
            if (group == null) return NotFound();
            return View(group);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var group = await _ctx.Groups.FindAsync(id);
            if (group != null)
            {
                _ctx.Groups.Remove(group);
                await _ctx.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
