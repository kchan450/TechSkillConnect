using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechSkillConnect.Models
{
    public class Learner
    {
        [Key]
        public int LearnerID { get; set; }

        [Required]
        public string UserID { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        [Display(Name = "First Name")]
        [StringLength(50, ErrorMessage = "First Name cannot be longer than 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "First Name can only contain letters and spaces.")]
        public string Learner_firstname { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        [Display(Name = "Last Name")]
        [StringLength(50, ErrorMessage = "Last Name cannot be longer than 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Last Name can only contain letters and spaces.")]
        public string Learner_lastname { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [Display(Name = "Learner Email")]
        public string LearnerEmail { get; set; }


        [Required(ErrorMessage = "Country of Birth is required.")]
        [Display(Name = "Country Of Birth")]
        [StringLength(50, ErrorMessage = "Country of Birth cannot be longer than 50 characters.")]
        public string CountryOfBirth { get; set; }

        [Required]
        [Display(Name = "Registration Date")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime Learner_registration_date { get; set; } = DateTime.UtcNow;
    }
}
