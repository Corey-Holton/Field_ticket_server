using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CA.Ticketing.Persistance.Migrations
{
    public partial class ServiceTypesMultiple : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ServiceTypesSelection",
                table: "FieldTickets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceTypesSelection",
                table: "FieldTickets");
        }
    }
}
