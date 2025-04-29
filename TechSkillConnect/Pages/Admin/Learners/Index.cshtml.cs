using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TechSkillConnect.Data;
using TechSkillConnect.Models;

namespace TechSkillConnect.Pages.Admin.Learners
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

      //  public IList<Learner> Learner { get; set; } = default!;


        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SelectedCountryOfBirth { get; set; }

        public SelectList? CountryOfBirthOptions { get; set; }

        public string CurrentSort { get; set; }
        public string FirstnameSort { get; set; }
        public string LastnameSort { get; set; }
        public string EmailSort { get; set; }
        public string CountryOfBirthSort { get; set; }
        public string RegistrationDateSort { get; set; }


        public string CurrentFilter { get; set; }

        public PaginatedList<Learner> Learners { get; set; }

        public async Task OnGetAsync(string sortOrder, string currentFilter, string SearchString, int? pageIndex)
        {
            //sherry
            //only admin user can access this page

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != "b8bb4c3d-1383-489d-b070-7205c81f623b") //admin user id example for account t1@t.com
            {
                Response.Redirect("/Identity/Account/Login");
                return;
            }


            CurrentSort = sortOrder;
            FirstnameSort = sortOrder == "firstname" ? "firstname_desc" : "firstname";
            LastnameSort = sortOrder == "lastname" ? "lastname_desc" : "lastname";
            EmailSort = sortOrder == "email" ? "email_desc" : "email";
            CountryOfBirthSort = sortOrder == "countryOfBirth" ? "countryOfBirth_desc" : "countryOfBirth";
            RegistrationDateSort = sortOrder == "registrationDate" ? "registrationDate_desc" : "registrationDate";

            if (SearchString != null)
            {
                pageIndex = 1;

            }
            else
            {
                SearchString = currentFilter;
            }
            currentFilter = SearchString;
            
            IQueryable<Learner> learnerIQ = from t in _context.Learners
                                           select t;
           
            switch (sortOrder)
            {
                case "firstname":
                    learnerIQ = learnerIQ.OrderBy(l => l.Learner_firstname);
                    break;
                case "firstname_desc":
                    learnerIQ = learnerIQ.OrderByDescending(l => l.Learner_firstname);
                    break;
                case "lastname":
                    learnerIQ = learnerIQ.OrderBy(l => l.Learner_lastname);
                    break;
                case "lastname_desc":
                    learnerIQ = learnerIQ.OrderByDescending(l => l.Learner_lastname);
                    break;
                case "email":
                    learnerIQ = learnerIQ.OrderBy(l => l.LearnerEmail);
                    break;
                case "email_desc":
                    learnerIQ = learnerIQ.OrderByDescending(l => l.LearnerEmail);
                    break;
                case "countryOfBirth":
                    learnerIQ = learnerIQ.OrderBy(l => l.CountryOfBirth);
                    break;
                case "countryOfBirth_desc":
                    learnerIQ = learnerIQ.OrderByDescending(l => l.CountryOfBirth);
                    break;
                case "registrationDate":
                    learnerIQ = learnerIQ.OrderBy(l => l.Learner_registration_date);
                    break;
                case "registrationDate_desc":
                    learnerIQ = learnerIQ.OrderByDescending(l => l.Learner_registration_date);
                    break;
                default:
                    learnerIQ = learnerIQ.OrderBy(l => l.Learner_firstname);
                    break;
            }


            var Learner = await _context.Learners
                .OrderBy(l => l.Learner_firstname)
                .ToListAsync();

            var CountryOfBirthQuery = _context.Learners
              .OrderBy(l => l.Learner_firstname)
              .Select(l => l.CountryOfBirth)
              .Distinct();

            CountryOfBirthOptions = new SelectList(await CountryOfBirthQuery.ToListAsync());

   
            // Apply search filter
            if (!string.IsNullOrEmpty(SearchString))
            {
                learnerIQ = learnerIQ.Where(l =>
                    l.Learner_firstname.Contains(SearchString) ||
                    l.Learner_lastname.Contains(SearchString) ||
                    l.LearnerEmail.Contains(SearchString) ||
                    l.CountryOfBirth.Contains(SearchString) ||
                    l.Learner_registration_date.ToString().Contains(SearchString));
            }

            // Apply Country filter
            if (!string.IsNullOrEmpty(SelectedCountryOfBirth))
            {
                learnerIQ = learnerIQ.Where(t => t.CountryOfBirth == SelectedCountryOfBirth);
            }

            int pageSize = 10;
            // Fetch Learner
            Learners= await PaginatedList<Learner>.CreateAsync(learnerIQ.AsNoTracking(), pageIndex ?? 1, pageSize);



        }
    }
}
