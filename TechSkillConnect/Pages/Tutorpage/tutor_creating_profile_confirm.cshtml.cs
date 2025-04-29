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
        public Tutor Tutor { get; set; } = new Tutor(); // Ensure instance exists

        public TutorCreatingProfileConfirmModel(ApplicationDbContext context)
        {
            _context = context;
        }


        public IActionResult OnGet()
        {
            string tutorData = TempData["Tutor"] as string;
            string tutorProfileData = TempData["TutorProfile"] as string;

            if (string.IsNullOrEmpty(tutorData) || string.IsNullOrEmpty(tutorProfileData))
            {
                Console.WriteLine("TempData missing at OnGet.");
                return RedirectToPage("/Tutorpage/tutor_creating_profile_combined"); // Or an error page
            }


            this.Tutor = JsonConvert.DeserializeObject<Tutor>(tutorData) ?? new Tutor();
            this.TutorProfile = JsonConvert.DeserializeObject<TutorProfile>(tutorProfileData) ?? new TutorProfile();
            this.TutorProfile.Tutor = this.Tutor;

            TempData.Keep("Tutor");
            TempData.Keep("TutorProfile");

            return Page();
        }

        //public IActionResult OnGet()
        //{
        //    string tutorData = TempData["Tutor"] as string; // Match key from previous step
        //    string tutorProfileData = TempData["TutorProfile"] as string;

        //    // Check if TempData is null or empty
        //    if (string.IsNullOrEmpty(tutorData) || string.IsNullOrEmpty(tutorProfileData))
        //    {
        //        // Log the issue
        //        Console.WriteLine("TempData for Tutor or TutorProfile is null or empty.");
        //        // Return an error page or redirect
        //        return RedirectToPage("/ErrorPage"); // Or any other page to handle this case
        //    }

        //    // Deserialize data from TempData
        //    this.Tutor = JsonConvert.DeserializeObject<Tutor>(tutorData) ?? new Tutor();
        //    this.TutorProfile = JsonConvert.DeserializeObject<TutorProfile>(tutorProfileData) ?? new TutorProfile();
        //    this.TutorProfile.Tutor = this.Tutor; // Assign Tutor to TutorProfile

        //    // Keep TempData for future use
        //    TempData.Keep("Tutor");
        //    TempData.Keep("TutorProfile");

        //    return Page();
        //}

        public async Task<IActionResult> OnPostConfirmAsync()
        {
            ModelState.Clear();  // Clear validation state

            Console.WriteLine("OnPostConfirmAsync started...");

            // 🔥 Recover TempData during POST (very important)
            string tutorData = TempData["Tutor"] as string;
            string tutorProfileData = TempData["TutorProfile"] as string;

            if (!string.IsNullOrEmpty(tutorData))
            {
                this.Tutor = JsonConvert.DeserializeObject<Tutor>(tutorData) ?? new Tutor();
            }
            if (!string.IsNullOrEmpty(tutorProfileData))
            {
                this.TutorProfile = JsonConvert.DeserializeObject<TutorProfile>(tutorProfileData) ?? new TutorProfile();
            }

            string currentUserID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string currentUserEmail = User.FindFirstValue(ClaimTypes.Email);

            try
            {
                // 🔵 Step 1: Find if Tutor already exists
                var tutor = await _context.Tutors.FirstOrDefaultAsync(t => t.IdentityID == currentUserID);

                if (tutor == null)
                {
                    // 🔵 Step 2: Create new Tutor (DO NOT set TutorID manually)
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
                    await _context.SaveChangesAsync(); // 🔥 Save Tutor first so that TutorID is generated
                }
                else
                {
                    // 🔵 Step 3: Update existing Tutor
                    tutor.Tutor_firstname = this.Tutor.Tutor_firstname;
                    tutor.Tutor_lastname = this.Tutor.Tutor_lastname;
                    tutor.CountryOfBirth = this.Tutor.CountryOfBirth;
                    tutor.Tutor_phone = this.Tutor.Tutor_phone;
                    tutor.TutorEmail = currentUserEmail;

                    await _context.SaveChangesAsync(); // Save updates
                }

                Console.WriteLine($"Tutor saved. TutorID = {tutor?.TutorID}");

                // 🔵 Step 4: Save TutorProfile (using correct TutorID)
                var profile = await _context.TutorProfiles.FirstOrDefaultAsync(p => p.TutorID == tutor.TutorID);

                if (profile == null)
                {
                    profile = new TutorProfile
                    {
                        TutorID = tutor.TutorID,  // ✅ Correct real TutorID from DB
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

                await _context.SaveChangesAsync(); // Save TutorProfile

                // 🔵 Step 5: Update Onboarding
                var onboarding = await _context.TutorOnboardings.FirstOrDefaultAsync(o => o.UserId == currentUserID);

                if (onboarding != null)
                {
                    onboarding.IsProfileComplete = true;
                    onboarding.ProfileCompletedAt = DateTime.UtcNow;

                    await _context.SaveChangesAsync(); // Save onboarding update
                }

                // 🔵 Step 6: Redirect to Thank You page
                Console.WriteLine("Saving success, redirecting to Thank You page...");
                return RedirectToPage("/Tutorpage/ThankYouForRegistration");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                ModelState.AddModelError("", "An error occurred while processing your request. Please try again.");
                return Page(); // Stay on same page if error happens
            }
        }




    }
}
