using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace TechSkillConnect.Models
{
    public class TutorOnboarding
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public IdentityUser User { get; set; }

        // ✅ New Fields
        [MaxLength(100)]
        public string FirstName { get; set; }

        [MaxLength(100)]
        public string LastName { get; set; }

        [MaxLength(100)]
        public string CountryOfBirth { get; set; }

        [MaxLength(50)]
        public string TutorPhone { get; set; }

        // Onboarding status
        public bool IsProfileComplete { get; set; } = false;
        public bool HasPaidFee { get; set; } = false;

        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ProfileCompletedAt { get; set; }
        public DateTime? PaymentConfirmedAt { get; set; }
    }
}
