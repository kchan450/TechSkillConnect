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

        public string TutorEmailSort { get; set; }
        public string LanguageSort { get; set; }
        public string YearsOfExperienceSort { get; set; }
        public string SkillLevelSort { get; set; }
        public string FeePerSessionSort { get; set; }
        public string SelfIntroSort { get; set; }
        public string SelfHeadlineSort { get; set; }

        public string CurrentSort { get; set; }
        public string CurrentFilter { get; set; }

        public PaginatedList<TutorProfile> TutorProfiles { get; set; }

        public async Task OnGetAsync(string sortOrder, string currentFilter, int? pageIndex)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != "db0c6b20-0021-425e-998e-c3feb36c6364") // Admin user ID
            {
                Response.Redirect("/Identity/Account/Login");
                return;
            }

            CurrentSort = sortOrder;
            TutorEmailSort = sortOrder == "tutorEmail" ? "tutorEmail_desc" : "tutorEmail";
            LanguageSort = sortOrder == "language" ? "language_desc" : "language";
            YearsOfExperienceSort = sortOrder == "yearsOfExperience" ? "yearsOfExperience_desc" : "yearsOfExperience";
            SkillLevelSort = sortOrder == "skillLevel_desc" ? "skillLevel_desc" : "skillLevel";
            FeePerSessionSort = sortOrder == "feePerSession_desc" ? "feePerSession" : "feePerSession_desc";
            SelfIntroSort = sortOrder == "selfIntro_desc" ? "selfIntro" : "selfIntro_desc";
            SelfHeadlineSort = sortOrder == "selfHeadline_desc" ? "selfHeadline" : "selfHeadline_desc";

            if (!string.IsNullOrEmpty(SearchString))
            {
                pageIndex = 1; // New search, go back to page 1
            }
            else
            {
                SearchString = currentFilter; // Keep previous search if no new search
            }
            CurrentFilter = SearchString;

            IQueryable<TutorProfile> tutorsProfileIQ = _context.TutorProfiles.Include(tp => tp.Tutor);

            // Apply search filter
            if (!string.IsNullOrEmpty(SearchString))
            {
                tutorsProfileIQ = tutorsProfileIQ.Where(tp =>
                    tp.Tutor.TutorEmail.Contains(SearchString) ||
                    tp.Tutor.Tutor_phone.Contains(SearchString));
            }

            // Apply sorting
            tutorsProfileIQ = sortOrder switch
            {
                "tutorEmail" => tutorsProfileIQ.OrderBy(t => t.Tutor.TutorEmail),
                "tutorEmail_desc" => tutorsProfileIQ.OrderByDescending(t => t.Tutor.TutorEmail),
                "language" => tutorsProfileIQ.OrderBy(t => t.Language),
                "language_desc" => tutorsProfileIQ.OrderByDescending(t => t.Language),
                "yearsOfExperience" => tutorsProfileIQ.OrderBy(t => t.YearsOfExperience),
                "yearsOfExperience_desc" => tutorsProfileIQ.OrderByDescending(t => t.YearsOfExperience),
                "skillLevel" => tutorsProfileIQ.OrderBy(t => t.SkillLevel),
                "skillLevel_desc" => tutorsProfileIQ.OrderByDescending(t => t.SkillLevel),
                "feePerSession" => tutorsProfileIQ.OrderBy(t => t.FeePerSession),
                "feePerSession_desc" => tutorsProfileIQ.OrderByDescending(t => t.FeePerSession),
                "selfIntro" => tutorsProfileIQ.OrderBy(t => t.SelfIntro),
                "selfIntro_desc" => tutorsProfileIQ.OrderByDescending(t => t.SelfIntro),
                "selfHeadline" => tutorsProfileIQ.OrderBy(t => t.SelfHeadline),
                "selfHeadline_desc" => tutorsProfileIQ.OrderByDescending(t => t.SelfHeadline),
                _ => tutorsProfileIQ.OrderBy(t => t.Language),
            };

            int pageSize = 10;
            TutorProfiles = await PaginatedList<TutorProfile>.CreateAsync(tutorsProfileIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
        }

    }
}
