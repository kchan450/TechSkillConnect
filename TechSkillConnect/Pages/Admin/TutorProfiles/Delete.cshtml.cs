using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TechSkillConnect.Data;
using TechSkillConnect.Models;

namespace TechSkillConnect.Pages.Admin.TutorProfiles
{
    public class DeleteModel : PageModel
    {
        private readonly TechSkillConnect.Data.ApplicationDbContext _context;

        public DeleteModel(TechSkillConnect.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public TutorProfile TutorProfile { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tutorprofile = await _context.TutorProfiles.FirstOrDefaultAsync(m => m.ProfileID == id);

            if (tutorprofile == null)
            {
                return NotFound();
            }
            else
            {
                TutorProfile = tutorprofile;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tutorprofile = await _context.TutorProfiles.FindAsync(id);
            if (tutorprofile != null)
            {
                TutorProfile = tutorprofile;
                _context.TutorProfiles.Remove(TutorProfile);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
