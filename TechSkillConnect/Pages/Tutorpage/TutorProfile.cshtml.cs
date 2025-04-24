using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using TechSkillConnect.Data;
using TechSkillConnect.Models;

namespace TechSkillConnect.Pages.Tutorpage
{
    public class TutorProfileModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public Tutor Tutor { get; set; }

        [BindProperty]
        public TutorProfile TutorProfile { get; set; }

        public TutorProfileModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Get the Tutor record based on IdentityID
            Tutor = await _context.Tutors.FirstOrDefaultAsync(t => t.IdentityID == userId);

            if (Tutor == null)
            {
                return RedirectToPage("/Tutorpage/TutorCreatingProfile");
            }

            TutorProfile = await _context.TutorProfiles.FirstOrDefaultAsync(p => p.TutorID == Tutor.TutorID);

            if (TutorProfile == null)
            {
                TutorProfile = new TutorProfile
                {
                    TutorID = Tutor.TutorID
                };
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ModelState.Remove("Tutor.IdentityID");
            ModelState.Remove("Tutor.TutorProfile");
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var tutor = await _context.Tutors.FirstOrDefaultAsync(t => t.IdentityID == userId);

            if (tutor == null)
            {
                return RedirectToPage("/Tutorpage/TutorCreatingProfile");
            }

            tutor.Tutor_firstname = Tutor.Tutor_firstname;
            tutor.Tutor_lastname = Tutor.Tutor_lastname;
            tutor.TutorEmail = Tutor.TutorEmail;
            tutor.Tutor_phone = Tutor.Tutor_phone;
            tutor.CountryOfBirth = Tutor.CountryOfBirth;

            var profile = await _context.TutorProfiles.FirstOrDefaultAsync(p => p.TutorID == tutor.TutorID);

            if (profile == null)
            {
                TutorProfile.TutorID = tutor.TutorID;
                _context.TutorProfiles.Add(TutorProfile);
            }
            else
            {
                profile.Language = TutorProfile.Language;
                profile.YearsOfExperience = TutorProfile.YearsOfExperience;
                profile.SkillLevel = TutorProfile.SkillLevel;
                profile.FeePerSession = TutorProfile.FeePerSession;
                profile.SelfIntro = TutorProfile.SelfIntro;
                profile.SelfHeadline = TutorProfile.SelfHeadline;
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("/Tutorpage/tutor_dashboard");
        }
    }
}
