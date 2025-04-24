using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TechSkillConnect.Data;
using TechSkillConnect.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace TechSkillConnect.Pages.Tutorpage
{
    public class MyStudentModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public MyStudentModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Connection> Connections { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tutor = await _context.Tutors.FirstOrDefaultAsync(t => t.IdentityID == userId);

            if (tutor == null)
                return Unauthorized();

            Connections = await _context.Connections
                .Where(c => c.TutorID == tutor.TutorID)
                .Include(c => c.Learner)
                .ToListAsync();

            return Page();
        }
    }
}
