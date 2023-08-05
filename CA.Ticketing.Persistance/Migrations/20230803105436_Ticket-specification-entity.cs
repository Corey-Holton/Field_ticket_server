using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CA.Ticketing.Persistance.Migrations
{
    public partial class Ticketspecificationentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TicketSpecification",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    EquipmentChargeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketSpecification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketSpecification_EquipmentCharges_EquipmentChargeId",
                        column: x => x.EquipmentChargeId,
                        principalTable: "EquipmentCharges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TicketSpecification_EquipmentChargeId",
                table: "TicketSpecification",
                column: "EquipmentChargeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TicketSpecification");
        }
    }
}
