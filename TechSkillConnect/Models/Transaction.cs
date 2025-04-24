using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechSkillConnect.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionID { get; set; }

        //sherry
        [StringLength(450)]
        public string IdentityID { get; set; }

        [ForeignKey("Tutor")]
        [Required(ErrorMessage = "Tutor is required.")]
        public int TutorID { get; set; }

        [Required(ErrorMessage = "Payment ID is required.")]
        [Display(Name = "Payment ID")]

        // Use GUID for PaymentID
        public string PaymentID { get; set; } // GUID as string

        [Required(ErrorMessage = "Subscription Fee is required.")]
        [Range(0, 10000, ErrorMessage = "Subscription Fee must be between $0 and $10,000.")]
        [Display(Name = "Subscription Fee")]
        public int Sub_Fee { get; set; }

        [Required]
        [Display(Name = "Payment Timestamp")]
        public DateTime Payment_timestamp { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "Subscription Start Date is required.")]
        [Display(Name = "Subscription Start Date")]
        public DateTime Sub_start_date { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "Subscription End Date is required.")]
        [Display(Name = "Subscription End Date")]
        public DateTime Sub_end_date { get; set; }

        [Required(ErrorMessage = "Subscription Status is required.")]
        [StringLength(20, ErrorMessage = "Subscription Status cannot exceed 20 characters.")]
        [Display(Name = "Subscription Status")]
        public string Sub_status { get; set; }

        public virtual Tutor Tutor { get; set; }

        // Constructor to auto-set values
        public Transaction()
        {
            Sub_end_date = Sub_start_date.AddDays(30);

            Sub_status = DateTime.UtcNow >= Sub_start_date && DateTime.UtcNow <= Sub_end_date
                ? "Active"
                : "Inactive";
        }
    }
}
