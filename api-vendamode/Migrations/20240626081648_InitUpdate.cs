using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_vendamode.Migrations
{
    /// <inheritdoc />
    public partial class InitUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductSizeValues_ProductSizes_ProductSizeId",
                table: "ProductSizeValues");

            migrationBuilder.DropIndex(
                name: "IX_ProductSizeValues_ProductSizeId",
                table: "ProductSizeValues");

            migrationBuilder.CreateTable(
                name: "ProductSizeProductSizeValues",
                columns: table => new
                {
                    ProductSizeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductSizeValuesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSizeProductSizeValues", x => new { x.ProductSizeId, x.ProductSizeValuesId });
                    table.ForeignKey(
                        name: "FK_ProductSizeProductSizeValues_ProductSizeValues_ProductSizeV~",
                        column: x => x.ProductSizeValuesId,
                        principalTable: "ProductSizeValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductSizeProductSizeValues_ProductSizes_ProductSizeId",
                        column: x => x.ProductSizeId,
                        principalTable: "ProductSizes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductSizeProductSizeValues_ProductSizeValuesId",
                table: "ProductSizeProductSizeValues",
                column: "ProductSizeValuesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductSizeProductSizeValues");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSizeValues_ProductSizeId",
                table: "ProductSizeValues",
                column: "ProductSizeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSizeValues_ProductSizes_ProductSizeId",
                table: "ProductSizeValues",
                column: "ProductSizeId",
                principalTable: "ProductSizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
