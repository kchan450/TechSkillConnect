using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechSkillConnect.Models
{
    public class Tutor
    {
        [Key]
        public int TutorID { get; set; }

        [StringLength(450)]
        public string IdentityID { get; set; }

        //[Required(ErrorMessage = "Username is required.")]
        //[StringLength(30, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 30 characters.")]
        //[RegularExpression(@"^[a-zA-Z0-9_]*$", ErrorMessage = "Username can only contain letters, numbers, and underscores.")]
        //[Display(Name = "Tutor Username")]
        //public string Tutor_username { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        [StringLength(50, ErrorMessage = "First Name cannot be longer than 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "First Name can only contain letters and spaces.")]
        [Display(Name = "Tutor First Name")]
        public string Tutor_firstname { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        [StringLength(50, ErrorMessage = "Last Name cannot be longer than 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Last Name can only contain letters and spaces.")]
        [Display(Name = "Tutor Last Name")]
        public string Tutor_lastname { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [Display(Name = "Tutor Email")]
        public string TutorEmail { get; set; }

        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [StringLength(15, MinimumLength = 7, ErrorMessage = "Phone number must be between 7 and 15 digits.")]
        [Display(Name = "Tutor Phone")]
        public string Tutor_phone { get; set; } // Removed [Required]

        [Required(ErrorMessage = "Country of Birth is required.")]
        [StringLength(50, ErrorMessage = "Country of Birth cannot be longer than 50 characters.")]
        [Display(Name = "Country Of Birth")]
        public string CountryOfBirth { get; set; }

        [Required]
        [Display(Name = "Tutor Registration Date")]
        public DateTime Tutor_registration_date { get; set; } = DateTime.UtcNow; // ✅ Auto-generated on creation

        public TutorProfile TutorProfile { get; set; }
    }
}
