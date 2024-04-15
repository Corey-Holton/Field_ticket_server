using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CA.Ticketing.Persistance.Migrations
{
    public partial class specialTypeCharge : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TicketType",
                table: "FieldTickets");

            migrationBuilder.DropColumn(
                name: "TicketType",
                table: "Equipment");

            migrationBuilder.RenameColumn(
                name: "WellCharge",
                table: "TicketSpecification",
                newName: "SpecialCharge");

            migrationBuilder.AddColumn<string>(
                name: "TicketTypeId",
                table: "FieldTickets",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TicketTypeId",
                table: "Equipment",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TicketType",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SpecialTypeCharges",
                columns: table => new
                {
                    ChargeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TicketTypeId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecialTypeCharges", x => new { x.ChargeId, x.TicketTypeId });
                    table.ForeignKey(
                        name: "FK_SpecialCharges_Charge_ChargeId",
                        column: x => x.ChargeId,
                        principalTable: "Charges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SpecialCharges_TicketType_TicketTypeId",
                        column: x => x.TicketTypeId,
                        principalTable: "TicketType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TypeCharges",
                columns: table => new
                {
                    ChargeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TicketTypeId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeCharges", x => new { x.ChargeId, x.TicketTypeId });
                    table.ForeignKey(
                        name: "FK_TypeCharges_Charge_ChargeId",
                        column: x => x.ChargeId,
                        principalTable: "Charges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TypeCharges_TicketType_TicketTypeId",
                        column: x => x.TicketTypeId,
                        principalTable: "TicketType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FieldTickets_TicketTypeId",
                table: "FieldTickets",
                column: "TicketTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_TicketTypeId",
                table: "Equipment",
                column: "TicketTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SpecialTypeCharges_TicketTypeId",
                table: "SpecialTypeCharges",
                column: "TicketTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TypeCharges_TicketTypeId",
                table: "TypeCharges",
                column: "TicketTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipment_TicketType_TicketTypeId",
                table: "Equipment",
                column: "TicketTypeId",
                principalTable: "TicketType",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FieldTickets_TicketType_TicketTypeId",
                table: "FieldTickets",
                column: "TicketTypeId",
                principalTable: "TicketType",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipment_TicketType_TicketTypeId",
                table: "Equipment");

            migrationBuilder.DropForeignKey(
                name: "FK_FieldTickets_TicketType_TicketTypeId",
                table: "FieldTickets");

            migrationBuilder.DropTable(
                name: "SpecialTypeCharges");

            migrationBuilder.DropTable(
                name: "TypeCharges");

            migrationBuilder.DropTable(
                name: "TicketType");

            migrationBuilder.DropIndex(
                name: "IX_FieldTickets_TicketTypeId",
                table: "FieldTickets");

            migrationBuilder.DropIndex(
                name: "IX_Equipment_TicketTypeId",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "TicketTypeId",
                table: "FieldTickets");

            migrationBuilder.DropColumn(
                name: "TicketTypeId",
                table: "Equipment");

            migrationBuilder.RenameColumn(
                name: "SpecialCharge",
                table: "TicketSpecification",
                newName: "WellCharge");

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
    }
}
