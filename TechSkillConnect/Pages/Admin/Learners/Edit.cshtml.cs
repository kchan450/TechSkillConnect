using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TechSkillConnect.Data;
using TechSkillConnect.Models;

namespace TechSkillConnect.Pages.Admin.Learners
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Learner Learner { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Learner = await _context.Learners.FirstOrDefaultAsync(m => m.LearnerID == id);

            if (Learner == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                // Remove UserID from validation because it is set manually
                ModelState.Remove("Learner.UserID");

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors);
                    foreach (var error in errors)
                    {
                        Console.WriteLine(error.ErrorMessage); // Log validation errors
                    }
                    return Page();
                }

                // Re-fetch the existing learner from the database
                var existingLearner = await _context.Learners.AsNoTracking()
                    .FirstOrDefaultAsync(l => l.LearnerID == Learner.LearnerID);

                if (existingLearner == null)
                {
                    return NotFound();
                }

                // Ensure UserID and Registration Date remain unchanged
                Learner.UserID = existingLearner.UserID;
                Learner.Learner_registration_date = existingLearner.Learner_registration_date;

                // Attach Learner and explicitly mark editable fields as modified
                _context.Attach(Learner);
                _context.Entry(Learner).Property(l => l.Learner_firstname).IsModified = true;
                _context.Entry(Learner).Property(l => l.Learner_lastname).IsModified = true;
                _context.Entry(Learner).Property(l => l.LearnerEmail).IsModified = true;
                _context.Entry(Learner).Property(l => l.CountryOfBirth).IsModified = true;

                // Registration Date and UserID are NOT modified

                await _context.SaveChangesAsync();

                Console.WriteLine("Learner profile updated successfully.");
                return RedirectToPage("./Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LearnerExists(Learner.LearnerID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred.");
                return Page();
            }
        }


        private bool LearnerExists(int id)
        {
            return _context.Learners.Any(e => e.LearnerID == id);
        }
    }
}