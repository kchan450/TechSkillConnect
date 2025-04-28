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
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public TutorProfile TutorProfile { get; set; } = default!;

        // 🔹 Add Language dropdown property
        public SelectList LanguageOptions { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // 🔹 Fetch the TutorProfile
            var tutorprofile = await _context.TutorProfiles
                .Include(tp => tp.Tutor)  // Optional: Include Tutor if needed
                .FirstOrDefaultAsync(m => m.ProfileID == id);

            if (tutorprofile == null)
            {
                return NotFound();
            }

            TutorProfile = tutorprofile;

            // 🔹 Populate TutorID dropdown (existing logic)
            ViewData["TutorID"] = new SelectList(_context.Tutors, "TutorID", "TutorEmail");

            // 🔹 Populate Language dropdown (hardcoded list)
            LanguageOptions = new SelectList(new List<string>
            {
                "English",
                "French",
                "Spanish",
                "Japanese",
                "Mandarin"
            });

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // 🔹 Reload dropdowns if validation fails
                ViewData["TutorID"] = new SelectList(_context.Tutors, "TutorID", "TutorEmail");

                LanguageOptions = new SelectList(new List<string>
                {
                    "English",
                    "French",
                    "Spanish",
                    "Japanese",
                    "Mandarin"
                });

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
