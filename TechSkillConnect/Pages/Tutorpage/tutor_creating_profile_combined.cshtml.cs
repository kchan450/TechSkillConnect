using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using TechSkillConnect.Data;
using TechSkillConnect.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace TechSkillConnect.Pages.Tutorpage
{
    public class TutorCreatingProfileCombinedModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public TutorCreatingProfileCombinedModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public TutorProfile TutorProfile { get; set; } = new TutorProfile();

        [BindProperty]
        public Tutor Tutor { get; set; } = new Tutor();

        public async Task<IActionResult> OnGetAsync()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get logged-in user's ID

            // Fetch Tutor data from TutorOnboarding if the tutor is not yet created
            var onboarding = await _context.TutorOnboardings.FirstOrDefaultAsync(o => o.UserId == userId);

            if (onboarding != null)
            {
                Tutor.Tutor_firstname = onboarding.FirstName;
                Tutor.Tutor_lastname = onboarding.LastName;
                Tutor.CountryOfBirth = onboarding.CountryOfBirth;
                Tutor.Tutor_phone = onboarding.TutorPhone;
            }

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            string currentUserID = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get logged-in user's ID

            // Get the logged-in user's email
            string currentUserEmail = User.FindFirstValue(ClaimTypes.Email);

            // Create or update TutorOnboarding record in TempData without saving to database
            var onboarding = new TutorOnboarding
            {
                UserId = currentUserID, // Ensure the UserId is set from the logged-in user
                CreatedAt = DateTime.UtcNow,
                FirstName = Tutor.Tutor_firstname,
                LastName = Tutor.Tutor_lastname,
                CountryOfBirth = Tutor.CountryOfBirth,
                TutorPhone = Tutor.Tutor_phone
            };

            // Save onboarding data in TempData for later use
            TempData["Onboarding"] = JsonConvert.SerializeObject(onboarding);

            // Create or update Tutor in TempData without saving to database
            var tutor = new Tutor
            {
                IdentityID = currentUserID,
                Tutor_firstname = Tutor.Tutor_firstname,
                Tutor_lastname = Tutor.Tutor_lastname,
                CountryOfBirth = Tutor.CountryOfBirth,
                Tutor_phone = Tutor.Tutor_phone,
                TutorEmail = currentUserEmail, // Use the logged-in user's email as TutorEmail
                Tutor_registration_date = DateTime.UtcNow
            };

            // Save tutor data in TempData for later use
            TempData["Tutor"] = JsonConvert.SerializeObject(tutor);

            // Create or update TutorProfile in TempData without saving to database
            var profile = new TutorProfile
            {
                TutorID = tutor.TutorID,
                Language = TutorProfile.Language,
                YearsOfExperience = TutorProfile.YearsOfExperience,
                SkillLevel = TutorProfile.SkillLevel,
                FeePerSession = TutorProfile.FeePerSession,
                SelfIntro = TutorProfile.SelfIntro,
                SelfHeadline = TutorProfile.SelfHeadline
            };

            // Save profile data in TempData for later use
            TempData["TutorProfile"] = JsonConvert.SerializeObject(profile);

            // Optionally store some data in TempData for later use
            TempData["TutorFirstName"] = Tutor.Tutor_firstname;
            TempData["TutorLastName"] = Tutor.Tutor_lastname;

            // Redirect to the next step (profile confirmation page)
            return RedirectToPage("/Tutorpage/tutor_creating_profile_confirm");
        }

        //public async Task<IActionResult> OnPostAsync()
        //{
        //    string currentUserID = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get logged-in user's ID

        //    // Get the logged-in user's email
        //    string currentUserEmail = User.FindFirstValue(ClaimTypes.Email);

        //    // Fetch or create a TutorOnboarding record for the current user
        //    var onboarding = await _context.TutorOnboardings
        //        .FirstOrDefaultAsync(x => x.UserId == currentUserID);

        //    if (onboarding == null)
        //    {
        //        onboarding = new TutorOnboarding
        //        {
        //            UserId = currentUserID, // Ensure the UserId is set from the logged-in user
        //            CreatedAt = DateTime.UtcNow
        //        };
        //        _context.TutorOnboardings.Add(onboarding);
        //    }

        //    // Save form data into the TutorOnboarding record
        //    onboarding.FirstName = Tutor.Tutor_firstname;
        //    onboarding.LastName = Tutor.Tutor_lastname;
        //    onboarding.CountryOfBirth = Tutor.CountryOfBirth;
        //    onboarding.TutorPhone = Tutor.Tutor_phone;

        //    // Save the updated onboarding data
        //    await _context.SaveChangesAsync();

        //    // Create or update Tutor record
        //    var tutor = await _context.Tutors.FirstOrDefaultAsync(t => t.IdentityID == currentUserID);

        //    if (tutor == null)
        //    {
        //        tutor = new Tutor
        //        {
        //            IdentityID = currentUserID,
        //            Tutor_firstname = Tutor.Tutor_firstname,
        //            Tutor_lastname = Tutor.Tutor_lastname,
        //            CountryOfBirth = Tutor.CountryOfBirth,
        //            Tutor_phone = Tutor.Tutor_phone,
        //            TutorEmail = currentUserEmail, // Use the logged-in user's email as TutorEmail
        //            Tutor_registration_date = DateTime.UtcNow
        //        };
        //        _context.Tutors.Add(tutor);
        //    }
        //    else
        //    {
        //        tutor.Tutor_firstname = Tutor.Tutor_firstname;
        //        tutor.Tutor_lastname = Tutor.Tutor_lastname;
        //        tutor.CountryOfBirth = Tutor.CountryOfBirth;
        //        tutor.Tutor_phone = Tutor.Tutor_phone;
        //        tutor.TutorEmail = currentUserEmail; // Ensure email is updated with logged-in user's email
        //    }

        //    // Save the Tutor data
        //    await _context.SaveChangesAsync();

        //    // Now handle TutorProfile
        //    var profile = await _context.TutorProfiles.FirstOrDefaultAsync(p => p.TutorID == tutor.TutorID);

        //    if (profile == null)
        //    {
        //        profile = new TutorProfile
        //        {
        //            TutorID = tutor.TutorID,
        //            Language = TutorProfile.Language,
        //            YearsOfExperience = TutorProfile.YearsOfExperience,
        //            SkillLevel = TutorProfile.SkillLevel,
        //            FeePerSession = TutorProfile.FeePerSession,
        //            SelfIntro = TutorProfile.SelfIntro,
        //            SelfHeadline = TutorProfile.SelfHeadline
        //        };
        //        _context.TutorProfiles.Add(profile);
        //    }
        //    else
        //    {
        //        profile.Language = TutorProfile.Language;
        //        profile.YearsOfExperience = TutorProfile.YearsOfExperience;
        //        profile.SkillLevel = TutorProfile.SkillLevel;
        //        profile.FeePerSession = TutorProfile.FeePerSession;
        //        profile.SelfIntro = TutorProfile.SelfIntro;
        //        profile.SelfHeadline = TutorProfile.SelfHeadline;
        //    }

        //    // Save the TutorProfile data
        //    await _context.SaveChangesAsync();

        //    // Save both Tutor and TutorProfile in TempData
        //    TempData["Tutor"] = JsonConvert.SerializeObject(Tutor);
        //    TempData["TutorProfile"] = JsonConvert.SerializeObject(TutorProfile);

        //    // Optionally store some data in TempData for later use
        //    TempData["TutorFirstName"] = Tutor.Tutor_firstname;
        //    TempData["TutorLastName"] = Tutor.Tutor_lastname;

        //    // Redirect to the next step
        //    return RedirectToPage("/Tutorpage/tutor_creating_profile_confirm");
        //}


    }
}
