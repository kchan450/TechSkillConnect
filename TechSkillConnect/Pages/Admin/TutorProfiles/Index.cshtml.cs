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

        public string CurrentFilter { get; set; } = string.Empty;
        public string CurrentSort { get; set; } = string.Empty;

        public PaginatedList<Tutor> Tutors { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string sortOrder, string currentFilter, int? pageIndex)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Only allow Admin to access
            if (userId != "db0c6b20-0021-425e-998e-c3feb36c6364")
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

            // 🔥 Load all tutors, include TutorProfile if exists
            var tutorsIQ = _context.Tutors
                .IgnoreQueryFilters()
                .Include(t => t.TutorProfile)
                .AsNoTracking();

            // 🔍 Apply search if needed
            if (!string.IsNullOrEmpty(SearchString))
            {
                tutorsIQ = tutorsIQ.Where(t =>
                    t.TutorEmail.Contains(SearchString) ||
                    t.Tutor_phone.Contains(SearchString));
            }

            int pageSize = 10; // 10 tutors per page
            Tutors = await PaginatedList<Tutor>.CreateAsync(tutorsIQ, pageIndex ?? 1, pageSize);

            return Page();
        }
    }
}
