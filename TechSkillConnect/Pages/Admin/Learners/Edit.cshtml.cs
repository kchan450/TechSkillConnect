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
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage); // Optional: Log validation errors
                }
                return Page();
            }

            // Re-fetch the existing learner from the database to get non-editable fields (like UserID)
            var existingLearner = await _context.Learners.AsNoTracking()
                .FirstOrDefaultAsync(l => l.LearnerID == Learner.LearnerID);

            if (existingLearner == null)
            {
                return NotFound();
            }

            // Ensure UserID and Registration Date remain unchanged
            Learner.UserID = existingLearner.UserID;
            Learner.Learner_registration_date = existingLearner.Learner_registration_date;

            // Attach the updated learner
            _context.Attach(Learner).State = EntityState.Modified;

            // Ensure Registration Date is not modified (this is optional now since we reset it above)
            _context.Entry(Learner).Property(x => x.Learner_registration_date).IsModified = false;

            try
            {
                await _context.SaveChangesAsync();
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

            return RedirectToPage("./Index");
        }

        private bool LearnerExists(int id)
        {
            return _context.Learners.Any(e => e.LearnerID == id);
        }
    }
}