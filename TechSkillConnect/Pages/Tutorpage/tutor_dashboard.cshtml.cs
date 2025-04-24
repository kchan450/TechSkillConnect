using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TechSkillConnect.Data;
using Microsoft.EntityFrameworkCore;

namespace TechSkillConnect.Pages.TutorPage
{
    public class TutorDashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public string ProfileStatus { get; set; }
        public string PaymentStatus { get; set; }

        public TutorDashboardModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get logged-in user's ID
            var tutor = await _context.Tutors.FirstOrDefaultAsync(t => t.IdentityID == userId);

            if (tutor != null)
            {
                // Check if the profile is complete
                ProfileStatus = "Complete";

                // Set the payment status based on whether the tutor has paid
                // We are assuming you have a transaction status to check here, adjust accordingly
                var transaction = await _context.Transactions
                    .FirstOrDefaultAsync(t => t.TutorID == tutor.TutorID);

                PaymentStatus = transaction != null && transaction.Sub_status == "Active" ? "Paid" : "Pending";
            }
            else
            {
                // No tutor found for the user, set status to Incomplete and Pending
                ProfileStatus = "Incomplete";
                PaymentStatus = "Pending";
            }

            return Page();
        }
    }
}