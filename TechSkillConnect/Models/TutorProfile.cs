using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechSkillConnect.Models
{
    public class TutorProfile
    {
        [Key]
        public int ProfileID { get; set; }

        [ForeignKey("Tutor")]
        public int TutorID { get; set; }

        [Required(ErrorMessage = "Skills is required.")]
        [StringLength(50, ErrorMessage = "Language cannot exceed 50 characters.")]
        public string Language { get; set; }

        [Required(ErrorMessage = "Years of experience is required.")]
        // [Range(0, 50, ErrorMessage = "Years of experience must be between 0 and 50.")]
        [Display(Name = "Year Of Experience")]
        public string YearsOfExperience { get; set; }


        [Required(ErrorMessage = "Skill Level is required.")]
        [StringLength(20, ErrorMessage = "Skill Level cannot exceed 20 characters.")]
        [Display(Name = "Skill Level")]
        public string SkillLevel { get; set; }

        [StringLength(100, ErrorMessage = "Certificate name cannot exceed 100 characters.")]
  //      public string? Certificate { get; set; }

        [Required(ErrorMessage = "Fee per session is required.")]
        [Range(0, 1000, ErrorMessage = "Fee per session must be between 0 and 1000.")]
        [Display(Name = "Fee Per Session")]
        public int FeePerSession { get; set; }

        [StringLength(500, ErrorMessage = "Self Introduction cannot exceed 500 characters.")]
        [Display(Name = "Self Intro")]
        public string? SelfIntro { get; set; }

        [StringLength(100, ErrorMessage = "Self Headline cannot exceed 100 characters.")]
        [Display(Name = "Self Headline")]
        public string? SelfHeadline { get; set; }

        public virtual Tutor? Tutor { get; set; }
    }
}
