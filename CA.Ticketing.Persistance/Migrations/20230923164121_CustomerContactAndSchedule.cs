using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CA.Ticketing.Persistance.Migrations
{
    public partial class CustomerContactAndSchedule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerContacts_CustomerLocations_CustomerLocationId",
                table: "CustomerContacts");

            migrationBuilder.DropIndex(
                name: "IX_CustomerContacts_CustomerLocationId",
                table: "CustomerContacts");

            migrationBuilder.DropColumn(
                name: "CustomerLocationId",
                table: "CustomerContacts");

            migrationBuilder.AddColumn<string>(
                name: "CustomerContactId",
                table: "Scheduling",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JobTitle",
                table: "CustomerContacts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Scheduling_CustomerContactId",
                table: "Scheduling",
                column: "CustomerContactId");

            migrationBuilder.AddForeignKey(
                name: "FK_Scheduling_CustomerContacts_CustomerContactId",
                table: "Scheduling",
                column: "CustomerContactId",
                principalTable: "CustomerContacts",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Scheduling_CustomerContacts_CustomerContactId",
                table: "Scheduling");

            migrationBuilder.DropIndex(
                name: "IX_Scheduling_CustomerContactId",
                table: "Scheduling");

            migrationBuilder.DropColumn(
                name: "CustomerContactId",
                table: "Scheduling");

            migrationBuilder.DropColumn(
                name: "JobTitle",
                table: "CustomerContacts");

            migrationBuilder.AddColumn<string>(
                name: "CustomerLocationId",
                table: "CustomerContacts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerContacts_CustomerLocationId",
                table: "CustomerContacts",
                column: "CustomerLocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerContacts_CustomerLocations_CustomerLocationId",
                table: "CustomerContacts",
                column: "CustomerLocationId",
                principalTable: "CustomerLocations",
                principalColumn: "Id");
        }
    }
}
