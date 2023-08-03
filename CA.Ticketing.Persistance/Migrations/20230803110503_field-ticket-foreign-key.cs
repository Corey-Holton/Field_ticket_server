using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CA.Ticketing.Persistance.Migrations
{
    public partial class fieldticketforeignkey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FieldTicketId",
                table: "TicketSpecification",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TicketSpecification_FieldTicketId",
                table: "TicketSpecification",
                column: "FieldTicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketSpecification_FieldTickets_FieldTicketId",
                table: "TicketSpecification",
                column: "FieldTicketId",
                principalTable: "FieldTickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketSpecification_FieldTickets_FieldTicketId",
                table: "TicketSpecification");

            migrationBuilder.DropIndex(
                name: "IX_TicketSpecification_FieldTicketId",
                table: "TicketSpecification");

            migrationBuilder.DropColumn(
                name: "FieldTicketId",
                table: "TicketSpecification");
        }
    }
}
