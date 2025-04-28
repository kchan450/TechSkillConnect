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
        private readonly UserManager<IdentityUser> _userManager;  // ✅ Add this

        // ✅ Inject UserManager in the constructor
        public EditModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;  // ✅ Assign
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
                ModelState.Remove("Learner.UserID");

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors);
                    foreach (var error in errors)
                    {
                        Console.WriteLine(error.ErrorMessage);
                    }
                    return Page();
                }

                // ✅ Fetch existing learner
                var existingLearner = await _context.Learners.AsNoTracking()
                    .FirstOrDefaultAsync(l => l.LearnerID == Learner.LearnerID);

                if (existingLearner == null)
                {
                    return NotFound();
                }

                // Preserve UserID and Registration Date
                Learner.UserID = existingLearner.UserID;
                Learner.Learner_registration_date = existingLearner.Learner_registration_date;

                // Update Learner fields
                _context.Attach(Learner);
                _context.Entry(Learner).Property(l => l.Learner_firstname).IsModified = true;
                _context.Entry(Learner).Property(l => l.Learner_lastname).IsModified = true;
                _context.Entry(Learner).Property(l => l.LearnerEmail).IsModified = true;
                _context.Entry(Learner).Property(l => l.CountryOfBirth).IsModified = true;

                await _context.SaveChangesAsync();
                Console.WriteLine("Learner changes saved successfully.");

                // ✅ Update AspNetUser
                var user = await _userManager.FindByIdAsync(Learner.UserID);
                if (user != null)
                {
                    user.UserName = Learner.LearnerEmail;
                    user.Email = Learner.LearnerEmail;
                    user.NormalizedUserName = Learner.LearnerEmail.ToUpperInvariant();
                    user.NormalizedEmail = Learner.LearnerEmail.ToUpperInvariant();


                    var updateResult = await _userManager.UpdateAsync(user);
                    if (updateResult.Succeeded)
                    {
                        Console.WriteLine("AspNetUser updated successfully.");

                        user.NormalizedUserName = Learner.LearnerEmail.ToUpperInvariant();
                        user.NormalizedEmail = Learner.LearnerEmail.ToUpperInvariant();

                        _context.Attach(user);
                        _context.Entry(user).Property(u => u.NormalizedUserName).IsModified = true;
                        _context.Entry(user).Property(u => u.NormalizedEmail).IsModified = true;

                        await _context.SaveChangesAsync();
                        Console.WriteLine("Normalized fields updated via DbContext.");
                    }
                    else
                    {
                        foreach (var error in updateResult.Errors)
                        {
                            Console.WriteLine($"AspNetUser update error: {error.Description}");
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return Page();
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
