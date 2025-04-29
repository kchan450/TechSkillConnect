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

        public PaginatedList<Tutor> Tutors { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string sortOrder, string currentFilter, int? pageIndex)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != "db0c6b20-0021-425e-998e-c3feb36c6364") // Admin ID
            {
                return RedirectToPage("/Identity/Account/Login");
            }

            CurrentSort = sortOrder;

            // If search box has input, treat as a new search
            if (!string.IsNullOrEmpty(SearchString))
            {
                pageIndex = 1;
                CurrentFilter = SearchString; // Update filter
            }
            else
            {
                SearchString = currentFilter; // Stay on previous search if paging
                CurrentFilter = currentFilter; // Keep showing previous filter text
            }

            IQueryable<Tutor> tutorsIQ = _context.Tutors
                .Include(t => t.TutorProfile)
                .AsNoTracking();

            // ❗ Apply search filter ONLY if SearchString is NOT EMPTY
            if (!string.IsNullOrEmpty(SearchString))
            {
                tutorsIQ = tutorsIQ.Where(t =>
                    t.TutorEmail.Contains(SearchString) ||
                    t.Tutor_phone.Contains(SearchString));
            }

            int pageSize = 10;
            Tutors = await PaginatedList<Tutor>.CreateAsync(tutorsIQ, pageIndex ?? 1, pageSize);

            return Page();
        }

    }
}
