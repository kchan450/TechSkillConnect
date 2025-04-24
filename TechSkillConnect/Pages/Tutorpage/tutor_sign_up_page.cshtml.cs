using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TechSkillConnect.Models;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace TechSkillConnect.Pages.Tutorpage
{
    public class tutor_sign_up_pageModel : PageModel
    {
        private readonly TechSkillConnect.Data.ApplicationDbContext _context;

        public tutor_sign_up_pageModel(TechSkillConnect.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public TutorSignUpViewModel TutorSignUp { get; set; } = new TutorSignUpViewModel();

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Log validation errors for debugging
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    System.Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                }
                return Page();
            }

            // Map TutorSignUpViewModel to Tutor model
            var tutor = new Tutor
            {
                //Tutor_username = TutorSignUp.Tutor_username,
                TutorEmail = TutorSignUp.TutorEmail,
                //TutorPassword = TutorSignUp.TutorPassword
                // Other fields like Tutor_firstname, Tutor_lastname, etc., remain null for now
            };

            // Store the partial Tutor data in TempData
            TempData["TutorSignUpData"] = JsonConvert.SerializeObject(tutor);
            TempData.Keep("TutorSignUpData");

            // Redirect to the personal info page
            return RedirectToPage("tutor_creating_profile_PI");
        }
    }

    // Define TutorSignUpViewModel without TermsAccepted
    public class TutorSignUpViewModel
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 30 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9_]*$", ErrorMessage = "Username can only contain letters, numbers, and underscores.")]
        [Display(Name = "Tutor Username")]
        public string Tutor_username { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [Display(Name = "Tutor Email")]
        public string TutorEmail { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 50 characters.")]
        [DataType(DataType.Password)]
        [Display(Name = "Tutor Password")]
        public string TutorPassword { get; set; }
    }
}