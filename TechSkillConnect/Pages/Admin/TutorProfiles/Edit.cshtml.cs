using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechSkillConnect.Data;
using TechSkillConnect.Models;

namespace TechSkillConnect.Pages.Admin.TutorProfiles
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public TutorProfile TutorProfile { get; set; } = default!;

        public SelectList LanguageOptions { get; set; } = default!;
        public SelectList YearsOfExperienceOptions { get; set; } = default!;
        public SelectList SkillLevelOptions { get; set; } = default!;
        public SelectList FeePerSessionOptions { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var tutorprofile = await _context.TutorProfiles
                .Include(tp => tp.Tutor)
                .FirstOrDefaultAsync(m => m.ProfileID == id);

            if (tutorprofile == null)
                return NotFound();

            TutorProfile = tutorprofile;

            ViewData["TutorID"] = new SelectList(_context.Tutors, "TutorID", "TutorEmail", TutorProfile.TutorID);
            LanguageOptions = GetLanguageOptions(TutorProfile.Language);
            YearsOfExperienceOptions = GetYearsOfExperienceOptions(TutorProfile.YearsOfExperience);
            SkillLevelOptions = GetSkillLevelOptions(TutorProfile.SkillLevel);
            FeePerSessionOptions = GetFeePerSessionOptions(TutorProfile.FeePerSession);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ViewData["TutorID"] = new SelectList(_context.Tutors, "TutorID", "TutorEmail", TutorProfile.TutorID);
                LanguageOptions = GetLanguageOptions(TutorProfile.Language);
                YearsOfExperienceOptions = GetYearsOfExperienceOptions(TutorProfile.YearsOfExperience);
                SkillLevelOptions = GetSkillLevelOptions(TutorProfile.SkillLevel);
                FeePerSessionOptions = GetFeePerSessionOptions(TutorProfile.FeePerSession);
                return Page();
            }

            _context.Attach(TutorProfile).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TutorProfileExists(TutorProfile.ProfileID))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToPage("./Index");
        }

        private bool TutorProfileExists(int id)
        {
            return _context.TutorProfiles.Any(e => e.ProfileID == id);
        }

        private SelectList GetLanguageOptions(string? selectedLanguage)
        {
            var languages = new List<string>
            {
                "Machine Learning",
                "Python",
                "Java",
                "AI",
                "Data Science"
            };

            if (!string.IsNullOrEmpty(selectedLanguage) && !languages.Contains(selectedLanguage))
                languages.Insert(0, selectedLanguage);

            return new SelectList(languages, selectedLanguage);
        }

        private SelectList GetYearsOfExperienceOptions(string? selectedExperience)
        {
            var experiences = new List<string> { "0-2", "3-5", "6-10", "10+" };

            if (!string.IsNullOrEmpty(selectedExperience) && !experiences.Contains(selectedExperience))
                experiences.Insert(0, selectedExperience);

            return new SelectList(experiences, selectedExperience);
        }

        private SelectList GetSkillLevelOptions(string? selectedSkillLevel)
        {
            var skillLevels = new List<string> { "Beginner", "Intermediate", "Advanced", "Expert" };

            if (!string.IsNullOrEmpty(selectedSkillLevel) && !skillLevels.Contains(selectedSkillLevel))
                skillLevels.Insert(0, selectedSkillLevel);

            return new SelectList(skillLevels, selectedSkillLevel);
        }

        private SelectList GetFeePerSessionOptions(int selectedFee)
        {
            var fees = new List<int> { 10, 20, 30, 40, 50, 75, 100 };

            if (!fees.Contains(selectedFee))
                fees.Insert(0, selectedFee);

            return new SelectList(fees, selectedFee);
        }
    }
}
