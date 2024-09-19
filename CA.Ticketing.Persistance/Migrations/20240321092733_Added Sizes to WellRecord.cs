using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CA.Ticketing.Persistance.Migrations
{
    public partial class AddedSizestoWellRecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Size",
                table: "WellRecords",
                newName: "SizeW");

            migrationBuilder.AddColumn<string>(
                name: "SizeH",
                table: "WellRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SizeL",
                table: "WellRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SizeH",
                table: "WellRecords");

            migrationBuilder.DropColumn(
                name: "SizeL",
                table: "WellRecords");

            migrationBuilder.RenameColumn(
                name: "SizeW",
                table: "WellRecords",
                newName: "Size");
        }
    }
}
