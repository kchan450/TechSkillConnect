namespace TechSkillConnect.Models
{
    public class LearnerProfileViewModel
    {
        public int LearnerID { get; set; }
        public string Learner_firstname { get; set; }
        public string Learner_lastname { get; set; }
        public string LearnerEmail { get; set; }
        public string CountryOfBirth { get; set; }
        public DateTime Learner_registration_date { get; set; }

        // You can add other fields as needed, like a profile picture
    }
}
