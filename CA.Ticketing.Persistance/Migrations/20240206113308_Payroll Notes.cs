using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CA.Ticketing.Persistance.Migrations
{
    public partial class PayrollNotes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasNote",
                table: "PayrollData");

            migrationBuilder.AddColumn<string>(
                name: "EmployeeNoteId",
                table: "PayrollData",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TicketId",
                table: "EmployeeNotes",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EmployeeId",
                table: "EmployeeNotes",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollData_EmployeeNoteId",
                table: "PayrollData",
                column: "EmployeeNoteId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeNotes_EmployeeId",
                table: "EmployeeNotes",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeNotes_TicketId",
                table: "EmployeeNotes",
                column: "TicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeNotes_Employees_EmployeeId",
                table: "EmployeeNotes",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeNotes_FieldTickets_TicketId",
                table: "EmployeeNotes",
                column: "TicketId",
                principalTable: "FieldTickets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollData_EmployeeNotes_EmployeeNoteId",
                table: "PayrollData",
                column: "EmployeeNoteId",
                principalTable: "EmployeeNotes",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeNotes_Employees_EmployeeId",
                table: "EmployeeNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeNotes_FieldTickets_TicketId",
                table: "EmployeeNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_PayrollData_EmployeeNotes_EmployeeNoteId",
                table: "PayrollData");

            migrationBuilder.DropIndex(
                name: "IX_PayrollData_EmployeeNoteId",
                table: "PayrollData");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeNotes_EmployeeId",
                table: "EmployeeNotes");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeNotes_TicketId",
                table: "EmployeeNotes");

            migrationBuilder.DropColumn(
                name: "EmployeeNoteId",
                table: "PayrollData");

            migrationBuilder.AddColumn<bool>(
                name: "HasNote",
                table: "PayrollData",
                type: "bit",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TicketId",
                table: "EmployeeNotes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EmployeeId",
                table: "EmployeeNotes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
