using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using TechSkillConnect.Data;
using TechSkillConnect.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace TechSkillConnect.Pages.Tutorpage
{
    public class TutorCreatingProfileModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public TutorCreatingProfileModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public TutorProfile TutorProfile { get; set; } = new TutorProfile();

        [BindProperty]
        public Tutor Tutor { get; set; } = new Tutor();

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Fetch TutorOnboarding data for the logged-in user
            var onboarding = await _context.TutorOnboardings
                .FirstOrDefaultAsync(o => o.UserId == userId);

            // If onboarding data exists, populate the Tutor model with that data
            if (onboarding != null)
            {
                Tutor.Tutor_firstname = onboarding.FirstName;
                Tutor.Tutor_lastname = onboarding.LastName;
                Tutor.CountryOfBirth = onboarding.CountryOfBirth;
                Tutor.Tutor_phone = onboarding.TutorPhone;
            }

            // Optionally, you can also load the TutorProfile if it exists for the current tutor
            var profile = await _context.TutorProfiles
                .FirstOrDefaultAsync(p => p.TutorID == Tutor.TutorID);
            if (profile != null)
            {
                TutorProfile = profile;
            }

            // Store Tutor and TutorProfile in TempData for future steps
            TempData["Tutor"] = JsonConvert.SerializeObject(Tutor);
            TempData["TutorProfile"] = JsonConvert.SerializeObject(TutorProfile);
            TempData.Keep("Tutor");
            TempData.Keep("TutorProfile");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            string currentUserID = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get logged-in user's ID

            // Fetch or create a TutorOnboarding record for the current user
            var onboarding = await _context.TutorOnboardings
                .FirstOrDefaultAsync(x => x.UserId == currentUserID);

            if (onboarding == null)
            {
                onboarding = new TutorOnboarding
                {
                    UserId = currentUserID, // Ensure the UserId is set from the logged-in user
                    CreatedAt = DateTime.UtcNow
                };
                _context.TutorOnboardings.Add(onboarding);
            }

            // Save form data into the TutorOnboarding record
            onboarding.FirstName = Tutor.Tutor_firstname;
            onboarding.LastName = Tutor.Tutor_lastname;
            onboarding.CountryOfBirth = Tutor.CountryOfBirth;
            onboarding.TutorPhone = Tutor.Tutor_phone;

            // Save the updated onboarding data
            await _context.SaveChangesAsync();

            // Now create/update the TutorProfile if necessary
            var tutorProfile = await _context.TutorProfiles
                .FirstOrDefaultAsync(p => p.TutorID == Tutor.TutorID);

            if (tutorProfile == null)
            {
                tutorProfile = new TutorProfile
                {
                    TutorID = Tutor.TutorID,
                    Language = TutorProfile.Language, // Use the value from the form
                    YearsOfExperience = TutorProfile.YearsOfExperience, // Use the value from the form
                    SkillLevel = TutorProfile.SkillLevel, // Use the value from the form
                    FeePerSession = TutorProfile.FeePerSession, // Use the value from the form
                    SelfIntro = TutorProfile.SelfIntro, // Use the value from the form
                    SelfHeadline = TutorProfile.SelfHeadline // Use the value from the form
                };

                _context.TutorProfiles.Add(tutorProfile);
                await _context.SaveChangesAsync();
            }

            // Save the Tutor and TutorProfile to TempData for the next page
            TempData["Tutor"] = JsonConvert.SerializeObject(Tutor);
            TempData["TutorProfile"] = JsonConvert.SerializeObject(tutorProfile);
            TempData.Keep("Tutor");
            TempData.Keep("TutorProfile");

            // Redirect to the next page in the profile creation process
            return RedirectToPage("/Tutorpage/tutor_creating_profile");
        }
    }
}
