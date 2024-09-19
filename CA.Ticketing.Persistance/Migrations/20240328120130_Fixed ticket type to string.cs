using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CA.Ticketing.Persistance.Migrations
{
    public partial class Fixedtickettypetostring : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SwabCups_FieldTickets_FieldTicketId",
                table: "SwabCups");

            migrationBuilder.AddColumn<bool>(
                name: "WellCharge",
                table: "TicketSpecification",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "FieldTicketId",
                table: "SwabCups",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TicketType",
                table: "FieldTickets",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "OtherText",
                table: "FieldTickets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "TicketType",
                table: "Equipment",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_SwabCups_FieldTickets_FieldTicketId",
                table: "SwabCups",
                column: "FieldTicketId",
                principalTable: "FieldTickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SwabCups_FieldTickets_FieldTicketId",
                table: "SwabCups");

            migrationBuilder.DropColumn(
                name: "WellCharge",
                table: "TicketSpecification");

            migrationBuilder.DropColumn(
                name: "OtherText",
                table: "FieldTickets");

            migrationBuilder.AlterColumn<string>(
                name: "FieldTicketId",
                table: "SwabCups",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "TicketType",
                table: "FieldTickets",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "TicketType",
                table: "Equipment",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_SwabCups_FieldTickets_FieldTicketId",
                table: "SwabCups",
                column: "FieldTicketId",
                principalTable: "FieldTickets",
                principalColumn: "Id");
        }
    }
}
