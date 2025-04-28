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

        public SelectList LanguageOptions { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tutorprofile = await _context.TutorProfiles
                .Include(tp => tp.Tutor)
                .FirstOrDefaultAsync(m => m.ProfileID == id);

            if (tutorprofile == null)
            {
                return NotFound();
            }

            TutorProfile = tutorprofile;

            ViewData["TutorID"] = new SelectList(_context.Tutors, "TutorID", "TutorEmail", TutorProfile.TutorID);
            LanguageOptions = GetLanguageOptions(TutorProfile.Language);  // Pass current value

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ViewData["TutorID"] = new SelectList(_context.Tutors, "TutorID", "TutorEmail", TutorProfile.TutorID);
                LanguageOptions = GetLanguageOptions(TutorProfile.Language);  // Ensure correct selection
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

        private SelectList GetLanguageOptions(string? selectedLanguage)
        {
            var languages = new List<string>
            {
                "Machine Learning",
                "Python",
                "Java",
                "AI",
                "Data Science"
            };

            // If the current value is missing, add it to the top
            if (!string.IsNullOrEmpty(selectedLanguage) && !languages.Contains(selectedLanguage))
            {
                languages.Insert(0, selectedLanguage);
            }

            return new SelectList(languages, selectedLanguage);
        }
    }
}
