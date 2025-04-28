using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TechSkillConnect.Data;
using TechSkillConnect.Models;

namespace TechSkillConnect.Pages.Admin.Tutors
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public EditModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Tutor Tutor { get; set; } = default!;  // Directly bind Tutor.cs

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            Tutor = await _context.Tutors.FirstOrDefaultAsync(t => t.TutorID == id);

            if (Tutor == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("Tutor.IdentityID");  // Remove IdentityID from validation

            if (!ModelState.IsValid)
                return Page();

            var existingTutor = await _context.Tutors.AsNoTracking()
                .FirstOrDefaultAsync(t => t.TutorID == Tutor.TutorID);

            if (existingTutor == null)
                return NotFound();

            // Preserve IdentityID and Registration Date
            Tutor.IdentityID = existingTutor.IdentityID;
            Tutor.Tutor_registration_date = existingTutor.Tutor_registration_date;

            // Update specific fields
            _context.Attach(Tutor);
            _context.Entry(Tutor).Property(t => t.Tutor_firstname).IsModified = true;
            _context.Entry(Tutor).Property(t => t.Tutor_lastname).IsModified = true;
            _context.Entry(Tutor).Property(t => t.TutorEmail).IsModified = true;
            _context.Entry(Tutor).Property(t => t.Tutor_phone).IsModified = true;
            _context.Entry(Tutor).Property(t => t.CountryOfBirth).IsModified = true;

            await _context.SaveChangesAsync();

            // Update AspNetUsers
            var user = await _userManager.FindByIdAsync(Tutor.IdentityID);
            if (user != null)
            {
                user.UserName = Tutor.TutorEmail;
                user.Email = Tutor.TutorEmail;
                user.NormalizedUserName = Tutor.TutorEmail.ToUpperInvariant();
                user.NormalizedEmail = Tutor.TutorEmail.ToUpperInvariant();

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    foreach (var error in updateResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return Page();
                }
            }

            return RedirectToPage("./Index");
        }
    }
}
