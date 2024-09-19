using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_vendamode.Migrations
{
    /// <inheritdoc />
    public partial class InitDeleteStockTagIntoStockItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StockTag",
                table: "StockItems");

            migrationBuilder.AddColumn<string>(
                name: "StockTag",
                table: "Products",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StockTag",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "StockTag",
                table: "StockItems",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
