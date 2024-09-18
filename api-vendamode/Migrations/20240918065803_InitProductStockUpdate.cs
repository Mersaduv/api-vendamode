using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_vendamode.Migrations
{
    /// <inheritdoc />
    public partial class InitProductStockUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductImages_StockItems_StockItemId",
                table: "ProductImages");

            migrationBuilder.DropIndex(
                name: "IX_ProductImages_StockItemId",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "StockItemId",
                table: "ProductImages");

            migrationBuilder.CreateTable(
                name: "StockImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    Placeholder = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockImages_StockItems_EntityId",
                        column: x => x.EntityId,
                        principalTable: "StockItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StockImages_EntityId",
                table: "StockImages",
                column: "EntityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockImages");

            migrationBuilder.AddColumn<Guid>(
                name: "StockItemId",
                table: "ProductImages",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_StockItemId",
                table: "ProductImages",
                column: "StockItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImages_StockItems_StockItemId",
                table: "ProductImages",
                column: "StockItemId",
                principalTable: "StockItems",
                principalColumn: "Id");
        }
    }
}
