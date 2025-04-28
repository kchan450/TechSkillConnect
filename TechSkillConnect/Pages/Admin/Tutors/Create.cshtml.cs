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
                // ✅ Skip validation on Tutor.IdentityID because it's set later
                ModelState.Remove("Tutor.IdentityID");

                if (!ModelState.IsValid)
                {
                    Console.WriteLine("ModelState is invalid.");
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        Console.WriteLine(error.ErrorMessage);
                    }
                    return Page();
                }

                Console.WriteLine("ModelState is valid.");

                // 1. Ensure "Tutor" role exists
                if (!await _roleManager.RoleExistsAsync("Tutor"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Tutor"));
                }

                // 2. Create a user account in AspNetUsers
                var user = new IdentityUser
                {
                    UserName = Tutor.TutorEmail,
                    Email = Tutor.TutorEmail,
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
                        return Page();
                    }
                }

                var result = await _userManager.CreateAsync(user, "Abc123!");

                if (result.Succeeded)
                {
                    Console.WriteLine($"User created successfully. UserId: {user.Id}");

                    user.EmailConfirmed = true;

                    // 3. Add to "Tutor" role
                    await _userManager.AddToRoleAsync(user, "Tutor");

                    // 4. Add Tutor profile, linked to AspNetUsers.Id
                    Tutor.IdentityID = user.Id;
                    Tutor.Tutor_registration_date = DateTime.Now;

                    _context.Tutors.Add(Tutor);
                    await _context.SaveChangesAsync();

                    Console.WriteLine("Tutor profile saved successfully.");
                    return RedirectToPage("./Index");
                }
                else
                {
                    Console.WriteLine("User creation failed.");
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine(error.Description);
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return Page();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred.");
                return Page();
            }
        }

    }
}
