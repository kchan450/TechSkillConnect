using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechSkillConnect.Data;
using TechSkillConnect.Models;

namespace TechSkillConnect.Pages.Admin.TutorProfiles
{
    public class EditModel : PageModel
    {
        private readonly TechSkillConnect.Data.ApplicationDbContext _context;

        public EditModel(TechSkillConnect.Data.ApplicationDbContext context)
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

            var tutorprofile =  await _context.TutorProfiles.FirstOrDefaultAsync(m => m.ProfileID == id);
            if (tutorprofile == null)
            {
                return NotFound();
            }
            TutorProfile = tutorprofile;
           ViewData["TutorID"] = new SelectList(_context.Tutors, "TutorID", "TutorEmail");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(TutorProfile).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TutorProfileExists(TutorProfile.ProfileID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool TutorProfileExists(int id)
        {
            return _context.TutorProfiles.Any(e => e.ProfileID == id);
        }
    }
}
