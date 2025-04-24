using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechSkillConnect.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Payments_PaymentID",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_TutorProfiles_TutorID",
                table: "TutorProfiles");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_PaymentID",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Tutors");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Tutors");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Tutors");

            migrationBuilder.DropColumn(
                name: "LearnerName",
                table: "Learners");

            migrationBuilder.RenameColumn(
                name: "TransactionFee",
                table: "Transactions",
                newName: "Sub_Fee");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Transactions",
                newName: "Sub_status");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Transactions",
                newName: "Sub_start_date");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "Transactions",
                newName: "Sub_end_date");

            migrationBuilder.AlterColumn<string>(
                name: "TutorPassword",
                table: "Tutors",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Tutor_firstname",
                table: "Tutors",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tutor_lastname",
                table: "Tutors",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tutor_phone",
                table: "Tutors",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "Tutor_registration_date",
                table: "Tutors",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<string>(
                name: "Tutor_username",
                table: "Tutors",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "Payment_timestamp",
                table: "Transactions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "LearnerPassword",
                table: "Learners",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Learner_firstname",
                table: "Learners",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Learner_lastname",
                table: "Learners",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "Learner_registration_date",
                table: "Learners",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_TutorProfiles_TutorID",
                table: "TutorProfiles",
                column: "TutorID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TutorProfiles_TutorID",
                table: "TutorProfiles");

            migrationBuilder.DropColumn(
                name: "Tutor_firstname",
                table: "Tutors");

            migrationBuilder.DropColumn(
                name: "Tutor_lastname",
                table: "Tutors");

            migrationBuilder.DropColumn(
                name: "Tutor_phone",
                table: "Tutors");

            migrationBuilder.DropColumn(
                name: "Tutor_registration_date",
                table: "Tutors");

            migrationBuilder.DropColumn(
                name: "Tutor_username",
                table: "Tutors");

            migrationBuilder.DropColumn(
                name: "Payment_timestamp",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "Learner_firstname",
                table: "Learners");

            migrationBuilder.DropColumn(
                name: "Learner_lastname",
                table: "Learners");

            migrationBuilder.DropColumn(
                name: "Learner_registration_date",
                table: "Learners");

            migrationBuilder.RenameColumn(
                name: "Sub_status",
                table: "Transactions",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "Sub_start_date",
                table: "Transactions",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "Sub_end_date",
                table: "Transactions",
                newName: "EndDate");

            migrationBuilder.RenameColumn(
                name: "Sub_Fee",
                table: "Transactions",
                newName: "TransactionFee");

            migrationBuilder.AlterColumn<string>(
                name: "TutorPassword",
                table: "Tutors",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Tutors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Tutors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Tutors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "LearnerPassword",
                table: "Learners",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "LearnerName",
                table: "Learners",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    PaymentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    PaymentStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.PaymentID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TutorProfiles_TutorID",
                table: "TutorProfiles",
                column: "TutorID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_PaymentID",
                table: "Transactions",
                column: "PaymentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Payments_PaymentID",
                table: "Transactions",
                column: "PaymentID",
                principalTable: "Payments",
                principalColumn: "PaymentID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
