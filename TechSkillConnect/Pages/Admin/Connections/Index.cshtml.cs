using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TechSkillConnect.Data;
using TechSkillConnect.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace TechSkillConnect.Pages.Admin.Connections
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

       // public IList<Connection> Connection { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        public string CurrentSort { get; set; }
        public string ConnectionDateSort { get; set; }
        public string TutorEmailSort { get; set; }
        public string LearnerEmailSort { get; set; }

        public string CurrentFilter { get; set; }
        public PaginatedList<Connection> Connections { get; set; }

        public async Task OnGetAsync(string sortOrder, string currentFilter, string SearchString, int? pageIndex)
        {
            //sherry
            //only admin user can access this page
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != "db0c6b20-0021-425e-998e-c3feb36c6364") //admin user id example for account t1@t.com
            {
                Response.Redirect("/Identity/Account/Login");
                return;
            }


            CurrentSort = sortOrder;
            ConnectionDateSort = sortOrder == "connectionDate" ? "connectionDate_desc" : "connectionDate";
            TutorEmailSort = sortOrder == "tutorEmail" ? "tutorEmail_desc" : "tutorEmail";
            LearnerEmailSort = sortOrder == "learnerEmail" ? "learnerEmail_desc" : "learnerEmail";


            if (SearchString != null)
            {
                pageIndex = 1;

            }
            else
            {
                SearchString = currentFilter;
            }
            currentFilter = SearchString;

            // Start the base query
            IQueryable<Connection> query = _context.Connections
                .Include(c => c.Tutor)
                .Include(c => c.Learner);

            //// Start the base query
            //var query = _context.Connections
            //    .Include(c => c.Tutor)
            //    .Include(c => c.Learner)
            //    .AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(SearchString))
            {
                query = query.Where(c =>
                    c.Tutor.TutorEmail.Contains(SearchString) ||
                    c.Learner.LearnerEmail.Contains(SearchString));
            }



            //Connection = await _context.Connections
            //    .Include(c => c.Tutor)
            //    .Include(c => c.Learner)
            //    .OrderByDescending(c => c.ConnectionDate) // ✅ Show recent connections first
            //    .ToListAsync();

            switch (sortOrder)
            {
                case "connectionDate":
                    query = query.OrderBy(c => c.ConnectionDate);
                    break;
                case "connectionDate_desc":
                    query = query.OrderByDescending(c => c.ConnectionDate);
                    break;
                case "tutorEmail":
                    query = query.OrderBy(c => c.Tutor.TutorEmail);
                    break;
                case "tutorEmail_desc":
                    query = query.OrderByDescending(c => c.Tutor.TutorEmail);
                    break;
                case "learnerEmail":
                    query = query.OrderBy(c => c.Learner.LearnerEmail);
                    break;
                case "learnerEmail_desc":
                    query = query.OrderByDescending(c => c.Learner.LearnerEmail);
                    break;
                default:
                    query = query.OrderByDescending(c => c.ConnectionDate); // default sort
                    break;
            }
            // Define the base query for tutors profiles
            // var query = _context.Connections.AsQueryable();


           
           // Connection = await query.ToListAsync();
            int pageSize = 10;

            // Fetch connections
            Connections = await PaginatedList<Connection>.CreateAsync(query.AsNoTracking(), pageIndex ?? 1, pageSize);


        }
    }
}
