using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TechSkillConnect.Data;
using TechSkillConnect.Models;

namespace TechSkillConnect.Pages.Learnerpage
{

    [Authorize(Roles = "Learner")]
    public class LearnerProfilePageModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public LearnerProfilePageModel(ApplicationDbContext context)
        {
            _context = context;
        }
        [BindProperty]
        public Learner Learner { get; set; }

        [BindProperty]
        public LearnerProfileViewModel LearnerProfile { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Ensure the user has a Tutor record
            Learner = await _context.Learners.FirstOrDefaultAsync(t => t.UserID == userId);

            if (Learner == null)
            {
                // If no tutor record, redirect to the profile creation page
                return RedirectToPage("/Learnerpage/LearnerProfileCreate");
            }

            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ModelState.Remove("UserID");
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Check if the TutorProfile exists or if it's a new profile
            var learnerInfo = await _context.Learners.FirstOrDefaultAsync(p => p.UserID == userId);

            if (learnerInfo != null)
            {
                // Update the existing profile
                learnerInfo.Learner_firstname = Learner.Learner_firstname;
                learnerInfo.Learner_lastname = Learner.Learner_lastname;
                learnerInfo.CountryOfBirth = Learner.CountryOfBirth;

            }

            // Save the changes to the database
            await _context.SaveChangesAsync();

            // Redirect to the Tutor Profile page after successful update
            return RedirectToPage("/Learnerpage/learner_dashboard");
        }

    }
}
