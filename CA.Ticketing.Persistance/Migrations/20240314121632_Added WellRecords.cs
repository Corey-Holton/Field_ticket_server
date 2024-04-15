using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CA.Ticketing.Persistance.Migrations
{
    public partial class AddedWellRecords : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WellRecords",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FieldTicketId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WellRecordType = table.Column<int>(type: "int", nullable: false),
                    Pump_Number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pulled = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ran = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Size = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WellRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WellRecords_FieldTickets_FieldTicketId",
                        column: x => x.FieldTicketId,
                        principalTable: "FieldTickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WellRecords_FieldTicketId",
                table: "WellRecords",
                column: "FieldTicketId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WellRecords");
        }
    }
}
