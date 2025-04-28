using System.Collections.Generic;
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

        public List<SelectListItem> SkillOptions { get; set; } = new();
        public List<SelectListItem> ExperienceOptions { get; set; } = new();
        public List<SelectListItem> SkillLevelOptions { get; set; } = new();
        public List<SelectListItem> FeeOptions { get; set; } = new();

        public IActionResult OnGet()
        {
            ViewData["TutorID"] = new SelectList(_context.Tutors, "TutorID", "TutorEmail");

            TutorProfile = new TutorProfile(); // Ensure TutorProfile is initialized

            LoadDropdowns();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ViewData["TutorID"] = new SelectList(_context.Tutors, "TutorID", "TutorEmail");
                LoadDropdowns();
                return Page();
            }

            _context.TutorProfiles.Add(TutorProfile);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private void LoadDropdowns()
        {
            SkillOptions = GetSkillOptions(TutorProfile?.Language).ToList();
            ExperienceOptions = GetExperienceOptions(TutorProfile?.YearsOfExperience).ToList();
            SkillLevelOptions = GetSkillLevelOptions(TutorProfile?.SkillLevel).ToList();
            FeeOptions = GetFeeOptions(TutorProfile?.FeePerSession ?? 0).ToList();
        }

        private SelectList GetSkillOptions(string? selectedSkill = null)
        {
            var skills = new List<string> { "Machine Learning", "Python", "Java", "AI", "Data Science" };
            if (!string.IsNullOrEmpty(selectedSkill) && !skills.Contains(selectedSkill))
                skills.Insert(0, selectedSkill);
            return new SelectList(skills, selectedSkill);
        }

        private SelectList GetExperienceOptions(string? selectedExperience = null)
        {
            var experiences = new List<string> { "0-2", "3-5", "6-10", "10+" };
            if (!string.IsNullOrEmpty(selectedExperience) && !experiences.Contains(selectedExperience))
                experiences.Insert(0, selectedExperience);
            return new SelectList(experiences, selectedExperience);
        }

        private SelectList GetSkillLevelOptions(string? selectedLevel = null)
        {
            var levels = new List<string> { "Beginner", "Intermediate", "Advanced", "Expert" };
            if (!string.IsNullOrEmpty(selectedLevel) && !levels.Contains(selectedLevel))
                levels.Insert(0, selectedLevel);
            return new SelectList(levels, selectedLevel);
        }

        private SelectList GetFeeOptions(int selectedFee)
        {
            var fees = new List<int> { 10, 20, 30, 40, 50, 75, 100 };
            if (selectedFee != 0 && !fees.Contains(selectedFee))
                fees.Insert(0, selectedFee);
            return new SelectList(fees, selectedFee);
        }
    }
}