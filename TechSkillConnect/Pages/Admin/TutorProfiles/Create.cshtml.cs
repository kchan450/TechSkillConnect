using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechSkillConnect.Data;
using TechSkillConnect.Models;

namespace TechSkillConnect.Pages.Admin.TutorProfiles
{
    public class CreateModel : PageModel
    {
        private readonly TechSkillConnect.Data.ApplicationDbContext _context;

        public CreateModel(TechSkillConnect.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["TutorID"] = new SelectList(_context.Tutors, "TutorID", "TutorEmail");
            return Page();
        }

        [BindProperty]
        public TutorProfile TutorProfile { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.TutorProfiles.Add(TutorProfile);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
