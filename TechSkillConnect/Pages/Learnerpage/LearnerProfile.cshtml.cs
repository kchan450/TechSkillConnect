//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.RazorPages;
//using Microsoft.EntityFrameworkCore;
//using TechSkillConnect.Data;
//using TechSkillConnect.Models;
//using System.Threading.Tasks;
//using System.Security.Claims;

//namespace TechSkillConnect.Pages.Learnerpage
//{
//    public class LearnerProfileModel : PageModel
//    {
//        private readonly ApplicationDbContext _context;

//        [BindProperty]
//        public Learner Learner { get; set; }

//        [BindProperty]
//        public LearnerProfile LearnerProfile { get; set; }

//        public LearnerProfileModel(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        public async Task<IActionResult> OnGetAsync()
//        {
//            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

//            // Ensure the user has a Learner record
//            Learner = await _context.Learners.FirstOrDefaultAsync(l => l.IdentityID == userId);

//            if (Learner == null)
//            {
//                // If no learner record, redirect to the profile creation page
//                return RedirectToPage("/Learnerpage/LearnerCreatingProfile");
//            }

//            // Fetch the associated LearnerProfile record
//            LearnerProfile = await _context.LearnerProfiles.FirstOrDefaultAsync(p => p.LearnerID == Learner.LearnerID);

//            // If no learner profile exists, create a new instance
//            LearnerProfile ??= new LearnerProfile { LearnerID = Learner.LearnerID };

//            return Page();
//        }

//        public async Task<IActionResult> OnPostAsync()
//        {
//            if (!ModelState.IsValid)
//            {
//                return Page();
//            }

//            // Update or create LearnerProfile
//            var profile = await _context.LearnerProfiles.FirstOrDefaultAsync(p => p.LearnerID == Learner.LearnerID);

//            if (profile == null)
//            {
//                _context.LearnerProfiles.Add(LearnerProfile);
//            }
//            else
//            {
//                profile.Language = LearnerProfile.Language;
//                profile.YearsOfStudy = LearnerProfile.YearsOfStudy;
//                profile.StudyArea = LearnerProfile.StudyArea;
//                profile.Goals = LearnerProfile.Goals;
//            }

//            // Save changes
//            await _context.SaveChangesAsync();

//            return RedirectToPage("/Learnerpage/LearnerProfile");
//        }
//    }
//}
