using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TechSkillConnect.Models;
using TechSkillConnect.Data;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace TechSkillConnect.Pages.Tutorpage
{
    public class TutorCreatingProfileFirstPaymentResultModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public TutorProfile TutorProfile { get; set; } = new TutorProfile();

        [BindProperty]
        public Tutor Tutor { get; set; } = new Tutor(); // Ensure instance exists

        public TutorCreatingProfileFirstPaymentResultModel(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            // Retrieve the logged-in user's ID
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Fetch tutor information from the database based on the logged-in user
            var tutor = await _context.Tutors.FirstOrDefaultAsync(t => t.IdentityID == userId);

            // If tutor does not exist, redirect to the profile creation page
            if (tutor == null)
            {
                return RedirectToPage("/Tutorpage/tutor_creating_profile");
            }

            // Fetch tutor profile information from the database
            var tutorProfile = await _context.TutorProfiles.FirstOrDefaultAsync(p => p.TutorID == tutor.TutorID);

            // Assign the tutor and tutor profile to the page model properties
            this.Tutor = tutor;
            this.TutorProfile = tutorProfile ?? new TutorProfile { TutorID = tutor.TutorID };

            return Page();
        }

        //public async Task<IActionResult> OnGetAsync()
        //{
        //    // Retrieve the tutor and tutor profile from TempData
        //    string tutorData = TempData["Tutor"] as string;
        //    string tutorProfileData = TempData["TutorProfile"] as string;

        //    // If TempData is missing or invalid, redirect back to the first payment page
        //    if (string.IsNullOrEmpty(tutorData) || string.IsNullOrEmpty(tutorProfileData))
        //    {
        //        Console.WriteLine("TempData missing or invalid. Redirecting to TutorCreatingProfile.");
        //        return RedirectToPage("/Tutorpage/tutor_creating_profile");
        //    }

        //    // Deserialize the data to populate Tutor and TutorProfile objects
        //    this.Tutor = JsonConvert.DeserializeObject<Tutor>(tutorData) ?? new Tutor();
        //    this.TutorProfile = JsonConvert.DeserializeObject<TutorProfile>(tutorProfileData) ?? new TutorProfile();

        //    // Keep TempData for future use
        //    TempData.Keep("Tutor");
        //    TempData.Keep("TutorProfile");

        //    return Page();
        //}

        public async Task<IActionResult> OnPostAsync()
        {
            // Log transaction details for debugging
            string tutorData = TempData.Peek("Tutor") as string;
            string tutorProfileData = TempData.Peek("TutorProfile") as string;
            Console.WriteLine($"tutorData: {tutorData}");
            Console.WriteLine($"tutorProfileData: {tutorProfileData}");

            try
            {
                // Deserialize the tutor and tutor profile data
                this.Tutor = JsonConvert.DeserializeObject<Tutor>(tutorData) ?? new Tutor();
                this.TutorProfile = JsonConvert.DeserializeObject<TutorProfile>(tutorProfileData) ?? new TutorProfile();

                // Ensure that necessary fields are set
                if (string.IsNullOrEmpty(this.Tutor.TutorEmail))
                    this.Tutor.TutorEmail = "default@example.com"; // Use the default or the logged-in user's email

                // Save the final Tutor information
                _context.Tutors.Add(this.Tutor);
                await _context.SaveChangesAsync();
                Console.WriteLine($"Tutor saved with TutorID: {this.Tutor.TutorID}");

                // Assign TutorID to TutorProfile
                this.TutorProfile.TutorID = this.Tutor.TutorID;

                // Ensure FeePerSession is set
                if (this.TutorProfile.FeePerSession == 0) this.TutorProfile.FeePerSession = 68; // Set to default fee

                // Save TutorProfile to database
                _context.TutorProfiles.Add(this.TutorProfile);
                await _context.SaveChangesAsync();
                Console.WriteLine("TutorProfile saved");

                // Add a transaction record (simulating successful payment)
                var transaction = new Transaction
                {
                    TutorID = this.Tutor.TutorID,
                    PaymentID = Guid.NewGuid().ToString(), // Use a unique GUID for PaymentID
                    Sub_Fee = 68,   // Subscription fee
                    Payment_timestamp = DateTime.UtcNow,
                    Sub_start_date = DateTime.UtcNow,
                    Sub_end_date = DateTime.UtcNow.AddDays(30), // 30-day subscription
                    Sub_status = "Active"
                };

                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();
                Console.WriteLine("Transaction saved");

                // Log success and redirect to the success page
                Console.WriteLine("Payment successful. Redirecting to the success page.");
                return RedirectToPage("/Tutorpage/tutor_creating_profile_first_payment_result");
            }
            catch (Exception ex)
            {
                // Catch errors and log
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                ModelState.AddModelError("", "An error occurred while processing your request. Please try again.");
                return Page();
            }
        }
    }
}
