using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CA.Ticketing.Persistance.Migrations
{
    public partial class PayrollData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileIndicatorGenerated",
                table: "FieldTickets",
                newName: "SignedBy");

            migrationBuilder.RenameColumn(
                name: "FileIndicatorCustomer",
                table: "FieldTickets",
                newName: "EmployeeSignature");

            migrationBuilder.AddColumn<double>(
                name: "RoustaboutHours",
                table: "PayrollData",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "YardHours",
                table: "PayrollData",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "CustomerPrintedName",
                table: "FieldTickets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerSignedBy",
                table: "FieldTickets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CustomerSignedOn",
                table: "FieldTickets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmployeePrintedName",
                table: "FieldTickets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "SignedOn",
                table: "FieldTickets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Signature",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoustaboutHours",
                table: "PayrollData");

            migrationBuilder.DropColumn(
                name: "YardHours",
                table: "PayrollData");

            migrationBuilder.DropColumn(
                name: "CustomerPrintedName",
                table: "FieldTickets");

            migrationBuilder.DropColumn(
                name: "CustomerSignedBy",
                table: "FieldTickets");

            migrationBuilder.DropColumn(
                name: "CustomerSignedOn",
                table: "FieldTickets");

            migrationBuilder.DropColumn(
                name: "EmployeePrintedName",
                table: "FieldTickets");

            migrationBuilder.DropColumn(
                name: "SignedOn",
                table: "FieldTickets");

            migrationBuilder.DropColumn(
                name: "Signature",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "SignedBy",
                table: "FieldTickets",
                newName: "FileIndicatorGenerated");

            migrationBuilder.RenameColumn(
                name: "EmployeeSignature",
                table: "FieldTickets",
                newName: "FileIndicatorCustomer");
        }
    }
}
