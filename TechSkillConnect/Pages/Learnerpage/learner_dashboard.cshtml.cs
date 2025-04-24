using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TechSkillConnect.Data;

namespace TechSkillConnect.Pages.Learnerpage
{

    [Authorize(Roles = "Learner")]
    public class LearnerDashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public string ProfileStatus { get; set; }


        public LearnerDashboardModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get logged-in user's ID
            var learner = await _context.Learners.FirstOrDefaultAsync(l => l.UserID == userId);

            if (learner != null)
            {
                // Check if the profile is complete (you can add checks for specific fields if needed)
                ProfileStatus = string.IsNullOrEmpty(learner.CountryOfBirth) ? "Incomplete" : "Complete"; // Example check on CountryOfBirth field


            }
            else
            {
                // No learner found for the user, set status to Incomplete and Pending
                ProfileStatus = "Incomplete";

            }

            return Page();
        }
    }
}
