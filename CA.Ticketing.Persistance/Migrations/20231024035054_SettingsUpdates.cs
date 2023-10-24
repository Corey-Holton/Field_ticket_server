using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CA.Ticketing.Persistance.Migrations
{
    public partial class SettingsUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FuelCalculationMultiplier",
                table: "Settings");

            migrationBuilder.AlterColumn<double>(
                name: "MileageCost",
                table: "Settings",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "MileageCost",
                table: "Settings",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<double>(
                name: "FuelCalculationMultiplier",
                table: "Settings",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
