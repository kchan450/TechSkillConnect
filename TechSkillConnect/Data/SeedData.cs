using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TechSkillConnect.Models; // Ensure this namespace includes your models
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TechSkillConnect.Data
{
    public class SeedData
    {
public static async Task InitializeAsync(IServiceProvider serviceProvider)
{
    using (var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
    {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var tutors = await SeedTutorsAsync(context); // ✅ This now returns a List<Tutor>
                        if (tutors.Any()) // ✅ Check if tutors list is populated
                        {
                            await SeedTutorProfilesAsync(context, tutors); // ✅ Pass the correct tutors list
                        }

                        await SeedLearnersAsync(context);
                        await SeedConnectionsAsync(context);
                        await SeedTransactionsAsync(context);

                        await context.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        Console.WriteLine($"An error occurred while seeding the database: {ex.Message}");
                    }
                }

            }
        }

        private static async Task<List<Tutor>> SeedTutorsAsync(ApplicationDbContext context)
        {
            if (!await context.Tutors.AnyAsync()) // Check if Tutors table is empty
            {
                var tutors = new List<Tutor>
        {
            new Tutor
            {
                //Tutor_username = "john_doe",
                Tutor_firstname = "John",
                Tutor_lastname = "Doe",
                TutorEmail = "johndoe@example.com",
                Tutor_phone = "123-456-7890",
                CountryOfBirth = "USA",
                Tutor_registration_date = DateTime.UtcNow
            },
            new Tutor
            {
                //Tutor_username = "jane_smith",
                Tutor_firstname = "Jane",
                Tutor_lastname = "Smith",
                TutorEmail = "janesmith@example.com",
                Tutor_phone = "987-654-3210",
                CountryOfBirth = "Canada",
                Tutor_registration_date = DateTime.UtcNow
            }
        };

                await context.Tutors.AddRangeAsync(tutors);
                await context.SaveChangesAsync();

                return tutors;  // ✅ Ensure that it returns the list of tutors
            }

            return await context.Tutors.ToListAsync(); // ✅ Return existing tutors if they already exist
        }


        private static async Task SeedTutorProfilesAsync(ApplicationDbContext context, List<Tutor> tutors)
        {
            if (!await context.TutorProfiles.AnyAsync())
            {
                var profiles = new List<TutorProfile>
        {
            new TutorProfile { TutorID = tutors[0].TutorID, Language = "English", YearsOfExperience = "4-5", SkillLevel = "Advanced", Certificate = "TESOL Certified", FeePerSession = 50, SelfIntro = "Experienced English teacher with over 5 years of experience.", SelfHeadline = "Dynamic and engaging instructor!" },
            new TutorProfile { TutorID = tutors[1].TutorID, Language = "French", YearsOfExperience = "2-3", SkillLevel = "Intermediate", Certificate = "DELF B2", FeePerSession = 40, SelfIntro = "Passionate about teaching French to speakers of other languages.", SelfHeadline = "Fun and effective learning!" }
        };

                await context.TutorProfiles.AddRangeAsync(profiles);
                await context.SaveChangesAsync();
            }
        }



        private static async Task SeedLearnersAsync(ApplicationDbContext context)
        {
            if (!await context.Learners.AnyAsync())
            {
                var learners = new List<Learner>
        {
            new Learner
            {
                Learner_firstname = "Alice",
                Learner_lastname = "Johnson",
                LearnerEmail = "alice@example.com",
                CountryOfBirth = "UK",
                Learner_registration_date = DateTime.UtcNow
            },
            new Learner
            {
                Learner_firstname = "Bob",
                Learner_lastname = "Lee",
                LearnerEmail = "bob@example.com",
                CountryOfBirth = "USA",
                Learner_registration_date = DateTime.UtcNow
            }
        };

                await context.Learners.AddRangeAsync(learners);
                await context.SaveChangesAsync();
            }
        }


        private static async Task SeedConnectionsAsync(ApplicationDbContext context)
        {
            if (!await context.Connections.AnyAsync()) // ✅ Only seed if table is empty
            {
                var tutors = await context.Tutors.ToListAsync();
                var learners = await context.Learners.ToListAsync();

                if (tutors.Any() && learners.Any())
                {
                    var connections = new List<Connection>();

                    // ✅ Ensure that each tutor gets at least one connection with a learner
                    foreach (var tutor in tutors)
                    {
                        var randomLearner = learners.OrderBy(l => Guid.NewGuid()).FirstOrDefault(); // Randomly assign learners
                        if (randomLearner != null)
                        {
                            connections.Add(new Connection
                            {
                                TutorID = tutor.TutorID,
                                LearnerID = randomLearner.LearnerID,
                                ConnectionDate = DateTime.UtcNow.AddDays(-new Random().Next(1, 30)) // ✅ Assign a random past date
                            });
                        }
                    }

                    await context.Connections.AddRangeAsync(connections);
                    await context.SaveChangesAsync();
                }
            }
        }


        private static async Task SeedTransactionsAsync(ApplicationDbContext context)
        {
            if (!await context.Transactions.AnyAsync()) // ✅ Only seed if the table is empty
            {
                var tutors = await context.Tutors.ToListAsync();
                var random = new Random();

                if (tutors.Any())
                {
                    var transactions = new List<Transaction>();

                    foreach (var tutor in tutors)
                    {
                        var subStartDate = DateTime.UtcNow.AddDays(-random.Next(1, 30)); // ✅ Random past subscription start date
                        var subEndDate = subStartDate.AddDays(30); // ✅ Auto-calculate end date (+30 days)
                        var subFee = random.Next(40, 100); // ✅ Random subscription fee ($40-$100)
                        var subStatus = DateTime.UtcNow >= subStartDate && DateTime.UtcNow <= subEndDate ? "Active" : "Inactive"; // ✅ Auto-determine status

                        transactions.Add(new Transaction
                        {
                            TutorID = tutor.TutorID,
                            PaymentID = Guid.NewGuid().ToString(), // ✅ Generate a random PaymentID (since it's not a foreign key)
                            Sub_Fee = subFee,
                            Payment_timestamp = subStartDate,
                            Sub_start_date = subStartDate,
                            Sub_end_date = subEndDate,
                            Sub_status = subStatus
                        });
                    }

                    await context.Transactions.AddRangeAsync(transactions);
                    await context.SaveChangesAsync();
                }
            }
        }

    }
}
