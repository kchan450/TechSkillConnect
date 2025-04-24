using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechSkillConnect.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPersonalFieldsToTutorOnboarding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CountryOfBirth",
                table: "TutorOnboardings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "TutorOnboardings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "TutorOnboardings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TutorPhone",
                table: "TutorOnboardings",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountryOfBirth",
                table: "TutorOnboardings");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "TutorOnboardings");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "TutorOnboardings");

            migrationBuilder.DropColumn(
                name: "TutorPhone",
                table: "TutorOnboardings");
        }
    }
}
