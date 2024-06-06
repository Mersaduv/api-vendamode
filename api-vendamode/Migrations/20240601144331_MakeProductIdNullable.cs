using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_vendamode.Migrations
{
    /// <inheritdoc />
    public partial class MakeProductIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Features_Categories_CategoryId",
                table: "Features");

            migrationBuilder.DropForeignKey(
                name: "FK_Features_Products_ProductId",
                table: "Features");

            migrationBuilder.DropForeignKey(
                name: "FK_FeatureValues_Features_ProductFeatureId",
                table: "FeatureValues");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Features",
                table: "Features");

            migrationBuilder.RenameTable(
                name: "Features",
                newName: "ProductFeatures");

            migrationBuilder.RenameIndex(
                name: "IX_Features_ProductId",
                table: "ProductFeatures",
                newName: "IX_ProductFeatures_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_Features_CategoryId",
                table: "ProductFeatures",
                newName: "IX_ProductFeatures_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductFeatures",
                table: "ProductFeatures",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FeatureValues_ProductFeatures_ProductFeatureId",
                table: "FeatureValues",
                column: "ProductFeatureId",
                principalTable: "ProductFeatures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductFeatures_Categories_CategoryId",
                table: "ProductFeatures",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductFeatures_Products_ProductId",
                table: "ProductFeatures",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FeatureValues_ProductFeatures_ProductFeatureId",
                table: "FeatureValues");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductFeatures_Categories_CategoryId",
                table: "ProductFeatures");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductFeatures_Products_ProductId",
                table: "ProductFeatures");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductFeatures",
                table: "ProductFeatures");

            migrationBuilder.RenameTable(
                name: "ProductFeatures",
                newName: "Features");

            migrationBuilder.RenameIndex(
                name: "IX_ProductFeatures_ProductId",
                table: "Features",
                newName: "IX_Features_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductFeatures_CategoryId",
                table: "Features",
                newName: "IX_Features_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Features",
                table: "Features",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Features_Categories_CategoryId",
                table: "Features",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Features_Products_ProductId",
                table: "Features",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FeatureValues_Features_ProductFeatureId",
                table: "FeatureValues",
                column: "ProductFeatureId",
                principalTable: "Features",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
