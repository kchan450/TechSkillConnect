using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechSkillConnect.Data;
using TechSkillConnect.Models;

namespace TechSkillConnect.Pages.Admin.Tutors
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Tutor Tutor { get; set; } = new Tutor();

        public SelectList CountryOptions { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // ✅ Load distinct country options for dropdown
            CountryOptions = new SelectList(await _context.Tutors
                .Select(t => t.CountryOfBirth)
                .Distinct()
                .ToListAsync());

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // Reload dropdowns in case of validation errors
                    CountryOptions = new SelectList(await _context.Tutors
                        .Select(t => t.CountryOfBirth)
                        .Distinct()
                        .ToListAsync());

                    return Page(); // 🚀 If validation fails, stay on the form
                }

                // ✅ Ensure registration date is set automatically
                Tutor.Tutor_registration_date = DateTime.UtcNow;
                Tutor.IdentityID = Tutor.IdentityID; //sherry

                _context.Tutors.Add(Tutor);
                await _context.SaveChangesAsync();

                Console.WriteLine("🎉 Tutor record successfully created!");

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error saving tutor: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while saving the tutor record.");

                return Page();
            }
        }
    }
}
