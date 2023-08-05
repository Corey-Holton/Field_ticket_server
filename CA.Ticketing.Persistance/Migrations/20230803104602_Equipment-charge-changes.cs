using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CA.Ticketing.Persistance.Migrations
{
    public partial class Equipmentchargechanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FieldTickets_Invoices_InvoiceId",
                table: "FieldTickets");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "EquipmentCharges",
                newName: "Rate");

            migrationBuilder.AlterColumn<int>(
                name: "InvoiceId",
                table: "FieldTickets",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "ItemName",
                table: "EquipmentCharges",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_FieldTickets_Invoices_InvoiceId",
                table: "FieldTickets",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FieldTickets_Invoices_InvoiceId",
                table: "FieldTickets");

            migrationBuilder.DropColumn(
                name: "ItemName",
                table: "EquipmentCharges");

            migrationBuilder.RenameColumn(
                name: "Rate",
                table: "EquipmentCharges",
                newName: "Value");

            migrationBuilder.AlterColumn<int>(
                name: "InvoiceId",
                table: "FieldTickets",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FieldTickets_Invoices_InvoiceId",
                table: "FieldTickets",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
