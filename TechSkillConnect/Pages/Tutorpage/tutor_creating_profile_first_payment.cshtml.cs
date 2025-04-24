using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TechSkillConnect.Models;
using TechSkillConnect.Data;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;


namespace TechSkillConnect.Pages.Tutorpage
{
    public class TutorCreatingProfileFirstPaymentModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public TutorProfile TutorProfile { get; set; } = new TutorProfile();

        [BindProperty]
        public Tutor Tutor { get; set; } = new Tutor();

        public TutorCreatingProfileFirstPaymentModel(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            // Fetch the logged-in user's ID
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Fetch tutor information from the database based on the logged-in user
            var tutor = await _context.Tutors.FirstOrDefaultAsync(t => t.IdentityID == userId);

            if (tutor == null)
            {
                // Redirect to the profile creation page if no tutor record is found
                return RedirectToPage("/Tutorpage/TutorCreatingProfile");
            }

            // Fetch tutor profile information from the database
            var tutorProfile = await _context.TutorProfiles.FirstOrDefaultAsync(p => p.TutorID == tutor.TutorID);

            // Assign the data to the properties
            this.Tutor = tutor;
            this.TutorProfile = tutorProfile ?? new TutorProfile { TutorID = tutor.TutorID };

            return Page();
        }

        //public async Task<IActionResult> OnGetAsync()
        //{
        //    // Check if TempData contains tutor and profile data
        //    string tutorData = TempData["Tutor"] as string;
        //    string tutorProfileData = TempData["TutorProfile"] as string;

        //    // If TempData is missing, try fetching from the database
        //    if (string.IsNullOrEmpty(tutorData) || string.IsNullOrEmpty(tutorProfileData))
        //    {
        //        Console.WriteLine("TempData missing or invalid. Checking if tutor record exists.");

        //        // Fetch tutor information from the database based on the logged-in user
        //        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get logged-in user's ID
        //        var tutor = _context.Tutors.FirstOrDefault(t => t.IdentityID == userId);

        //        if (tutor == null)
        //        {
        //            // Redirect to the profile creation page if no tutor record is found
        //            return RedirectToPage("/Tutorpage/TutorCreatingProfile");
        //        }

        //        // Fetch tutor profile information from the database
        //        var tutorProfile = _context.TutorProfiles.FirstOrDefault(p => p.TutorID == tutor.TutorID);

        //        // If no tutor profile exists, create a new instance
        //        this.Tutor = tutor;
        //        this.TutorProfile = tutorProfile ?? new TutorProfile { TutorID = tutor.TutorID };

        //        return Page();
        //    }

        //    // If TempData is valid, deserialize and assign data to properties
        //    this.Tutor = JsonConvert.DeserializeObject<Tutor>(tutorData) ?? new Tutor();
        //    this.TutorProfile = JsonConvert.DeserializeObject<TutorProfile>(tutorProfileData) ?? new TutorProfile();

        //    TempData.Keep("Tutor");
        //    TempData.Keep("TutorProfile");

        //    return Page();
        //}
        public async Task<IActionResult> OnPostAsync()
        {
            Console.WriteLine("Entering OnPostAsync");

            try
            {
                // Get the current user's ID
                string currentUserID = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Fetch Tutor data directly from the database
                var tutor = await _context.Tutors.FirstOrDefaultAsync(t => t.IdentityID == currentUserID);
                if (tutor == null)
                {
                    // If no tutor found, return an error message
                    ModelState.AddModelError("", "No tutor found for the current user.");
                    return Page();
                }

                // Update the subscription details
                var transaction = new Transaction
                {
                    IdentityID =Guid.NewGuid().ToString(),
                    TutorID = tutor.TutorID,
                    PaymentID = Guid.NewGuid().ToString(),
                    Sub_Fee = 68, // The subscription fee
                    Payment_timestamp = DateTime.UtcNow,
                    Sub_start_date = DateTime.UtcNow,
                    Sub_end_date = DateTime.UtcNow.AddDays(30), // Example: 1 month subscription
                    Sub_status = "Active"
                };

                // Add the transaction record
                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();
                Console.WriteLine("Transaction saved");

                // Redirect to success page after the payment
                return RedirectToPage("/Tutorpage/tutor_creating_profile_first_payment_result");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                ModelState.AddModelError("", "An error occurred while processing your payment. Please try again.");
                return Page(); // Stay on the current page to show the error message
            }
        }

    }
}
