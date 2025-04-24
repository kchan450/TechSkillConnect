using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TechSkillConnect.Data;
using TechSkillConnect.Models;

namespace TechSkillConnect.Pages.Learnerpage
{
    [Authorize(Roles = "Learner")]
    public class FindMyTutorModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public FindMyTutorModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? SelectedCountry { get; set; }
        public SelectList? CountryOptions { get; set; }

        public string FirstnameSort { get; set; }
        public string LastnameSort { get; set; }
 
        public string CountrySort { get; set; }
        public string LanguageSort { get; set; }
        public string ExperienceSort { get; set; }
        public string SkillLevelSort { get; set; }
        public string FeeSort { get; set; }


        public string CurrentSort { get; set; }

        public string CurrentFilter { get; set; }

        public PaginatedList<Tutor> Tutors { get; set; }

        public async Task OnGetAsync(string sortOrder, string currentFilter, string SearchString, int? pageIndex)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            CurrentSort = sortOrder;
            FirstnameSort = sortOrder == "firstname" ? "firstname_desc" : "firstname";
            LastnameSort = sortOrder == "lastname" ? "lastname_desc" : "lastname";


           
            CountrySort = sortOrder == "country" ? "country_desc" : "country";
            LanguageSort = sortOrder == "language" ? "language_desc" : "language";
            ExperienceSort = sortOrder == "experience" ? "experience_desc" : "experience";
            SkillLevelSort = sortOrder == "skill" ? "skill_desc" : "skill";
            FeeSort = sortOrder == "fee" ? "fee_desc" : "fee";

            if (SearchString != null)
            {
                pageIndex = 1;

            }
            else
            {
                SearchString = currentFilter;
            }
            currentFilter = SearchString;

            //IQueryable<Tutor> tutorsIQ = _context.Tutors
            //                            .Include(t => t.TutorProfile); // Eagerly load related TutorProfile

            var tutorsIQ = from tutor in _context.Tutors
                           join profile in _context.TutorProfiles
                           on tutor.TutorID equals profile.TutorID
                           select new Tutor
                           {
                               TutorID = tutor.TutorID,
                               Tutor_firstname = tutor.Tutor_firstname,
                               Tutor_lastname = tutor.Tutor_lastname,
                               TutorEmail = tutor.TutorEmail,
                               Tutor_phone = tutor.Tutor_phone,
                               CountryOfBirth = tutor.CountryOfBirth,
                               TutorProfile = profile // if you want to attach it manually
                           };
            //IQueryable<Tutor> tutorsIQ = from t in _context.Tutors
            //                             select t;


            tutorsIQ = sortOrder switch
            {
                "firstname" => tutorsIQ.OrderBy(t => t.Tutor_firstname),
                "firstname_desc" => tutorsIQ.OrderByDescending(t => t.Tutor_firstname),
                "lastname" => tutorsIQ.OrderBy(t => t.Tutor_lastname),
                "lastname_desc" => tutorsIQ.OrderByDescending(t => t.Tutor_lastname),
                "country" => tutorsIQ.OrderBy(t => t.CountryOfBirth),
                "country_desc" => tutorsIQ.OrderByDescending(t => t.CountryOfBirth),

                "language" => tutorsIQ.OrderBy(t => t.TutorProfile.Language),
                "language_desc" => tutorsIQ.OrderByDescending(t => t.TutorProfile.Language),
                "experience" => tutorsIQ.OrderBy(t => t.TutorProfile.YearsOfExperience),
                "experience_desc" => tutorsIQ.OrderByDescending(t => t.TutorProfile.YearsOfExperience),
                "skill" => tutorsIQ.OrderBy(t => t.TutorProfile.SkillLevel),
                "skill_desc" => tutorsIQ.OrderByDescending(t => t.TutorProfile.SkillLevel),
                "fee" => tutorsIQ.OrderBy(t => t.TutorProfile.FeePerSession),
                "fee_desc" => tutorsIQ.OrderByDescending(t => t.TutorProfile.FeePerSession),

                _ => tutorsIQ.OrderBy(t => t.Tutor_lastname)
            };

            // Fetch distinct country values dynamically
            var countryQuery = _context.Tutors
                .OrderBy(t => t.CountryOfBirth)
                .Select(t => t.CountryOfBirth)
                .Distinct();

            CountryOptions = new SelectList(await countryQuery.ToListAsync());

            // Apply country filter
            if (!string.IsNullOrEmpty(SelectedCountry))
            {
                tutorsIQ = tutorsIQ.Where(t => t.CountryOfBirth == SelectedCountry);
            }

            // Apply search filter
            if (!string.IsNullOrEmpty(SearchString))
            {
                tutorsIQ = tutorsIQ.Where(t =>
                    t.Tutor_firstname.Contains(SearchString) ||
                    t.Tutor_lastname.Contains(SearchString) ||
                    t.CountryOfBirth.Contains(SearchString) ||

                    t.TutorProfile.Language.Contains(SearchString) ||
                    t.TutorProfile.YearsOfExperience.Contains(SearchString) ||
                    t.TutorProfile.SkillLevel.Contains(SearchString) ||
                    t.TutorProfile.FeePerSession.ToString().Contains(SearchString)
                );
            }
            int pageSize = 10;

            // Fetch tutors
            Tutors = await PaginatedList<Tutor>.CreateAsync(tutorsIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
        }
    }
}
