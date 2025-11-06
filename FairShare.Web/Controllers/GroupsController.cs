using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FairShare.Web.Data;
using FairShare.Web.Models;
using FairShare.Web.Services;

namespace FairShare.Web.Controllers
{
    public class GroupsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrencyRateService _rateService;

        public GroupsController(ApplicationDbContext context, ICurrencyRateService rateService)
        {
            _context = context;
            _rateService = rateService;
        }

        // GET: /Groups
        public async Task<IActionResult> Index()
        {
            var groups = await _context.Groups
                .AsNoTracking()
                .OrderBy(g => g.Name)
                .ToListAsync();

            return View(groups);
        }

        // GET: /Groups/Details/5[?to=EUR]
        public async Task<IActionResult> Details(int id, string? to)
        {
            var group = await _context.Groups
                .Include(g => g.Expenses)
                    .ThenInclude(e => e.PaidByUser)
                .Include(g => g.Expenses)
                    .ThenInclude(e => e.ExpenseShares)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (group is null) return NotFound();

            // Base total in USD (stored currency)
            var total = group.Expenses?.Sum(e => e.Amount) ?? 0m;
            ViewBag.TotalUSD = $"USD {total:0.00}";

            // Optional currency conversion via fetch-only API
            if (!string.IsNullOrWhiteSpace(to) &&
                !string.Equals(to, "USD", StringComparison.OrdinalIgnoreCase))
            {
                var rate = await _rateService.GetRateAsync("USD", to);
                ViewBag.ConvertedTo = to.ToUpperInvariant();
                ViewBag.ConvertedTotal = rate.HasValue
                    ? $"{to.ToUpperInvariant()} {(total * rate.Value):0.00}"
                    : "Conversion unavailable right now.";
            }

            return View(group);
        }

        // GET: /Groups/Create
        public IActionResult Create()
        {
            return View(new Group { CreatedUtc = DateTime.UtcNow });
        }

        // POST: /Groups/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description")] Group group)
        {
            if (!ModelState.IsValid) return View(group);

            group.CreatedUtc = DateTime.UtcNow;
            _context.Add(group);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = group.Id });
        }

        // GET: /Groups/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group is null) return NotFound();
            return View(group);
        }

        // POST: /Groups/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,CreatedUtc")] Group group)
        {
            if (id != group.Id) return BadRequest();
            if (!ModelState.IsValid) return View(group);

            try
            {
                _context.Update(group);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = group.Id });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Groups.AnyAsync(g => g.Id == id))
                    return NotFound();
                throw;
            }
        }

        // GET: /Groups/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var group = await _context.Groups.AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (group is null) return NotFound();

            return View(group);
        }

        // POST: /Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group is null) return NotFound();

            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
