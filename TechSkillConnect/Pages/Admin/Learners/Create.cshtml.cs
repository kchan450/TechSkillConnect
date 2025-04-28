using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechSkillConnect.Data;
using TechSkillConnect.Models;

namespace TechSkillConnect.Pages.Admin.Learners
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
        public Learner Learner { get; set; } = default!;

        public IActionResult OnGet()
        {
            Learner = new Learner
            {
                Learner_registration_date = DateTime.UtcNow
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                // ✅ Skip validation on Learner.UserID because it's set later
                ModelState.Remove("Learner.UserID");

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

                // 1. Create a user account in AspNetUsers
                var user = new IdentityUser
                {
                    UserName = Learner.LearnerEmail,
                    Email = Learner.LearnerEmail
                };

                var result = await _userManager.CreateAsync(user, "ABC123!");

                if (result.Succeeded)
                {
                    Console.WriteLine($"User created successfully. UserId: {user.Id}");

                    // 2. Add learner profile, linked to the AspNetUsers.Id
                    Learner.UserID = user.Id;
                    Learner.Learner_registration_date = DateTime.Now;

                    _context.Learners.Add(Learner);
                    await _context.SaveChangesAsync();

                    Console.WriteLine("Learner profile saved successfully.");
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
