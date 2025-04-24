using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechSkillConnect.Data;
using TechSkillConnect.Models;

namespace TechSkillConnect.Pages.Learnerpage
{
    public class learner_sign_up_pageModel : PageModel
    {
        private readonly TechSkillConnect.Data.ApplicationDbContext _context;

        public learner_sign_up_pageModel(TechSkillConnect.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Learner Learner { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Learners.Add(Learner);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
