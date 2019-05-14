using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class init1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "REWARD_DATA",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    SwapCur = table.Column<string>(nullable: true),
                    BaseCur = table.Column<string>(nullable: true),
                    Reward = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REWARD_DATA", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SYMBOL_DATA",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    SymbolStr = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SYMBOL_DATA", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TRADE_DATA",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    Price = table.Column<double>(nullable: false),
                    Amount = table.Column<double>(nullable: false),
                    Symbol = table.Column<string>(nullable: true),
                    SwapCur = table.Column<string>(nullable: true),
                    BaseCur = table.Column<string>(nullable: true),
                    Side = table.Column<string>(nullable: true),
                    TradeId = table.Column<long>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TRADE_DATA", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TRADE_INFO_PER_MIN",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    Symbol = table.Column<string>(nullable: true),
                    SwapCur = table.Column<string>(nullable: true),
                    BaseCur = table.Column<string>(nullable: true),
                    AvgPrice = table.Column<double>(nullable: false),
                    MaxPrice = table.Column<double>(nullable: false),
                    MinPrice = table.Column<double>(nullable: false),
                    Gmt8DataTime = table.Column<string>(nullable: true),
                    TotalTradeBase = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TRADE_INFO_PER_MIN", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "REWARD_DATA");

            migrationBuilder.DropTable(
                name: "SYMBOL_DATA");

            migrationBuilder.DropTable(
                name: "TRADE_DATA");

            migrationBuilder.DropTable(
                name: "TRADE_INFO_PER_MIN");
        }
    }
}
