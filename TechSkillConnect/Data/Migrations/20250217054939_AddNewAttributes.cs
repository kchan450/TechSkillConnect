using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechSkillConnect.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNewAttributes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Certificate",
                table: "TutorProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "FeePerSession",
                table: "TutorProfiles",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "SelfHeadline",
                table: "TutorProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SelfIntro",
                table: "TutorProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Certificate",
                table: "TutorProfiles");

            migrationBuilder.DropColumn(
                name: "FeePerSession",
                table: "TutorProfiles");

            migrationBuilder.DropColumn(
                name: "SelfHeadline",
                table: "TutorProfiles");

            migrationBuilder.DropColumn(
                name: "SelfIntro",
                table: "TutorProfiles");
        }
    }
}
