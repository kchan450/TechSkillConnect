using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechSkillConnect.Data;
using TechSkillConnect.Models;

namespace TechSkillConnect.Pages.Admin.Transactions
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Transaction Transaction { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Transaction = await _context.Transactions
                .Include(t => t.Tutor) // ✅ Include tutor to display email
                .FirstOrDefaultAsync(m => m.TransactionID == id);

            if (Transaction == null)
            {
                return NotFound();
            }

            ViewData["TutorID"] = new SelectList(_context.Tutors, "TutorID", "TutorEmail");
            ViewData["PaymentID"] = new SelectList(_context.Transactions, "PaymentID", "PaymentID"); // ✅ Added Payment dropdown

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var transactionToUpdate = await _context.Transactions.FindAsync(Transaction.TransactionID);
            if (transactionToUpdate == null)
            {
                return NotFound();
            }

            // ✅ Update only relevant fields
            transactionToUpdate.TutorID = Transaction.TutorID;
            transactionToUpdate.PaymentID = Transaction.PaymentID;

            // ✅ Auto-generate values
            transactionToUpdate.Sub_status = DateTime.UtcNow >= transactionToUpdate.Sub_start_date && DateTime.UtcNow <= transactionToUpdate.Sub_end_date ? "Active" : "Inactive";

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionExists(Transaction.TransactionID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(e => e.TransactionID == id);
        }
    }
}
