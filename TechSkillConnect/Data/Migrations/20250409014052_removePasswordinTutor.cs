using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechSkillConnect.Data.Migrations
{
    /// <inheritdoc />
    public partial class removePasswordinTutor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TutorPassword",
                table: "Tutors");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TutorPassword",
                table: "Tutors",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
