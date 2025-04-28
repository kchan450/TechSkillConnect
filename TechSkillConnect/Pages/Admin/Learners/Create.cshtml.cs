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
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // 1. Create a user account in AspNetUsers
            var user = new IdentityUser
            {
                UserName = Learner.LearnerEmail,  // Email as username
                Email = Learner.LearnerEmail
            };

            var result = await _userManager.CreateAsync(user, "ABC123!");  // Set default password

            if (result.Succeeded)
            {
                // 2. Add learner profile, linked to the AspNetUsers.Id
                Learner.UserID = user.Id;  // Link to AspNetUsers
                Learner.Learner_registration_date = DateTime.UtcNow;

                _context.Learners.Add(Learner);
                await _context.SaveChangesAsync();

                return RedirectToPage("./Index");
            }
            else
            {
                // Handle user creation errors
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }
        }
    }
}
