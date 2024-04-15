using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CA.Ticketing.Persistance.Migrations
{
    public partial class FixTicketType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Equipment");

            migrationBuilder.AddColumn<string>(
                name: "TicketType",
                table: "FieldTickets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TicketType",
                table: "Equipment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TicketType",
                table: "FieldTickets");

            migrationBuilder.DropColumn(
                name: "TicketType",
                table: "Equipment");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Equipment",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
