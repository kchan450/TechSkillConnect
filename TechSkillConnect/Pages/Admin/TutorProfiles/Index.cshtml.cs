using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TechSkillConnect.Data;
using TechSkillConnect.Models;

namespace TechSkillConnect.Pages.Admin.TutorProfiles
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }

        public List<Tutor> Tutors { get; set; } = new List<Tutor>(); // Change to List, not PaginatedList for now

        public async Task<IActionResult> OnGetAsync(string sortOrder, string currentFilter, int? pageIndex)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != "db0c6b20-0021-425e-998e-c3feb36c6364") // Admin ID
            {
                return RedirectToPage("/Identity/Account/Login");
            }

            CurrentSort = sortOrder;

            if (!string.IsNullOrEmpty(SearchString))
            {
                pageIndex = 1;
                CurrentFilter = SearchString;
            }
            else
            {
                SearchString = currentFilter;
                CurrentFilter = currentFilter;
            }

            // 🔥 Load all tutors, IGNORE filters, whether they have profile or not
            var tutorsIQ = _context.Tutors
                .IgnoreQueryFilters()
                .Include(t => t.TutorProfile)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(SearchString))
            {
                tutorsIQ = tutorsIQ.Where(t =>
                    t.TutorEmail.Contains(SearchString) ||
                    t.Tutor_phone.Contains(SearchString));
            }

            // 🛠 Load all tutors into a List (no pagination for now)
            Tutors = await tutorsIQ.ToListAsync();

            // 🔎 Debugging - print number of tutors loaded
            System.Diagnostics.Debug.WriteLine($"Total tutors loaded: {Tutors.Count}");

            return Page();
        }
    }
}
