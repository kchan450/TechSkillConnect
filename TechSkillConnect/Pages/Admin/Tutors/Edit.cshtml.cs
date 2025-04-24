//using System;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.RazorPages;
//using Microsoft.EntityFrameworkCore;
//using TechSkillConnect.Data;
//using TechSkillConnect.Models;

//namespace TechSkillConnect.Pages.Admin.Tutors
//{
//    public class EditModel : PageModel
//    {
//        private readonly ApplicationDbContext _context;

//        public EditModel(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        [BindProperty]
//        public Tutor Tutor { get; set; } = default!;

//        public async Task<IActionResult> OnGetAsync(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            Tutor = await _context.Tutors.FirstOrDefaultAsync(m => m.TutorID == id);

//            if (Tutor == null)
//            {
//                return NotFound();
//            }

//            return Page();
//        }

//        public async Task<IActionResult> OnPostAsync()
//        {
//            if (!ModelState.IsValid)
//            {
//                return Page();
//            }

//            try
//            {
//                _context.Attach(Tutor).State = EntityState.Modified;
//                await _context.SaveChangesAsync();
//            }
//            catch (DbUpdateConcurrencyException)
//            {
//                if (!TutorExists(Tutor.TutorID))
//                {
//                    return NotFound();
//                }
//                else
//                {
//                    throw;
//                }
//            }

//            return RedirectToPage("./Index");
//        }

//        private bool TutorExists(int id)
//        {
//            return _context.Tutors.Any(e => e.TutorID == id);
//        }
//    }
//}

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using TechSkillConnect.Data;
using TechSkillConnect.ViewModels;

namespace TechSkillConnect.Pages.Admin.Tutors
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public TutorEditViewModel Tutor { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var tutorEntity = await _context.Tutors.FindAsync(id);
            if (tutorEntity == null)
                return NotFound();

            // Map entity to view model
            Tutor = new TutorEditViewModel
            {
                TutorID = tutorEntity.TutorID,
                //Tutor_username = tutorEntity.Tutor_username,
                Tutor_firstname = tutorEntity.Tutor_firstname,
                Tutor_lastname = tutorEntity.Tutor_lastname,
                TutorEmail = tutorEntity.TutorEmail,
                Tutor_phone = tutorEntity.Tutor_phone,
                CountryOfBirth = tutorEntity.CountryOfBirth,
                Tutor_registration_date = tutorEntity.Tutor_registration_date
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var tutorEntity = await _context.Tutors.FindAsync(Tutor.TutorID);
            if (tutorEntity == null)
                return NotFound();

            // Map view model values back to entity
            //tutorEntity.Tutor_username = Tutor.Tutor_username;
            tutorEntity.Tutor_firstname = Tutor.Tutor_firstname;
            tutorEntity.Tutor_lastname = Tutor.Tutor_lastname;
            tutorEntity.TutorEmail = Tutor.TutorEmail;
            tutorEntity.Tutor_phone = Tutor.Tutor_phone;
            tutorEntity.CountryOfBirth = Tutor.CountryOfBirth;

            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}