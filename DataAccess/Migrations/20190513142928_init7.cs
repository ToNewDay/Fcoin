using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class init7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NeedSell",
                table: "TARGET_ORDER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SellOrderId",
                table: "TARGET_ORDER",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "SwapAmount",
                table: "TARGET_ORDER",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NeedSell",
                table: "TARGET_ORDER");

            migrationBuilder.DropColumn(
                name: "SellOrderId",
                table: "TARGET_ORDER");

            migrationBuilder.DropColumn(
                name: "SwapAmount",
                table: "TARGET_ORDER");
        }
    }
}
