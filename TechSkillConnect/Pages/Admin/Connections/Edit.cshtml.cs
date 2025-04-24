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
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }



        //[BindProperty]
        //public Connection Connection { get; set; } = default!;


        [BindProperty]
        public ConnectionEditViewModel ConnectionVM { get; set; } = default!;

        //public async Task<IActionResult> OnGetAsync(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    Connection = await _context.Connections
        //        .Include(c => c.Tutor)
        //        .Include(c => c.Learner)
        //        .FirstOrDefaultAsync(m => m.ConnectionID == id);

        //    if (Connection == null)
        //    {
        //        return NotFound();
        //    }

        //    ViewData["LearnerID"] = new SelectList(await _context.Learners.ToListAsync(), "LearnerID", "LearnerEmail");
        //    ViewData["TutorID"] = new SelectList(await _context.Tutors.ToListAsync(), "TutorID", "TutorEmail");

        //    return Page();
        //}

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var connection = await _context.Connections.FindAsync(id);
            if (connection == null) return NotFound();

            ConnectionVM = new ConnectionEditViewModel
            {
                ConnectionID = connection.ConnectionID,
                TutorID = connection.TutorID,
                LearnerID = connection.LearnerID,
                ConnectionDate = connection.ConnectionDate,
                TutorList = await _context.Tutors
                    .Select(t => new SelectListItem { Value = t.TutorID.ToString(), Text = t.TutorEmail })
                    .ToListAsync(),
                LearnerList = await _context.Learners
                    .Select(l => new SelectListItem { Value = l.LearnerID.ToString(), Text = l.LearnerEmail })
                    .ToListAsync()
            };

            return Page();
        }

        //public async Task<IActionResult> OnPostAsync()
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        //Reload dropdowns to avoid empty select lists on redisplay
        //        ViewData["LearnerID"] = new SelectList(await _context.Learners.ToListAsync(), "LearnerID", "LearnerEmail");
        //        ViewData["TutorID"] = new SelectList(await _context.Tutors.ToListAsync(), "TutorID", "TutorEmail");


        //        return Page();
        //    }

        //    try
        //    {
        //        _context.Attach(Connection).State = EntityState.Modified;

        //        // ✅ Prevent modification of ConnectionDate
        //        _context.Entry(Connection).Property(x => x.ConnectionDate).IsModified = false;

        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!ConnectionExists(Connection.ConnectionID))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return RedirectToPage("./Index");
        //}

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Reload dropdowns if validation fails
                ConnectionVM.TutorList = await _context.Tutors
                    .Select(t => new SelectListItem { Value = t.TutorID.ToString(), Text = t.TutorEmail })
                    .ToListAsync();

                ConnectionVM.LearnerList = await _context.Learners
                    .Select(l => new SelectListItem { Value = l.LearnerID.ToString(), Text = l.LearnerEmail })
                    .ToListAsync();

                return Page();
            }

            var connection = await _context.Connections.FindAsync(ConnectionVM.ConnectionID);
            if (connection == null) return NotFound();

            connection.TutorID = ConnectionVM.TutorID;
            connection.LearnerID = ConnectionVM.LearnerID;
            // ConnectionDate is readonly and not updated

            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }

        private bool ConnectionExists(int id)
        {
            return _context.Connections.Any(e => e.ConnectionID == id);
        }
    }
}
