using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CA.Ticketing.Persistance.Migrations
{
    public partial class fieldticketupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "FieldTickets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Signature",
                table: "FieldTickets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_FieldTickets_LocationId",
                table: "FieldTickets",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_FieldTickets_CustomerLocations_LocationId",
                table: "FieldTickets",
                column: "LocationId",
                principalTable: "CustomerLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FieldTickets_CustomerLocations_LocationId",
                table: "FieldTickets");

            migrationBuilder.DropIndex(
                name: "IX_FieldTickets_LocationId",
                table: "FieldTickets");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "FieldTickets");

            migrationBuilder.DropColumn(
                name: "Signature",
                table: "FieldTickets");
        }
    }
}
