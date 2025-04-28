using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
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

                // ✅ Debug: After ModelState validation
                Console.WriteLine("ModelState is valid.");
                Console.WriteLine($"LearnerID: {Learner.LearnerID}");
                Console.WriteLine($"First Name: {Learner.Learner_firstname}");
                Console.WriteLine($"Last Name: {Learner.Learner_lastname}");
                Console.WriteLine($"Email: {Learner.LearnerEmail}");
                Console.WriteLine($"Country: {Learner.CountryOfBirth}");

                // Re-fetch the existing learner from the database
                var existingLearner = await _context.Learners.AsNoTracking()
                    .FirstOrDefaultAsync(l => l.LearnerID == Learner.LearnerID);

                if (existingLearner == null)
                {
                    return NotFound();
                }

                // ✅ Debug: After fetching existing learner
                Console.WriteLine($"Existing UserID: {existingLearner.UserID}");
                Console.WriteLine($"Existing Registration Date: {existingLearner.Learner_registration_date}");

                // Ensure UserID and Registration Date remain unchanged
                Learner.UserID = existingLearner.UserID;
                Learner.Learner_registration_date = existingLearner.Learner_registration_date;

                // Attach Learner and explicitly mark editable fields as modified
                _context.Attach(Learner);
                _context.Entry(Learner).Property(l => l.Learner_firstname).IsModified = true;
                _context.Entry(Learner).Property(l => l.Learner_lastname).IsModified = true;
                _context.Entry(Learner).Property(l => l.LearnerEmail).IsModified = true;
                _context.Entry(Learner).Property(l => l.CountryOfBirth).IsModified = true;

                // ✅ Debug: Before saving learner changes
                Console.WriteLine("About to save learner changes...");
                await _context.SaveChangesAsync();
                Console.WriteLine("Learner changes saved successfully.");

                // ✅ Now update AspNetUsers using UserManager
                var user = await _userManager.FindByIdAsync(Learner.UserID);
                if (user != null)
                {
                    user.UserName = Learner.LearnerEmail;
                    user.Email = Learner.LearnerEmail;

                    var updateResult = await _userManager.UpdateAsync(user);
                    if (updateResult.Succeeded)
                    {
                        Console.WriteLine("AspNetUser updated successfully.");
                    }
                    else
                    {
                        foreach (var error in updateResult.Errors)
                        {
                            Console.WriteLine($"AspNetUser update error: {error.Description}");
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return Page(); // Stay on page if user update fails
                    }
                }

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