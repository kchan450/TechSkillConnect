using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TechSkillConnect.Models;

namespace TechSkillConnect.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Tutor> Tutors { get; set; }
        public DbSet<TutorProfile> TutorProfiles { get; set; }
        public DbSet<Learner> Learners { get; set; }
        public DbSet<Connection> Connections { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TutorOnboarding> TutorOnboardings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Tutor>()
                .Property(t => t.Tutor_registration_date)
                .HasDefaultValueSql("GETUTCDATE()");

        }
    }
}
