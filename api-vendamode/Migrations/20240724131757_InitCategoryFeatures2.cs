using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_vendamode.Migrations
{
    /// <inheritdoc />
    public partial class InitCategoryFeatures2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductFeatures_Categories_CategoryId",
                table: "ProductFeatures");

            migrationBuilder.DropIndex(
                name: "IX_ProductFeatures_CategoryId",
                table: "ProductFeatures");

            migrationBuilder.CreateTable(
                name: "CategoryProductFeatures",
                columns: table => new
                {
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductFeatureId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryProductFeatures", x => new { x.CategoryId, x.ProductFeatureId });
                    table.ForeignKey(
                        name: "FK_CategoryProductFeatures_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryProductFeatures_ProductFeatures_ProductFeatureId",
                        column: x => x.ProductFeatureId,
                        principalTable: "ProductFeatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryProductFeatures_ProductFeatureId",
                table: "CategoryProductFeatures",
                column: "ProductFeatureId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryProductFeatures");

            migrationBuilder.CreateIndex(
                name: "IX_ProductFeatures_CategoryId",
                table: "ProductFeatures",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductFeatures_Categories_CategoryId",
                table: "ProductFeatures",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");
        }
    }
}
