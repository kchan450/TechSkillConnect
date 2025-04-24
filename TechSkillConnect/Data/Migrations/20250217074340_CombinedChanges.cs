using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechSkillConnect.Data.Migrations
{
    /// <inheritdoc />
    public partial class CombinedChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "TutorProfiles");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "TutorProfiles",
                newName: "TutorName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TutorName",
                table: "TutorProfiles",
                newName: "LastName");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "TutorProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
