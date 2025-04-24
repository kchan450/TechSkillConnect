using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TechSkillConnect.Data;
using TechSkillConnect.Models;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace TechSkillConnect.Pages.LearnerPage
{
    public class LearnerProfileCreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        // Bind the form data to this model
        [BindProperty]
        public LearnerProfileCreateViewModel LearnerProfile { get; set; }

        public LearnerProfileCreateModel(ApplicationDbContext context)
        {
            _context = context;
            LearnerProfile = new LearnerProfileCreateViewModel(); // Initialize the model
        }

        public void OnGet()
        {
            // Optionally, you can pre-populate the form with data if needed
        }

        public async Task<IActionResult> OnPostAsync()
        {

            ModelState.Remove("LearnerProfile.LearnerEmail");

            if (ModelState.IsValid)
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get logged-in user's ID
                string userEmail = User.FindFirstValue(ClaimTypes.Email); // Get logged-in user's ID

                // Check if the profile already exists
                var existingProfile = await _context.Learners.FirstOrDefaultAsync(l => l.UserID == userId);
                if (existingProfile != null)
                {
                    // Redirect if the profile already exists
                    return RedirectToPage("/LearnerPage/LearnerDashboard");
                }

                // Create a new Learner object
                var learnerProfile = new Learner
                {
                    UserID = userId,
                    Learner_firstname = LearnerProfile.Learner_firstname,
                    Learner_lastname = LearnerProfile.Learner_lastname,
                    LearnerEmail = userEmail,
                    CountryOfBirth = LearnerProfile.CountryOfBirth,
                    Learner_registration_date = DateTime.UtcNow
                };

                // Add the learner profile to the database
                _context.Learners.Add(learnerProfile);
                await _context.SaveChangesAsync();

                // Redirect to the dashboard after profile creation
                return RedirectToPage("/Learnerpage/learner_dashboard");
            }

            // If the form is invalid, return the same page to show validation errors
            return Page();
        }
    }

    public class LearnerProfileCreateViewModel
    {
        public string Learner_firstname { get; set; }
        public string Learner_lastname { get; set; }
        public string LearnerEmail { get; set; }
        public string CountryOfBirth { get; set; }
    }
}
