using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CA.Ticketing.Persistance.Migrations
{
    public partial class schedulingmigrationfixanother : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Scheduling_Equipment_RigId",
                table: "Scheduling");

            migrationBuilder.RenameColumn(
                name: "locationY",
                table: "Scheduling",
                newName: "LocationY");

            migrationBuilder.RenameColumn(
                name: "locationX",
                table: "Scheduling",
                newName: "LocationX");

            migrationBuilder.RenameColumn(
                name: "RigId",
                table: "Scheduling",
                newName: "EquipmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Scheduling_RigId",
                table: "Scheduling",
                newName: "IX_Scheduling_EquipmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Scheduling_Equipment_EquipmentId",
                table: "Scheduling",
                column: "EquipmentId",
                principalTable: "Equipment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Scheduling_Equipment_EquipmentId",
                table: "Scheduling");

            migrationBuilder.RenameColumn(
                name: "LocationY",
                table: "Scheduling",
                newName: "locationY");

            migrationBuilder.RenameColumn(
                name: "LocationX",
                table: "Scheduling",
                newName: "locationX");

            migrationBuilder.RenameColumn(
                name: "EquipmentId",
                table: "Scheduling",
                newName: "RigId");

            migrationBuilder.RenameIndex(
                name: "IX_Scheduling_EquipmentId",
                table: "Scheduling",
                newName: "IX_Scheduling_RigId");

            migrationBuilder.AddForeignKey(
                name: "FK_Scheduling_Equipment_RigId",
                table: "Scheduling",
                column: "RigId",
                principalTable: "Equipment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
