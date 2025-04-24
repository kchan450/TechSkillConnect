using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechSkillConnect.Data.Migrations
{
    /// <inheritdoc />
    public partial class CU1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TutorProfiles_TutorID",
                table: "TutorProfiles");

            migrationBuilder.DropColumn(
                name: "LearnerPassword",
                table: "Learners");

            migrationBuilder.AddColumn<string>(
                name: "UserID",
                table: "Learners",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TutorProfiles_TutorID",
                table: "TutorProfiles",
                column: "TutorID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TutorProfiles_TutorID",
                table: "TutorProfiles");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "Learners");

            migrationBuilder.AddColumn<string>(
                name: "LearnerPassword",
                table: "Learners",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TutorProfiles_TutorID",
                table: "TutorProfiles",
                column: "TutorID");
        }
    }
}
