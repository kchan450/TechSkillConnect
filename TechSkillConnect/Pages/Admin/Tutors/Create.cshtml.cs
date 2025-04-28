using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<IdentityUser> _userManager;

        public CreateModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Tutor Tutor { get; set; } = new Tutor();

        public SelectList CountryOptions { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Load country options for dropdown
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
                    // Reload dropdown if validation fails
                    CountryOptions = new SelectList(await _context.Tutors
                        .Select(t => t.CountryOfBirth)
                        .Distinct()
                        .ToListAsync());

                    return Page();
                }

                // 1. Create AspNetUser
                var user = new IdentityUser
                {
                    UserName = Tutor.TutorEmail,
                    Email = Tutor.TutorEmail
                };

                var passwordValidators = _userManager.PasswordValidators;
                foreach (var validator in passwordValidators)
                {
                    var validationResult = await validator.ValidateAsync(_userManager, user, "Abc123!");
                    if (!validationResult.Succeeded)
                    {
                        foreach (var error in validationResult.Errors)
                        {
                            Console.WriteLine($"Password validation error: {error.Description}");
                            ModelState.AddModelError(string.Empty, error.Description);
                        }

                        // Reload dropdown
                        CountryOptions = new SelectList(await _context.Tutors
                            .Select(t => t.CountryOfBirth)
                            .Distinct()
                            .ToListAsync());

                        return Page();
                    }
                }

                var result = await _userManager.CreateAsync(user, "Abc123!");
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"User creation error: {error.Description}");
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    // Reload dropdown
                    CountryOptions = new SelectList(await _context.Tutors
                        .Select(t => t.CountryOfBirth)
                        .Distinct()
                        .ToListAsync());

                    return Page();
                }

                Console.WriteLine($"✅ User created: {user.Id}");

                // 2. Add Tutor linked to AspNetUser
                Tutor.IdentityID = user.Id;
                Tutor.Tutor_registration_date = DateTime.UtcNow;

                _context.Tutors.Add(Tutor);
                await _context.SaveChangesAsync();

                Console.WriteLine("🎉 Tutor record successfully created!");

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
                ModelState.AddModelError("", "An unexpected error occurred.");
                return Page();
            }
        }
    }
}
