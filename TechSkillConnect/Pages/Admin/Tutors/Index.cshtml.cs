using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechSkillConnect.Data;
using TechSkillConnect.Models;
using System.Security.Claims;

namespace TechSkillConnect.Pages.Admin.Tutors
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

       // public IList<Tutor> Tutors { get; set; } = new List<Tutor>();

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SelectedCountry { get; set; }

        public SelectList? CountryOptions { get; set; }

        public string UsernameSort { get; set; }
        public string FirstnameSort { get; set; }
        public string LastnameSort { get; set; }
        public string EmailSort { get; set; }
        public string PhoneSort { get; set; }
        public string CountryOfBirthSort { get; set; }
        public string TutorRegistrationDateSort { get; set; }

        public string CurrentSort { get; set; }

        public string CurrentFilter { get; set; }

        public PaginatedList<Tutor> Tutors { get; set; }

        public async Task OnGetAsync(string sortOrder, string currentFilter, string SearchString, int? pageIndex)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != "db0c6b20-0021-425e-998e-c3feb36c6364") //admin user id example for account t1@t.com
            {
                Response.Redirect("/Identity/Account/Login");
                return;
            }


            CurrentSort = sortOrder;
            UsernameSort = sortOrder == "username" ? "username_desc" : "username";
            FirstnameSort = sortOrder == "firstname" ? "firstname_desc" : "firstname";
            LastnameSort = sortOrder == "lastname" ? "lastname_desc" : "lastname";
            EmailSort = sortOrder == "email" ? "email_desc" : "email";
            PhoneSort = sortOrder ==  "Phone"? "Phonr_desc" : "Phone";
            CountryOfBirthSort = sortOrder == "countryOfBirth" ? "countryOfBirth_desc" : "countryOfBirth";
            TutorRegistrationDateSort = sortOrder == "tutorRegistrationDate" ? "tutorRegistrationDate_desc" : "tutorRegistrationDate";

            if (SearchString != null)
            {
                pageIndex = 1;

            }
            else {
                SearchString = currentFilter;
            }
            currentFilter = SearchString;

            IQueryable<Tutor> tutorsIQ = from t in _context.Tutors
                                        //.Where(c => c.IdentityID == userId)//sherry only the owner(login user) can see the data
                                         select t;
            switch (sortOrder)
            {
                //case "username_desc":

                //    tutorsIQ = tutorsIQ.OrderByDescending(t=>t.Tutor_username);
                //    break;

                case "firstname_desc":

                    tutorsIQ = tutorsIQ.OrderByDescending(t => t.Tutor_firstname);
                    break;

                case "lastname_desc":

                    tutorsIQ = tutorsIQ.OrderByDescending(t => t.Tutor_lastname);
                    break;

                case "email_desc":

                    tutorsIQ = tutorsIQ.OrderByDescending(t => t.TutorEmail);
                    break;

                case "phone_desc":

                    tutorsIQ = tutorsIQ.OrderByDescending(t => t.Tutor_phone);
                    break;

                case "countryOfBirth_desc":

                    tutorsIQ = tutorsIQ.OrderByDescending(t => t.CountryOfBirth);
                    break;

                case "tutorRegistrationDate":

                    tutorsIQ = tutorsIQ.OrderBy(t => t.Tutor_registration_date);
                    break;

                case "tutorRegistrationDate_desc":

                    tutorsIQ = tutorsIQ.OrderByDescending(t => t.Tutor_registration_date);
                    break;


                //case "username":

                //    tutorsIQ = tutorsIQ.OrderBy(t => t.Tutor_username);
                //    break;

                case "firstname":

                    tutorsIQ = tutorsIQ.OrderBy(t => t.Tutor_firstname);
                    break;

                case "lastname":

                    tutorsIQ = tutorsIQ.OrderBy(t => t.Tutor_lastname);
                    break;

                case "email":

                    tutorsIQ = tutorsIQ.OrderBy(t => t.TutorEmail);
                    break;

                case "phone":

                    tutorsIQ = tutorsIQ.OrderBy(t => t.Tutor_phone);
                    break;

                case "countryOfBirth":

                    tutorsIQ = tutorsIQ.OrderBy(t => t.CountryOfBirth);
                    break;

                default:

                    tutorsIQ= tutorsIQ.OrderBy(t => t.Tutor_lastname);
                    break;

            }

            // Fetch distinct country values dynamically
            var countryQuery = _context.Tutors
                .OrderBy(t => t.CountryOfBirth)
                .Select(t => t.CountryOfBirth)
                .Distinct();

            CountryOptions = new SelectList(await countryQuery.ToListAsync());

            // Define the base query for tutors
           // var query = tutorsIQ;

            // Apply search filter
            if (!string.IsNullOrEmpty(SearchString))
            {
                tutorsIQ = tutorsIQ.Where(t =>
                    //t.Tutor_username.Contains(SearchString) ||
                    t.Tutor_firstname.Contains(SearchString) ||
                    t.Tutor_lastname.Contains(SearchString) ||
                    t.TutorEmail.Contains(SearchString));
            }

            // Apply country filter
            if (!string.IsNullOrEmpty(SelectedCountry))
            {
                tutorsIQ = tutorsIQ.Where(t => t.CountryOfBirth == SelectedCountry);
            }

            int pageSize = 10;

            // Fetch tutors
            Tutors = await PaginatedList<Tutor>.CreateAsync(tutorsIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
        }
    }
}
