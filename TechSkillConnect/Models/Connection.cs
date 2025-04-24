using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TechSkillConnect.Models
{
    public class Connection
    {
        [Key]
        public int ConnectionID { get; set; }

        [Required(ErrorMessage = "Tutor is required.")]
        [ForeignKey("Tutor")]
        public int TutorID { get; set; }

        [Required(ErrorMessage = "Learner is required.")]
        [ForeignKey("Learner")]
        public int LearnerID { get; set; }

        [Required(ErrorMessage = "Connection Date is required.")]
        [Display(Name = "Connection Date")]
        [DataType(DataType.DateTime)]
        public DateTime ConnectionDate { get; set; } = DateTime.UtcNow;


        [BindNever]
        public virtual Tutor Tutor { get; set; }

        [BindNever]
        public virtual Learner Learner { get; set; }


    }
}
