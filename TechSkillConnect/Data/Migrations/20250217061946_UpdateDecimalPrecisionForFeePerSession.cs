using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechSkillConnect.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDecimalPrecisionForFeePerSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "FeePerSession",
                table: "TutorProfiles",
                type: "decimal(18,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "FeePerSession",
                table: "TutorProfiles",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)");
        }
    }
}
