using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TechSkillConnect.Data;
using TechSkillConnect.ViewModels;

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
        public TutorEditViewModel Tutor { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var tutorEntity = await _context.Tutors.FindAsync(id);
            if (tutorEntity == null)
                return NotFound();

            Tutor = new TutorEditViewModel
            {
                UserID = tutorEntity.IdentityID,  // ✅ Correct
                Tutor_firstname = tutorEntity.Tutor_firstname,
                Tutor_lastname = tutorEntity.Tutor_lastname,
                TutorEmail = tutorEntity.TutorEmail,
                Tutor_phone = tutorEntity.Tutor_phone,
                CountryOfBirth = tutorEntity.CountryOfBirth,
                Tutor_registration_date = tutorEntity.Tutor_registration_date
                // Assuming TutorEntity has UserID
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var tutorEntity = await _context.Tutors.AsNoTracking()
                .FirstOrDefaultAsync(t => t.TutorID == Tutor.TutorID);

            if (tutorEntity == null)
                return NotFound();

            // Preserve UserID and Registration Date
            Tutor.UserID = tutorEntity.IdentityID;  // ✅ Correct // Assuming TutorEditViewModel has UserID
            Tutor.Tutor_registration_date = tutorEntity.Tutor_registration_date;

            // Map back to Tutor Entity
            var updatedTutor = new Models.Tutor
            {
                TutorID = Tutor.TutorID,
                IdentityID = Tutor.UserID,
                Tutor_firstname = Tutor.Tutor_firstname,
                Tutor_lastname = Tutor.Tutor_lastname,
                TutorEmail = Tutor.TutorEmail,
                Tutor_phone = Tutor.Tutor_phone,
                CountryOfBirth = Tutor.CountryOfBirth,
                Tutor_registration_date = Tutor.Tutor_registration_date
            };

            _context.Attach(updatedTutor).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // Update AspNetUsers
            var user = await _userManager.FindByIdAsync(Tutor.UserID);
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
