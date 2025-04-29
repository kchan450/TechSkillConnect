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

            // Check if user is Admin
            if (userId != "db0c6b20-0021-425e-998e-c3feb36c6364")
            {
                return RedirectToPage("/Identity/Account/Login");
            }

            CurrentSort = sortOrder;

            // If user types new search, reset page index
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

            // Query all tutors, whether or not they have a profile
            var tutorsIQ = _context.Tutors
                .Select(t => new Tutor
                {
                    TutorID = t.TutorID,
                    TutorEmail = t.TutorEmail,
                    Tutor_phone = t.Tutor_phone,
                    TutorProfile = t.TutorProfile
                })
                .AsNoTracking();

            // Apply search filter if needed
            if (!string.IsNullOrEmpty(SearchString))
            {
                tutorsIQ = tutorsIQ.Where(t =>
                    t.TutorEmail.Contains(SearchString) ||
                    t.Tutor_phone.Contains(SearchString));
            }

            // Pagination
            int pageSize = 10;
            Tutors = await PaginatedList<Tutor>.CreateAsync(tutorsIQ, pageIndex ?? 1, pageSize);

            return Page();
        }
    }
}
