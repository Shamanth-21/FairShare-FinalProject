using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FairShare.Web.Data;      // DbContext namespace
using FairShare.Web.Models;    // Expense, Group, User, ExpenseShare, GroupMember

namespace FairShare.Web.Controllers
{
    public class ExpensesController : Controller
    {
        private readonly ApplicationDbContext _ctx;
        public ExpensesController(ApplicationDbContext ctx) => _ctx = ctx;

        // GET: /Expenses
        public async Task<IActionResult> Index()
        {
            var list = await _ctx.Expenses
                .Include(e => e.Group)
                .Include(e => e.PaidByUser)
                .AsNoTracking()
                .OrderByDescending(e => e.SpentOnUtc)
                .ToListAsync();

            return View(list);
        }

        // GET: /Expenses/Create
        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();
            return View(new Expense { SpentOnUtc = System.DateTime.UtcNow });
        }

        // POST: /Expenses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Description,Amount,GroupId,PaidByUserId,SpentOnUtc")] Expense e)
        {
            // 🔎 prove the action is hit (comment out when done)
            ViewData["DebugHit"] = "POST /Expenses/Create was hit";

            // Basic validation
            if (string.IsNullOrWhiteSpace(e.Description))
                ModelState.AddModelError(nameof(e.Description), "Description is required.");
            if (e.Amount <= 0)
                ModelState.AddModelError(nameof(e.Amount), "Enter a positive amount.");

            // normalize date to UTC
            if (e.SpentOnUtc.Kind == System.DateTimeKind.Unspecified)
                e.SpentOnUtc = System.DateTime.SpecifyKind(e.SpentOnUtc, System.DateTimeKind.Utc);

            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View(e);
            }

            try
            {
                // fields many schemas require
                e.CreatedUtc = System.DateTime.UtcNow;

                _ctx.Expenses.Add(e);
                await _ctx.SaveChangesAsync();

                // mirror your API: create shares for group members
                var memberIds = await _ctx.GroupMembers
                    .Where(m => m.GroupId == e.GroupId)
                    .Select(m => m.UserId)
                    .ToListAsync();

                if (memberIds.Count > 0)
                {
                    var each = System.Math.Round(e.Amount / memberIds.Count, 2);
                    foreach (var uid in memberIds)
                    {
                        _ctx.ExpenseShares.Add(new ExpenseShare
                        {
                            ExpenseId = e.Id,
                            UserId = uid,
                            ShareAmount = each,
                            IsSettled = uid == e.PaidByUserId
                        });
                    }
                    await _ctx.SaveChangesAsync();
                }

                TempData["Toast"] = "Expense added.";
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception ex)
            {
                // ✅ Surface the real DB error (FK/NOT NULL/etc.) on the form
                ModelState.AddModelError(string.Empty, "Save failed: " + ex.Message);
                await LoadDropdowns();
                return View(e);
            }
        }

        private async Task LoadDropdowns()
        {
            ViewBag.GroupId = new SelectList(
                await _ctx.Groups.AsNoTracking().OrderBy(g => g.Name).ToListAsync(),
                "Id", "Name");

            ViewBag.PaidByUserId = new SelectList(
                await _ctx.Users.AsNoTracking().OrderBy(u => u.Name).ToListAsync(),
                "Id", "Name");
        }
    }
}
