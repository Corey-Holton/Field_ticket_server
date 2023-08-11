using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CA.Ticketing.Persistance.Migrations
{
    public partial class Scheduling : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LocationX",
                table: "Scheduling");

            migrationBuilder.DropColumn(
                name: "LocationY",
                table: "Scheduling");

            migrationBuilder.RenameColumn(
                name: "Duration",
                table: "Scheduling",
                newName: "StartTime");

            migrationBuilder.RenameColumn(
                name: "ArrangeDateTime",
                table: "Scheduling",
                newName: "EndTime");

            migrationBuilder.AddColumn<int>(
                name: "CustomerLocationId",
                table: "Scheduling",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Scheduling_CustomerLocationId",
                table: "Scheduling",
                column: "CustomerLocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Scheduling_CustomerLocations_CustomerLocationId",
                table: "Scheduling",
                column: "CustomerLocationId",
                principalTable: "CustomerLocations",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Scheduling_CustomerLocations_CustomerLocationId",
                table: "Scheduling");

            migrationBuilder.DropIndex(
                name: "IX_Scheduling_CustomerLocationId",
                table: "Scheduling");

            migrationBuilder.DropColumn(
                name: "CustomerLocationId",
                table: "Scheduling");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "Scheduling",
                newName: "Duration");

            migrationBuilder.RenameColumn(
                name: "EndTime",
                table: "Scheduling",
                newName: "ArrangeDateTime");

            migrationBuilder.AddColumn<double>(
                name: "LocationX",
                table: "Scheduling",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "LocationY",
                table: "Scheduling",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
