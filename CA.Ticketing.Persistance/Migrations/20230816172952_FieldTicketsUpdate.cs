using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CA.Ticketing.Persistance.Migrations
{
    public partial class FieldTicketsUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PayrollData_FieldTickets_FieldTicketId",
                table: "PayrollData");

            migrationBuilder.DropColumn(
                name: "Total",
                table: "TicketSpecification");

            migrationBuilder.DropColumn(
                name: "RigHours",
                table: "PayrollData");

            migrationBuilder.DropColumn(
                name: "RoustaboutHours",
                table: "PayrollData");

            migrationBuilder.DropColumn(
                name: "TravelHours",
                table: "PayrollData");

            migrationBuilder.DropColumn(
                name: "HasCustomerSignature",
                table: "FieldTickets");

            migrationBuilder.DropColumn(
                name: "WellType",
                table: "FieldTickets");

            migrationBuilder.AlterColumn<double>(
                name: "Quantity",
                table: "TicketSpecification",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "AllowRateAdjustment",
                table: "TicketSpecification",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowUoMChange",
                table: "TicketSpecification",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "FieldTicketId",
                table: "PayrollData",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "FieldTickets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "AutoCalculated",
                table: "Charges",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollData_FieldTickets_FieldTicketId",
                table: "PayrollData",
                column: "FieldTicketId",
                principalTable: "FieldTickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PayrollData_FieldTickets_FieldTicketId",
                table: "PayrollData");

            migrationBuilder.DropColumn(
                name: "AllowRateAdjustment",
                table: "TicketSpecification");

            migrationBuilder.DropColumn(
                name: "AllowUoMChange",
                table: "TicketSpecification");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "FieldTickets");

            migrationBuilder.DropColumn(
                name: "AutoCalculated",
                table: "Charges");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "TicketSpecification",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<double>(
                name: "Total",
                table: "TicketSpecification",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AlterColumn<int>(
                name: "FieldTicketId",
                table: "PayrollData",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<double>(
                name: "RigHours",
                table: "PayrollData",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "RoustaboutHours",
                table: "PayrollData",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TravelHours",
                table: "PayrollData",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "HasCustomerSignature",
                table: "FieldTickets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "WellType",
                table: "FieldTickets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollData_FieldTickets_FieldTicketId",
                table: "PayrollData",
                column: "FieldTicketId",
                principalTable: "FieldTickets",
                principalColumn: "Id");
        }
    }
}
