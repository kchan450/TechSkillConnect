using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechSkillConnect.Data;
using TechSkillConnect.Models;

namespace TechSkillConnect.Pages.Admin.TutorProfiles
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public TutorProfile TutorProfile { get; set; } = default!;

        // For displaying Tutor Email (readonly)
        public string TutorEmail { get; set; } = string.Empty;

        // Dropdown options
        public List<SelectListItem> SkillOptions { get; set; } = new();
        public List<SelectListItem> ExperienceOptions { get; set; } = new();
        public List<SelectListItem> SkillLevelOptions { get; set; } = new();
        public List<SelectListItem> FeeOptions { get; set; } = new();

        // Handle GET request, receiving TutorID
        public async Task<IActionResult> OnGetAsync(int? tutorId)
        {
            if (tutorId == null)
            {
                return NotFound();
            }

            var tutor = await _context.Tutors.FindAsync(tutorId);

            if (tutor == null)
            {
                return NotFound();
            }

            // Prepare TutorProfile object with TutorID
            TutorProfile = new TutorProfile
            {
                TutorID = tutor.TutorID,
                Language = "Machine Learning",
                YearsOfExperience = "0-2",
                SkillLevel = "Beginner",
                FeePerSession = 10
            };

            // Save Tutor Email for displaying
            TutorEmail = tutor.TutorEmail;

            // Load dropdown menus
            LoadDropdowns();

            return Page();
        }

        // Handle POST (form submit)
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                LoadDropdowns();
                return Page();
            }

            _context.TutorProfiles.Add(TutorProfile);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        // Helper method to load dropdown lists
        private void LoadDropdowns()
        {
            SkillOptions = GetSkillOptions(TutorProfile?.Language).ToList();
            ExperienceOptions = GetExperienceOptions(TutorProfile?.YearsOfExperience).ToList();
            SkillLevelOptions = GetSkillLevelOptions(TutorProfile?.SkillLevel).ToList();
            FeeOptions = GetFeeOptions(TutorProfile?.FeePerSession ?? 0).ToList();
        }

        // Skill dropdown options
        private SelectList GetSkillOptions(string? selectedSkill = null)
        {
            var skills = new List<string> { "Machine Learning", "Python", "Java", "AI", "Data Science" };
            return new SelectList(skills, selectedSkill ?? skills[0]);
        }

        // Years of Experience dropdown options
        private SelectList GetExperienceOptions(string? selectedExperience = null)
        {
            var experiences = new List<string> { "0-2", "3-5", "6-10", "10+" };
            return new SelectList(experiences, selectedExperience ?? experiences[0]);
        }

        // Skill Level dropdown options
        private SelectList GetSkillLevelOptions(string? selectedLevel = null)
        {
            var levels = new List<string> { "Beginner", "Intermediate", "Advanced", "Expert" };
            return new SelectList(levels, selectedLevel ?? levels[0]);
        }

        // Fee Per Session dropdown options
        private SelectList GetFeeOptions(int selectedFee)
        {
            var fees = new List<int> { 10, 20, 30, 40, 50, 75, 100 };
            return new SelectList(fees, selectedFee != 0 ? selectedFee : fees[0]);
        }
    }
}
