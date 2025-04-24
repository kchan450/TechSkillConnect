using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace TechSkillConnect.Models.ViewModels
{
    public class ConnectionEditViewModel
    {
        public int ConnectionID { get; set; }

        [Required(ErrorMessage = "Tutor is required.")]
        public int TutorID { get; set; }

        [Required(ErrorMessage = "Learner is required.")]
        public int LearnerID { get; set; }

        [Required(ErrorMessage = "Connection Date is required.")]
        [Display(Name = "Connection Date")]
        [DataType(DataType.DateTime)]
        public DateTime ConnectionDate { get; set; } = DateTime.UtcNow;

        // Dropdowns for the form
        public List<SelectListItem> TutorList { get; set; } = new();
        public List<SelectListItem> LearnerList { get; set; } = new();
    }
}
