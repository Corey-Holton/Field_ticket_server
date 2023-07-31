using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CA.Ticketing.Persistance.Migrations
{
    public partial class nullableforeignkey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FieldTickets_Invoices_InvoiceId",
                table: "FieldTickets");

            migrationBuilder.AlterColumn<int>(
                name: "InvoiceId",
                table: "FieldTickets",
                type: "int",
                nullable: true,
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FieldTickets_Invoices_InvoiceId",
                table: "FieldTickets");

            migrationBuilder.AlterColumn<int>(
                name: "InvoiceId",
                table: "FieldTickets",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_FieldTickets_Invoices_InvoiceId",
                table: "FieldTickets",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id");
        }
    }
}
