using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TechSkillConnect.Models;
using TechSkillConnect.Data;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace TechSkillConnect.Pages.Tutorpage
{
    public class TutorCreatingProfileConfirmModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public TutorProfile TutorProfile { get; set; } = new TutorProfile();

        [BindProperty]
        public Tutor Tutor { get; set; } = new Tutor();

        public TutorCreatingProfileConfirmModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            string tutorData = TempData["Tutor"] as string;
            string tutorProfileData = TempData["TutorProfile"] as string;

            this.Tutor = JsonConvert.DeserializeObject<Tutor>(tutorData) ?? new Tutor();
            this.TutorProfile = JsonConvert.DeserializeObject<TutorProfile>(tutorProfileData) ?? new TutorProfile();
            this.TutorProfile.Tutor = this.Tutor;

            TempData.Keep("Tutor");
            TempData.Keep("TutorProfile");

            return Page();
        }

        public async Task<IActionResult> OnPostConfirmAsync()
        {
            string currentUserID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string currentUserEmail = User.FindFirstValue(ClaimTypes.Email);

            string tutorData = TempData.Peek("Tutor") as string;
            string tutorProfileData = TempData.Peek("TutorProfile") as string;

            if (string.IsNullOrEmpty(tutorData) || string.IsNullOrEmpty(tutorProfileData))
            {
                Console.WriteLine("TempData for Tutor or TutorProfile is missing.");
                ModelState.AddModelError("", "Unable to find the tutor data. Please try again.");
                return Page();
            }

            try
            {
                this.Tutor = JsonConvert.DeserializeObject<Tutor>(tutorData) ?? new Tutor();
                this.TutorProfile = JsonConvert.DeserializeObject<TutorProfile>(tutorProfileData) ?? new TutorProfile();

                var tutor = await _context.Tutors.FirstOrDefaultAsync(t => t.IdentityID == currentUserID);

                if (tutor == null)
                {
                    tutor = new Tutor
                    {
                        IdentityID = currentUserID,
                        Tutor_firstname = this.Tutor.Tutor_firstname,
                        Tutor_lastname = this.Tutor.Tutor_lastname,
                        CountryOfBirth = this.Tutor.CountryOfBirth,
                        Tutor_phone = this.Tutor.Tutor_phone,
                        TutorEmail = currentUserEmail,
                        Tutor_registration_date = DateTime.UtcNow
                    };
                    _context.Tutors.Add(tutor);
                }
                else
                {
                    tutor.Tutor_firstname = this.Tutor.Tutor_firstname;
                    tutor.Tutor_lastname = this.Tutor.Tutor_lastname;
                    tutor.CountryOfBirth = this.Tutor.CountryOfBirth;
                    tutor.Tutor_phone = this.Tutor.Tutor_phone;
                    tutor.TutorEmail = currentUserEmail;
                }

                await _context.SaveChangesAsync();

                var profile = await _context.TutorProfiles.FirstOrDefaultAsync(p => p.TutorID == tutor.TutorID);

                if (profile == null)
                {
                    profile = new TutorProfile
                    {
                        TutorID = tutor.TutorID,
                        Language = this.TutorProfile.Language,
                        YearsOfExperience = this.TutorProfile.YearsOfExperience,
                        SkillLevel = this.TutorProfile.SkillLevel,
                        FeePerSession = this.TutorProfile.FeePerSession,
                        SelfIntro = this.TutorProfile.SelfIntro,
                        SelfHeadline = this.TutorProfile.SelfHeadline
                    };
                    _context.TutorProfiles.Add(profile);
                }
                else
                {
                    profile.Language = this.TutorProfile.Language;
                    profile.YearsOfExperience = this.TutorProfile.YearsOfExperience;
                    profile.SkillLevel = this.TutorProfile.SkillLevel;
                    profile.FeePerSession = this.TutorProfile.FeePerSession;
                    profile.SelfIntro = this.TutorProfile.SelfIntro;
                    profile.SelfHeadline = this.TutorProfile.SelfHeadline;
                }

                await _context.SaveChangesAsync();

                var onboarding = await _context.TutorOnboardings.FirstOrDefaultAsync(o => o.UserId == currentUserID);
                if (onboarding != null)
                {
                    onboarding.IsProfileComplete = true;
                    onboarding.ProfileCompletedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }

                TempData["Tutor"] = JsonConvert.SerializeObject(tutor);
                TempData["TutorProfile"] = JsonConvert.SerializeObject(profile);

                // ✅ Correct Redirect: only page name
                return RedirectToPage("tutor_dashboard");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                ModelState.AddModelError("", "An error occurred while processing your request. Please try again.");
                return Page();
            }
        }
    }
}
