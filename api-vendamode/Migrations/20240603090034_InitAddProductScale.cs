using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_vendamode.Migrations
{
    /// <inheritdoc />
    public partial class InitAddProductScale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductScale_ProductScaleId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_SizeIds_ProductScale_ProductScaleId",
                table: "SizeIds");

            migrationBuilder.DropForeignKey(
                name: "FK_SizeModel_ProductScale_ProductScaleId",
                table: "SizeModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductScale",
                table: "ProductScale");

            migrationBuilder.RenameTable(
                name: "ProductScale",
                newName: "ProductScales");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductScales",
                table: "ProductScales",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductScales_ProductScaleId",
                table: "Products",
                column: "ProductScaleId",
                principalTable: "ProductScales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SizeIds_ProductScales_ProductScaleId",
                table: "SizeIds",
                column: "ProductScaleId",
                principalTable: "ProductScales",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SizeModel_ProductScales_ProductScaleId",
                table: "SizeModel",
                column: "ProductScaleId",
                principalTable: "ProductScales",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductScales_ProductScaleId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_SizeIds_ProductScales_ProductScaleId",
                table: "SizeIds");

            migrationBuilder.DropForeignKey(
                name: "FK_SizeModel_ProductScales_ProductScaleId",
                table: "SizeModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductScales",
                table: "ProductScales");

            migrationBuilder.RenameTable(
                name: "ProductScales",
                newName: "ProductScale");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductScale",
                table: "ProductScale",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductScale_ProductScaleId",
                table: "Products",
                column: "ProductScaleId",
                principalTable: "ProductScale",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SizeIds_ProductScale_ProductScaleId",
                table: "SizeIds",
                column: "ProductScaleId",
                principalTable: "ProductScale",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SizeModel_ProductScale_ProductScaleId",
                table: "SizeModel",
                column: "ProductScaleId",
                principalTable: "ProductScale",
                principalColumn: "Id");
        }
    }
}
