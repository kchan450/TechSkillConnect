using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechSkillConnect.Data;
using TechSkillConnect.Models;
using TechSkillConnect.Models.ViewModels;

namespace TechSkillConnect.Pages.Admin.Connections
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        //public async Task<IActionResult> OnGetAsync()
        //{
        //    ViewData["LearnerID"] = new SelectList(await _context.Learners.ToListAsync(), "LearnerID", "LearnerEmail");
        //    ViewData["TutorID"] = new SelectList(await _context.Tutors.ToListAsync(), "TutorID", "TutorEmail");
        //    return Page();
        //}

        public async Task<IActionResult> OnGetAsync()
        {
            ConnectionVM = new ConnectionEditViewModel
            {
                ConnectionDate = DateTime.UtcNow, // default value
                TutorList = await _context.Tutors
                    .Select(t => new SelectListItem { Value = t.TutorID.ToString(), Text = t.TutorEmail })
                    .ToListAsync(),
                LearnerList = await _context.Learners
                    .Select(l => new SelectListItem { Value = l.LearnerID.ToString(), Text = l.LearnerEmail })
                    .ToListAsync()
            };

            return Page();
        }

        //[BindProperty]
        //public Connection Connection { get; set; } = new Connection();

        [BindProperty]
        public ConnectionEditViewModel ConnectionVM { get; set; } = new();

        //public async Task<IActionResult> OnPostAsync()
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Page();
        //    }

        //    // ✅ Check if the connection already exists
        //    var existingConnection = await _context.Connections
        //        .FirstOrDefaultAsync(c => c.TutorID == Connection.TutorID && c.LearnerID == Connection.LearnerID);

        //    if (existingConnection != null)
        //    {
        //        ModelState.AddModelError("", "This learner is already connected with this tutor.");
        //        return Page();
        //    }

        //    // ✅ Auto-generate the ConnectionDate
        //    Connection.ConnectionDate = DateTime.UtcNow;

        //    _context.Connections.Add(Connection);
        //    await _context.SaveChangesAsync();

        //    return RedirectToPage("./Index");
        //}
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Reload dropdowns
                ConnectionVM.TutorList = await _context.Tutors
                    .Select(t => new SelectListItem { Value = t.TutorID.ToString(), Text = t.TutorEmail })
                    .ToListAsync();

                ConnectionVM.LearnerList = await _context.Learners
                    .Select(l => new SelectListItem { Value = l.LearnerID.ToString(), Text = l.LearnerEmail })
                    .ToListAsync();

                return Page();
            }

            var connection = new Connection
            {
                TutorID = ConnectionVM.TutorID,
                LearnerID = ConnectionVM.LearnerID,
                ConnectionDate = ConnectionVM.ConnectionDate
            };

            _context.Connections.Add(connection);
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }

    }
}
