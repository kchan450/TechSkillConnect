using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TechSkillConnect.Data;
using TechSkillConnect.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace TechSkillConnect.Pages.Admin.TutorProfiles
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

    //    public IList<TutorProfile> TutorProfiles { get; set; } = new List<TutorProfile>();

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SelectedLanguage { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SelectedSkillLevel { get; set; }

        public SelectList? LanguageOptions { get; set; }
        public SelectList? SkillLevelOptions { get; set; }

        public string LanguageSort { get; set; }
        public string YearsOfExperienceSort { get; set; }
        public string SkillLevelSort { get; set; }
        public string CertificateSort { get; set; }
        public string FeePerSessionSort { get; set; }
        public string SelfIntroSort { get; set; }
        public string SelfHeadlineSort { get; set; }

        public string CurrentSort { get; set; }
        public string CurrentFilter { get; set; }

        public PaginatedList<TutorProfile> TutorProfiles { get; set; }

        public async Task OnGetAsync(string sortOrder, string currentFilter, string SearchString, int? pageIndex)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != "d1a93e6f-8a9b-455e-9b47-5fc12458e530") //admin user id example for account t1@t.com
            {
                Response.Redirect("/Identity/Account/Login");
                return;
            }

            CurrentSort = sortOrder;
            LanguageSort = sortOrder == "language" ? "language_desc" : "language";
            YearsOfExperienceSort = sortOrder == "yearsOfExperience" ? "yearsOfExperience_desc" : "yearsOfExperience";
            SkillLevelSort = sortOrder == "skillLevel" ? "skillLevel_desc" : "skillLevel";
            CertificateSort = sortOrder == "certificate" ? "certificate_desc" : "certificate";
            FeePerSessionSort = sortOrder == "feePerSession" ? "feePerSession_desc" : "feePerSession";
            SelfIntroSort = sortOrder == "selfIntro" ? "selfIntro_desc" : "selfIntro";
            SelfHeadlineSort = sortOrder == "selfHeadline" ? "selfHeadline_desc" : "selfHeadline";

            if (SearchString != null)
            {
                pageIndex = 1;

            }
            else
            {
                SearchString = currentFilter;
            }
            currentFilter = SearchString;

            IQueryable<TutorProfile> tutorsProfileIQ = from t in _context.TutorProfiles
                                         select t;
            switch (sortOrder)
            {
                case "language_desc":

                    tutorsProfileIQ = tutorsProfileIQ.OrderByDescending(t => t.Language);
                    break;

                case "yearsOfExperience_desc":

                    tutorsProfileIQ = tutorsProfileIQ.OrderByDescending(t => t.YearsOfExperience);
                    break;

                case "skillLevel_desc":

                    tutorsProfileIQ = tutorsProfileIQ.OrderByDescending(t => t.SkillLevel);
                    break;

                case "certificate_desc":

                    tutorsProfileIQ = tutorsProfileIQ.OrderByDescending(t => t.Certificate);
                    break;

                case "feePerSession_desc":

                    tutorsProfileIQ = tutorsProfileIQ.OrderByDescending(t => t.FeePerSession);
                    break;

                case "selfIntro_desc":

                    tutorsProfileIQ = tutorsProfileIQ.OrderByDescending(t => t.SelfIntro);
                    break;

                case "selfHeadline_desc":

                    tutorsProfileIQ = tutorsProfileIQ.OrderBy(t => t.SelfHeadline);
                    break;


                case "language":

                    tutorsProfileIQ = tutorsProfileIQ.OrderBy(t => t.Language);
                    break;

                case "yearsOfExperience":

                    tutorsProfileIQ = tutorsProfileIQ.OrderBy(t => t.YearsOfExperience);
                    break;

                case "skillLevel":

                    tutorsProfileIQ = tutorsProfileIQ.OrderBy(t => t.SkillLevel);
                    break;

                case "certificate":

                    tutorsProfileIQ = tutorsProfileIQ.OrderBy(t => t.Certificate);
                    break;

                case "feePerSession":

                    tutorsProfileIQ = tutorsProfileIQ.OrderBy(t => t.FeePerSession);
                    break;

                case "selfIntro":

                    tutorsProfileIQ = tutorsProfileIQ.OrderBy(t => t.SelfIntro);
                    break;


                case "selfHeadline":

                    tutorsProfileIQ = tutorsProfileIQ.OrderBy(t => t.SelfHeadline);
                    break;
                    
                default:

                    tutorsProfileIQ = tutorsProfileIQ.OrderBy(t => t.Language);
                    break;

            }

         //   TutorProfiles = await _context.TutorProfiles.ToListAsync();

            var languageQuery = _context.TutorProfiles
               .OrderBy(t => t.Language)
               .Select(t => t.Language)
               .Distinct();

            var SkillLevelQuery = _context.TutorProfiles
              .OrderBy(t => t.SkillLevel)
              .Select(t => t.SkillLevel)
              .Distinct();


            LanguageOptions = new SelectList(await languageQuery.ToListAsync());
            SkillLevelOptions = new SelectList(await SkillLevelQuery.ToListAsync());

            // Define the base query for tutors profiles
   
            //var query = tutorsProfileIQ;
           // var query = _context.TutorProfiles.AsQueryable();


            // Apply search filter
            if (!string.IsNullOrEmpty(SearchString))
            {
                tutorsProfileIQ = tutorsProfileIQ.Where(t =>
                    t.SkillLevel.Contains(SearchString) ||
                    t.Language.Contains(SearchString) ||
                    t.YearsOfExperience.ToString().Contains(SearchString) ||
                    t.Certificate.Contains(SearchString) ||
                    t.FeePerSession.ToString().Contains(SearchString) ||
                    t.SelfIntro.Contains(SearchString));
            }

            // Apply language filter
            if (!string.IsNullOrEmpty(SelectedLanguage))
            {
                tutorsProfileIQ = tutorsProfileIQ.Where(t => t.Language == SelectedLanguage);
            }

            // Apply skill level filter
            if (!string.IsNullOrEmpty(SelectedSkillLevel))
            {
                tutorsProfileIQ = tutorsProfileIQ.Where(t => t.SkillLevel == SelectedSkillLevel);
            }

            // Fetch tutor profiles
           // TutorProfiles = await query.ToListAsync();

            int pageSize = 10;
            TutorProfiles = await PaginatedList<TutorProfile>.CreateAsync(tutorsProfileIQ.AsNoTracking(), pageIndex ?? 1, pageSize);

        }
    }
}
