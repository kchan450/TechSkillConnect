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

            Console.WriteLine("OnGet executed successfully.");
            return Page();
        }

        public async Task<IActionResult> OnPostConfirmAsync()
        {
            Console.WriteLine("OnPostConfirmAsync handler invoked...");

            string currentUserID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string currentUserEmail = User.FindFirstValue(ClaimTypes.Email);

            Console.WriteLine($"User authenticated: {User.Identity.IsAuthenticated}");
            Console.WriteLine($"CurrentUserID: {currentUserID ?? "null"}");
            Console.WriteLine($"CurrentUserEmail: {currentUserEmail ?? "null"}");

            if (!User.Identity.IsAuthenticated || string.IsNullOrEmpty(currentUserID))
            {
                Console.WriteLine("User is not authenticated. Redirecting to login...");
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            string tutorData = TempData.Peek("Tutor") as string;
            string tutorProfileData = TempData.Peek("TutorProfile") as string;

            if (string.IsNullOrEmpty(tutorData) || string.IsNullOrEmpty(tutorProfileData))
            {
                Console.WriteLine("Tutor or TutorProfile data is missing in TempData.");
                ModelState.AddModelError("", "Unable to find the tutor data. Please try again.");
                return Page();
            }

            try
            {
                Console.WriteLine("Deserializing Tutor and TutorProfile data...");
                this.Tutor = JsonConvert.DeserializeObject<Tutor>(tutorData) ?? new Tutor();
                this.TutorProfile = JsonConvert.DeserializeObject<TutorProfile>(tutorProfileData) ?? new TutorProfile();

                Console.WriteLine("Looking up tutor in the database...");
                var tutor = await _context.Tutors.FirstOrDefaultAsync(t => t.IdentityID == currentUserID);

                if (tutor == null)
                {
                    Console.WriteLine("Tutor not found. Creating new tutor...");
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
                    Console.WriteLine("Tutor found. Updating tutor details...");
                    tutor.Tutor_firstname = this.Tutor.Tutor_firstname;
                    tutor.Tutor_lastname = this.Tutor.Tutor_lastname;
                    tutor.CountryOfBirth = this.Tutor.CountryOfBirth;
                    tutor.Tutor_phone = this.Tutor.Tutor_phone;
                    tutor.TutorEmail = currentUserEmail;
                }

                Console.WriteLine("Saving tutor changes to the database...");
                await _context.SaveChangesAsync();
                Console.WriteLine("Tutor changes saved successfully.");
                Console.WriteLine($"TutorID after save: {tutor.TutorID}");

                Console.WriteLine("Looking up tutor profile in the database...");
                var profile = await _context.TutorProfiles.FirstOrDefaultAsync(p => p.TutorID == tutor.TutorID);

                if (profile == null)
                {
                    Console.WriteLine("Tutor profile not found. Creating new profile...");
                    profile = new TutorProfile
                    {
                        TutorID = tutor.TutorID,
                        Language = this.TutorProfile.Language,
                        YearsOfExperience = this.TutorProfile.YearsOfExperience,
                        SkillLevel = this.TutorProfile.SkillLevel,
                        FeePerSession = this.TutorProfile.FeePerSession,
                        SelfIntro = this.TutorProfile.SelfIntro,
                        SelfHeadline = this.TutorProfile.SelfHeadline,
                        Certificate = this.TutorProfile.Certificate
                    };
                    _context.TutorProfiles.Add(profile);
                }
                else
                {
                    Console.WriteLine("Tutor profile found. Updating profile details...");
                    profile.Language = this.TutorProfile.Language;
                    profile.YearsOfExperience = this.TutorProfile.YearsOfExperience;
                    profile.SkillLevel = this.TutorProfile.SkillLevel;
                    profile.FeePerSession = this.TutorProfile.FeePerSession;
                    profile.SelfIntro = this.TutorProfile.SelfIntro;
                    profile.SelfHeadline = this.TutorProfile.SelfHeadline;
                    profile.Certificate = this.TutorProfile.Certificate;
                }

                Console.WriteLine("Saving tutor profile changes to the database...");
                await _context.SaveChangesAsync();
                Console.WriteLine("Tutor profile changes saved successfully.");
                Console.WriteLine($"ProfileID after save: {profile.ProfileID}");

                Console.WriteLine("Clearing TempData...");
                TempData.Remove("Tutor");
                TempData.Remove("TutorProfile");

                if (!ModelState.IsValid)
                {
                    Console.WriteLine("ModelState is invalid. Returning to the same page.");
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        Console.WriteLine($"ModelState Error: {error.ErrorMessage}");
                    }
                    return Page();
                }

                Console.WriteLine("Attempting redirect to tutor_dashboard...");
                string redirectUrl = "https://techskillconnect-dv4tahcghugwhf6g.canadacentral-01.azurewebsites.net/Tutorpage/tutor_dashboard";
                var redirectResult = Redirect(redirectUrl);
                Console.WriteLine($"Redirect result created: {redirectResult?.GetType().Name}");
                return redirectResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                ModelState.AddModelError("", $"An error occurred while saving your profile: {ex.Message}");
                return Page();
            }
        }
    }
}