using System;
using System.ComponentModel.DataAnnotations;

namespace TechSkillConnect.ViewModels
{
    public class TutorEditViewModel
    {
        public int TutorID { get; set; }

        public string UserID { get; set; } = default!;

        //        [Required]
        //        [Display(Name = "Username")]
        //        public string Tutor_username { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string Tutor_firstname { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string Tutor_lastname { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string TutorEmail { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        public string Tutor_phone { get; set; }

        [Required]
        [Display(Name = "Country of Birth")]
        public string CountryOfBirth { get; set; }

        [Display(Name = "Registration Date")]
        public DateTime Tutor_registration_date { get; set; }
    }
}
