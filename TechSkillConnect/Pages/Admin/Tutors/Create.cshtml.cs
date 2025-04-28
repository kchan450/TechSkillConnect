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
        private readonly RoleManager<IdentityRole> _roleManager;

        public CreateModel(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
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

                // 1. Ensure "Tutor" role exists
                if (!await _roleManager.RoleExistsAsync("Tutor"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Tutor"));
                }

                // 2. Create AspNetUser
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

                // 3. Assign "Tutor" role (AspNetUserRoles)
                var roleResult = await _userManager.AddToRoleAsync(user, "Tutor");
                if (!roleResult.Succeeded)
                {
                    foreach (var error in roleResult.Errors)
                    {
                        Console.WriteLine($"Role assignment error: {error.Description}");
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return Page();
                }

                Console.WriteLine($"✅ User assigned to role 'Tutor'.");

                // 4. Add Tutor linked to AspNetUser
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
