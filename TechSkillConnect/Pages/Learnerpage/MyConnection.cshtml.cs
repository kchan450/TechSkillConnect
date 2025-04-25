using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using TechSkillConnect.Data;
using TechSkillConnect.Models;

namespace TechSkillConnect.Pages.Learnerpage
{
    [Authorize(Roles = "Learner")]
    public class MyConnectionModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public MyConnectionModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Connection> Connections { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var learner = await _context.Learners.FirstOrDefaultAsync(l => l.UserID == userId);

            if (learner == null)
            {
                // Create a new Learner record for the user
                learner = new Learner
                {
                    UserID = userId,
                    Learner_firstname = "Unknown", // Default value
                    Learner_lastname = "Unknown", // Default value
                    LearnerEmail = "unknown@example.com", // Default value (must be a valid email)
                    CountryOfBirth = "Not Specified" // Default value
                    // Learner_registration_date is already set to DateTime.UtcNow by default
                };
                _context.Learners.Add(learner);
                await _context.SaveChangesAsync();
            }

            Connections = await _context.Connections
                .Where(c => c.LearnerID == learner.LearnerID)
                .Include(c => c.Tutor)
                .Include(c => c.Tutor.TutorProfile)
                .ToListAsync();

            return Page();
        }
    }
}
