using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FairShare.Web.Data;

namespace FairShare.Web.Controllers
{
    [Route("debug")]
    public class DebugController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;

        public DebugController(ApplicationDbContext db, IWebHostEnvironment env, IConfiguration config)
        {
            _db = db;
            _env = env;
            _config = config;
            if (!_env.IsDevelopment())
                throw new InvalidOperationException("Debug endpoints are only available in Development.");
        }

        // GET /debug/db
        [HttpGet("db")]
        public async Task<IActionResult> Db()
        {
            // Basic counts
            var usersCount   = await _db.Users.CountAsync();
            var groupsCount  = await _db.Groups.CountAsync();
            var gmCount      = await _db.GroupMembers.CountAsync();
            var expensesCount= await _db.Expenses.CountAsync();
            var sharesCount  = await _db.ExpenseShares.CountAsync();

            // Quick samples
            var recentUsers = await _db.Users
                .AsNoTracking()
                .OrderByDescending(u => u.Id)
                .Select(u => new { u.Id, u.Name, u.Email, u.CreatedUtc })
                .Take(5).ToListAsync();

            var recentGroups = await _db.Groups
                .AsNoTracking()
                .OrderByDescending(g => g.Id)
                .Select(g => new { g.Id, g.Name, g.Description, g.CreatedUtc })
                .Take(5).ToListAsync();

            var recentExpenses = await _db.Expenses
                .AsNoTracking()
                .OrderByDescending(e => e.Id)
                .Select(e => new { e.Id, e.Description, e.Amount, e.GroupId, e.PaidByUserId, e.SpentOnUtc })
                .Take(5).ToListAsync();

            // Which provider + where is the DB (helpful for SQLite)
            var provider = _config["DatabaseProvider"] ?? "SqlServer";
            var connStr  = _config.GetConnectionString("DefaultConnection") ?? "(none)";

            return Json(new
            {
                Environment = _env.EnvironmentName,
                Provider = provider,
                ConnectionString = connStr,
                Counts = new {
                    Users = usersCount,
                    Groups = groupsCount,
                    GroupMembers = gmCount,
                    Expenses = expensesCount,
                    ExpenseShares = sharesCount
                },
                Samples = new {
                    Users = recentUsers,
                    Groups = recentGroups,
                    Expenses = recentExpenses
                }
            });
        }
    }
}
