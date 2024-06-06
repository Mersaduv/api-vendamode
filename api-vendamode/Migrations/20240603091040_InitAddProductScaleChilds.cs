using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_vendamode.Migrations
{
    /// <inheritdoc />
    public partial class InitAddProductScaleChilds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SizeModel_ProductScales_ProductScaleId",
                table: "SizeModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SizeModel",
                table: "SizeModel");

            migrationBuilder.RenameTable(
                name: "SizeModel",
                newName: "SizeModels");

            migrationBuilder.RenameIndex(
                name: "IX_SizeModel_ProductScaleId",
                table: "SizeModels",
                newName: "IX_SizeModels_ProductScaleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SizeModels",
                table: "SizeModels",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SizeModels_ProductScales_ProductScaleId",
                table: "SizeModels",
                column: "ProductScaleId",
                principalTable: "ProductScales",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SizeModels_ProductScales_ProductScaleId",
                table: "SizeModels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SizeModels",
                table: "SizeModels");

            migrationBuilder.RenameTable(
                name: "SizeModels",
                newName: "SizeModel");

            migrationBuilder.RenameIndex(
                name: "IX_SizeModels_ProductScaleId",
                table: "SizeModel",
                newName: "IX_SizeModel_ProductScaleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SizeModel",
                table: "SizeModel",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SizeModel_ProductScales_ProductScaleId",
                table: "SizeModel",
                column: "ProductScaleId",
                principalTable: "ProductScales",
                principalColumn: "Id");
        }
    }
}
