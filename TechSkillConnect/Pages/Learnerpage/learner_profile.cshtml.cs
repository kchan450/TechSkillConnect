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
    public class learner_profileModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public learner_profileModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Learner Learner { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            Learner = await _context.Learners.FirstOrDefaultAsync(t => t.UserID == userId);

            if (Learner == null)
            {
                return RedirectToPage("/Learnerpage/LearnerProfileCreate");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var learnerInDb = await _context.Learners.FirstOrDefaultAsync(p => p.UserID == userId);

            if (learnerInDb != null)
            {
                learnerInDb.Learner_firstname = Learner.Learner_firstname;
                learnerInDb.Learner_lastname = Learner.Learner_lastname;
                learnerInDb.LearnerEmail = Learner.LearnerEmail;
                learnerInDb.CountryOfBirth = Learner.CountryOfBirth;

                await _context.SaveChangesAsync();
            }

            return RedirectToPage("/Learnerpage/learner_dashboard");
        }
    }
}
