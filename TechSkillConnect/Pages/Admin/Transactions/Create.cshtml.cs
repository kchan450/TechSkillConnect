using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechSkillConnect.Data;
using TechSkillConnect.Models;

namespace TechSkillConnect.Pages.Admin.Transactions
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["TutorID"] = new SelectList(_context.Tutors, "TutorID", "TutorEmail");
            ViewData["PaymentID"] = new SelectList(_context.Transactions, "PaymentID", "PaymentID"); // ✅ Added Payment dropdown

            return Page();
        }

        [BindProperty]
        public Transaction Transaction { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // ✅ Auto-generate values
            Transaction.Payment_timestamp = DateTime.UtcNow;
            Transaction.Sub_start_date = Transaction.Payment_timestamp;
            Transaction.Sub_end_date = Transaction.Sub_start_date.AddDays(30);
            Transaction.Sub_status = DateTime.UtcNow >= Transaction.Sub_start_date && DateTime.UtcNow <= Transaction.Sub_end_date ? "Active" : "Inactive";
            Transaction.IdentityID = Transaction.IdentityID; //sherry
            _context.Transactions.Add(Transaction);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
