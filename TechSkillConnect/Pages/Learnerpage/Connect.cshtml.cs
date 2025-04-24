using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using TechSkillConnect.Data;
using TechSkillConnect.Models;

namespace TechSkillConnect.Pages.Learnerpage
{
    public class ConnectModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ConnectModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Connection Connection { get; set; } = default!;

        [BindProperty]
        public Tutor Tutor { get; set; } = default!;
        [BindProperty]
        public TutorProfile TutorProfile { get; set; } = default!;
        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Tutor = await _context.Tutors.FirstOrDefaultAsync(m => m.TutorID == id);
            TutorProfile = await _context.TutorProfiles.FirstOrDefaultAsync(m => m.TutorID == id);

            if (Tutor == null || TutorProfile == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var learnerInfo = await _context.Learners.FirstOrDefaultAsync(p => p.UserID == userId);

                var existingConnection = await _context.Connections
                        .FirstOrDefaultAsync(c => c.LearnerID == learnerInfo.LearnerID && c.TutorID == id);

                if (existingConnection != null)
                {
                    ModelState.AddModelError("", "You have already connected with this tutor.");
                    return Page();
                }

                Connection.ConnectionDate = DateTime.UtcNow;
                Connection.TutorID = id;
                Connection.LearnerID = learnerInfo.LearnerID;
                
                // Add the Connections profile to the database
                _context.Connections.Add(Connection);
                await _context.SaveChangesAsync();
                
                Console.WriteLine("Connection created!");
                return RedirectToPage("./FindMyTutor");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while creating the connection.");
                return Page();
            }
        }
    }
}
