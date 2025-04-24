using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using TechSkillConnect.Data;
using TechSkillConnect.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace TechSkillConnect.Pages.Tutorpage
{
    public class TutorCreatingProfilePIModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public TutorCreatingProfilePIModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Tutor Tutor { get; set; } = new Tutor();

        [BindProperty]
        public TutorProfile TutorProfile { get; set; } = new TutorProfile();

        public async Task<IActionResult> OnGetAsync()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Fetch TutorOnboarding data for the logged-in user
            var onboarding = await _context.TutorOnboardings.FirstOrDefaultAsync(o => o.UserId == currentUserId);

            if (onboarding != null)
            {
                // Populate the Tutor model with data from the TutorOnboarding
                Tutor.Tutor_firstname = onboarding.FirstName;
                Tutor.Tutor_lastname = onboarding.LastName;
                Tutor.CountryOfBirth = onboarding.CountryOfBirth;
                Tutor.Tutor_phone = onboarding.TutorPhone;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Fetch or create a TutorOnboarding record for the current user
            var onboarding = await _context.TutorOnboardings.FirstOrDefaultAsync(x => x.UserId == currentUserId);

            if (onboarding == null)
            {
                onboarding = new TutorOnboarding
                {
                    UserId = currentUserId, // Ensure the UserId is set from the logged-in user
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
            var tutorProfile = await _context.TutorProfiles.FirstOrDefaultAsync(p => p.TutorID == Tutor.TutorID);

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
