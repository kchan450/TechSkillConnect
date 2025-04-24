using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechSkillConnect.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAllModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Payment",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "SubscriptionID",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "TutorID",
                table: "Payment");

            migrationBuilder.RenameTable(
                name: "Payment",
                newName: "Payments");

            migrationBuilder.RenameColumn(
                name: "PaymentAmount",
                table: "Payments",
                newName: "Amount");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentType",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PaymentStatus",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payments",
                table: "Payments",
                column: "PaymentID");

            migrationBuilder.CreateTable(
                name: "Learners",
                columns: table => new
                {
                    LearnerID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LearnerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LearnerEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LearnerPassword = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CountryOfBirth = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Learners", x => x.LearnerID);
                });

            migrationBuilder.CreateTable(
                name: "Tutors",
                columns: table => new
                {
                    TutorID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TutorName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TutorEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TutorPassword = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CountryOfBirth = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tutors", x => x.TutorID);
                });

            migrationBuilder.CreateTable(
                name: "Connections",
                columns: table => new
                {
                    ConnectionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TutorID = table.Column<int>(type: "int", nullable: false),
                    LearnerID = table.Column<int>(type: "int", nullable: false),
                    ConnectionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Connections", x => x.ConnectionID);
                    table.ForeignKey(
                        name: "FK_Connections_Learners_LearnerID",
                        column: x => x.LearnerID,
                        principalTable: "Learners",
                        principalColumn: "LearnerID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Connections_Tutors_TutorID",
                        column: x => x.TutorID,
                        principalTable: "Tutors",
                        principalColumn: "TutorID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TransactionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TutorID = table.Column<int>(type: "int", nullable: false),
                    PaymentID = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.TransactionID);
                    table.ForeignKey(
                        name: "FK_Transactions_Payments_PaymentID",
                        column: x => x.PaymentID,
                        principalTable: "Payments",
                        principalColumn: "PaymentID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transactions_Tutors_TutorID",
                        column: x => x.TutorID,
                        principalTable: "Tutors",
                        principalColumn: "TutorID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TutorProfiles",
                columns: table => new
                {
                    ProfileID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TutorID = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    YearsOfExperience = table.Column<int>(type: "int", nullable: false),
                    SkillLevel = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TutorProfiles", x => x.ProfileID);
                    table.ForeignKey(
                        name: "FK_TutorProfiles_Tutors_TutorID",
                        column: x => x.TutorID,
                        principalTable: "Tutors",
                        principalColumn: "TutorID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Connections_LearnerID",
                table: "Connections",
                column: "LearnerID");

            migrationBuilder.CreateIndex(
                name: "IX_Connections_TutorID",
                table: "Connections",
                column: "TutorID");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_PaymentID",
                table: "Transactions",
                column: "PaymentID");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TutorID",
                table: "Transactions",
                column: "TutorID");

            migrationBuilder.CreateIndex(
                name: "IX_TutorProfiles_TutorID",
                table: "TutorProfiles",
                column: "TutorID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Connections");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "TutorProfiles");

            migrationBuilder.DropTable(
                name: "Learners");

            migrationBuilder.DropTable(
                name: "Tutors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payments",
                table: "Payments");

            migrationBuilder.RenameTable(
                name: "Payments",
                newName: "Payment");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "Payment",
                newName: "PaymentAmount");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentType",
                table: "Payment",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentStatus",
                table: "Payment",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "SubscriptionID",
                table: "Payment",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TutorID",
                table: "Payment",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payment",
                table: "Payment",
                column: "PaymentID");
        }
    }
}
