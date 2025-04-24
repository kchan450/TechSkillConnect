using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechSkillConnect.Data;
using TechSkillConnect.Models;

namespace TechSkillConnect.Pages.Admin.Learners
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Learner Learner { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // ✅ Ensure registration date is set automatically
            Learner.Learner_registration_date = DateTime.UtcNow;

            await _context.Learners.AddAsync(Learner);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}