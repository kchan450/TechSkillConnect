using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TechSkillConnect.Data;
using TechSkillConnect.Models;
using System.Security.Claims;

namespace TechSkillConnect.Pages.Admin.Transactions
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;

        public IndexModel(ApplicationDbContext context)
        {
            _dbContext = context ?? throw new ArgumentNullException(nameof(context));
        }

     //   public IList<Transaction> TransactionsList { get; private set; } = new List<Transaction>();


        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SelectedSubscriptionStatus { get; set; }

        public SelectList? SubscriptionStatusOptions { get; set; }

        public string PaymentIDSort { get; set; }
        public string PaymentTimestampSort { get; set; }
        public string SubFeeSort { get; set; }
        public string SubStatusSort { get; set; }
        public string SubStartDateSort { get; set; }
        public string SubEndDateSort { get; set; }
        public string TutorEmailSort { get; set; }
        public string CurrentSort { get; set; }

        public string CurrentFilter { get; set; }

        public PaginatedList<Transaction> Transactions { get; set; }
        public async Task OnGetAsync(string sortOrder, string currentFilter, string SearchString, int? pageIndex)
        {

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != "b8bb4c3d-1383-489d-b070-7205c81f623b") //admin user id example for account t1@t.com
            {
                Response.Redirect("/Identity/Account/Login");
                return;
            }

            CurrentSort = sortOrder;
            PaymentIDSort = sortOrder == "paymentID" ? "paymentID_desc" : "paymentID";
            PaymentTimestampSort = sortOrder == "paymentTimestamp" ? "paymentTimestamp_desc" : "paymentTimestamp";
            SubFeeSort = sortOrder == "subFee" ? "subFee_desc" : "subFee";
            SubStatusSort = sortOrder == "subStatus" ? "subStatus_desc" : "subStatus";
            SubStartDateSort = sortOrder == "subStartDate" ? "subStartDate_desc" : "subStartDate";
            SubEndDateSort = sortOrder == "subEndDate" ? "subEndDate_desc" : "subEndDate";
            TutorEmailSort = sortOrder == "tutorEmail" ? "tutorEmail_desc" : "tutorEmail";

            if (SearchString != null)
            {
                pageIndex = 1;

            }
            else
            {
                SearchString = currentFilter;
            }
            currentFilter = SearchString;
 
            //IQueryable<Transaction> transactionIQ = from t in _dbContext.Transactions
            //                                        select t;

            IQueryable<Transaction> transactionIQ = _dbContext.Transactions.Include(t => t.Tutor); // Eagerly load the Tutor related data


            switch (sortOrder)
            {
                case "paymentID":
                    transactionIQ = transactionIQ.OrderBy(t => t.PaymentID);
                    break;
                case "paymentID_desc":
                    transactionIQ = transactionIQ.OrderByDescending(t => t.PaymentID);
                    break;
                case "paymentTimestamp":
                    transactionIQ = transactionIQ.OrderBy(t => t.Payment_timestamp);
                    break;
                case "paymentTimestamp_desc":
                    transactionIQ = transactionIQ.OrderByDescending(t => t.Payment_timestamp);
                    break;
                case "subFee":
                    transactionIQ = transactionIQ.OrderBy(t => t.Sub_Fee);
                    break;
                case "subFee_desc":
                    transactionIQ = transactionIQ.OrderByDescending(t => t.Sub_Fee);
                    break;
                case "subStatus":
                    transactionIQ = transactionIQ.OrderBy(t => t.Sub_status);
                    break;
                case "subStatus_desc":
                    transactionIQ = transactionIQ.OrderByDescending(t => t.Sub_status);
                    break;
                case "subStartDate":
                    transactionIQ = transactionIQ.OrderBy(t => t.Sub_start_date);
                    break;
                case "subStartDate_desc":
                    transactionIQ = transactionIQ.OrderByDescending(t => t.Sub_start_date);
                    break;
                case "subEndDate":
                    transactionIQ = transactionIQ.OrderBy(t => t.Sub_end_date);
                    break;
                case "subEndDate_desc":
                    transactionIQ = transactionIQ.OrderByDescending(t => t.Sub_end_date);
                    break;
                case "tutorEmail":
                    transactionIQ = transactionIQ.OrderBy(t => t.Tutor.TutorEmail);
                    break;
                case "tutorEmail_desc":
                    transactionIQ = transactionIQ.OrderByDescending(t => t.Tutor.TutorEmail);
                    break;
                default:
                    transactionIQ = transactionIQ.OrderByDescending(t => t.Payment_timestamp); // default sort
                    break;
            }


            //TransactionsList = await _dbContext.Transactions
            //    .Include(t => t.Tutor)
            //    .ToListAsync();

            var subscriptionStatusQuery = _dbContext.Transactions
            .OrderBy(t => t.Tutor)
            .Select(t => t.Sub_status)
            .Distinct();

            SubscriptionStatusOptions = new SelectList(await subscriptionStatusQuery.ToListAsync());

            // Define the base query for transactions
            //var query = _dbContext.Transactions.AsQueryable();
            //var query = transactions;

            // Apply search filter
            if (!string.IsNullOrEmpty(SearchString))
            {
                transactionIQ = transactionIQ.Where(t =>
                    t.PaymentID.ToString().Contains(SearchString) ||
                    t.Sub_Fee.ToString().Contains(SearchString) ||
                    t.Sub_status.ToString().Contains(SearchString) ||
                    t.Sub_start_date.ToString().Contains(SearchString) ||
                    t.Sub_end_date.ToString().Contains(SearchString) ||
                    t.Tutor.TutorEmail.Contains(SearchString));
            }

            // Apply Subscription Status filterR
            if (!string.IsNullOrEmpty(SelectedSubscriptionStatus))
            {
                transactionIQ = transactionIQ.Where(t => t.Sub_status == SelectedSubscriptionStatus);
            }

            int pageSize = 10;

            // Fetch Tran
            Transactions = await PaginatedList<Transaction>.CreateAsync(transactionIQ.AsNoTracking(), pageIndex ?? 1, pageSize);

        }
    }
}