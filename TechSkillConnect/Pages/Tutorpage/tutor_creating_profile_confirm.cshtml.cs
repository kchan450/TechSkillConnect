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
            string currentUserID = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get logged-in user's ID
            string currentUserEmail = User.FindFirstValue(ClaimTypes.Email); // Get the logged-in user's email

            // Retrieve the Tutor and TutorProfile data from TempData
            string tutorData = TempData.Peek("Tutor") as string;
            string tutorProfileData = TempData.Peek("TutorProfile") as string;

            // Check if TempData is null or empty
            if (string.IsNullOrEmpty(tutorData) || string.IsNullOrEmpty(tutorProfileData))
            {
                Console.WriteLine("TempData for Tutor or TutorProfile is null or empty during OnPostConfirmAsync.");
                ModelState.AddModelError("", "Unable to find the tutor data. Please try again.");
                return Page(); // Stay on the current page to show the error message
            }

            try
            {
                // Deserialize data from TempData
                this.Tutor = JsonConvert.DeserializeObject<Tutor>(tutorData) ?? new Tutor();
                this.TutorProfile = JsonConvert.DeserializeObject<TutorProfile>(tutorProfileData) ?? new TutorProfile();

                // Update Tutor data if necessary
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
                        TutorEmail = currentUserEmail, // Set the email of the logged-in user
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
                    tutor.TutorEmail = currentUserEmail; // Ensure email is updated with logged-in user's email
                }

                // Save the updated Tutor data
                await _context.SaveChangesAsync();

                // Now handle TutorProfile
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

                // Save the updated TutorProfile data
                await _context.SaveChangesAsync();

                // Update TutorOnboarding record to mark profile as complete
                var onboarding = await _context.TutorOnboardings.FirstOrDefaultAsync(o => o.UserId == currentUserID);
                if (onboarding != null)
                {
                    onboarding.IsProfileComplete = true; // Mark the profile as complete
                    onboarding.ProfileCompletedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }

                // Optionally store some data in TempData for later use
                TempData["Tutor"] = JsonConvert.SerializeObject(tutor);
                TempData["TutorProfile"] = JsonConvert.SerializeObject(profile);

                // Redirect to the confirmation page
                return RedirectToPage("/Tutorpage/tutor_creating_profile_first_payment_result");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                ModelState.AddModelError("", "An error occurred while processing your request. Please try again.");
                return Page(); // Stay on the current page to show the error message
            }
        }

    }
}
