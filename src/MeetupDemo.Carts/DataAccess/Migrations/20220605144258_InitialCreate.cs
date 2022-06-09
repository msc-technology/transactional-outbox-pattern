using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetupDemo.Carts.DataAccess.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerAvailabilityFunds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    Currency = table.Column<string>(type: "VARCHAR(3)", maxLength: 3, nullable: false),
                    AvailabilityFund = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerAvailabilityFunds", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAvailabilityFunds_CustomerId_Currency",
                table: "CustomerAvailabilityFunds",
                columns: new[] { "CustomerId", "Currency" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerAvailabilityFunds");
        }
    }
}
