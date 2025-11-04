using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FairShare.Web.Data;
using FairShare.Web.Models;

namespace FairShare.Web.Controllers
{
    public class ExpensesController : Controller
    {
        private readonly ApplicationDbContext _ctx;
        public ExpensesController(ApplicationDbContext ctx) { _ctx = ctx; }

        public async Task<IActionResult> Index()
        {
            var q = _ctx.Expenses
                .Include(e => e.Group)
                .Include(e => e.PaidByUser)
                .OrderByDescending(e => e.SpentOnUtc);
            return View(await q.ToListAsync());
        }

        public async Task<IActionResult> Create()
        {
            await PopulateLookups();
            return View(new Expense { SpentOnUtc = DateTime.UtcNow.Date });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Expense expense)
        {
            if (!ModelState.IsValid)
            {
                await PopulateLookups();
                return View(expense);
            }

            _ctx.Expenses.Add(expense);
            await _ctx.SaveChangesAsync();

            var memberIds = await _ctx.GroupMembers
                .Where(m => m.GroupId == expense.GroupId)
                .Select(m => m.UserId)
                .ToListAsync();

            if (memberIds.Count > 0)
            {
                var each = Math.Round(expense.Amount / memberIds.Count, 2);
                foreach (var uid in memberIds)
                {
                    _ctx.ExpenseShares.Add(new ExpenseShare
                    {
                        ExpenseId = expense.Id,
                        UserId = uid,
                        ShareAmount = each,
                        IsSettled = uid == expense.PaidByUserId
                    });
                }
                await _ctx.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var expense = await _ctx.Expenses.FindAsync(id);
            if (expense == null) return NotFound();
            await PopulateLookups();
            return View(expense);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Expense expense)
        {
            if (id != expense.Id) return BadRequest();
            if (!ModelState.IsValid)
            {
                await PopulateLookups();
                return View(expense);
            }
            _ctx.Update(expense);
            await _ctx.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var expense = await _ctx.Expenses
                .Include(e => e.Group)
                .Include(e => e.PaidByUser)
                .Include(e => e.Shares).ThenInclude(s => s.User)
                .FirstOrDefaultAsync(e => e.Id == id);
            if (expense == null) return NotFound();
            return View(expense);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var expense = await _ctx.Expenses
                .Include(e => e.Group)
                .Include(e => e.PaidByUser)
                .FirstOrDefaultAsync(e => e.Id == id);
            if (expense == null) return NotFound();
            return View(expense);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var expense = await _ctx.Expenses.FindAsync(id);
            if (expense != null)
            {
                _ctx.Expenses.Remove(expense);
                await _ctx.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateLookups()
        {
            ViewBag.Groups = new SelectList(await _ctx.Groups.OrderBy(g => g.Name).ToListAsync(), "Id", "Name");
            ViewBag.Users  = new SelectList(await _ctx.Users.OrderBy(u => u.Name).ToListAsync(), "Id", "Name");
        }
    }
}
