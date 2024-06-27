using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_vendamode.Migrations
{
    /// <inheritdoc />
    public partial class InitUpdate3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductSizeId",
                table: "ProductSizeValues");

            migrationBuilder.RenameColumn(
                name: "ProductSizeValuesId",
                table: "ProductSizeProductSizeValues",
                newName: "ProductSizeValueId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductSizeProductSizeValues_ProductSizeValuesId",
                table: "ProductSizeProductSizeValues",
                newName: "IX_ProductSizeProductSizeValues_ProductSizeValueId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSizeValues_Name",
                table: "ProductSizeValues",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductSizeValues_Name",
                table: "ProductSizeValues");

            migrationBuilder.RenameColumn(
                name: "ProductSizeValueId",
                table: "ProductSizeProductSizeValues",
                newName: "ProductSizeValuesId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductSizeProductSizeValues_ProductSizeValueId",
                table: "ProductSizeProductSizeValues",
                newName: "IX_ProductSizeProductSizeValues_ProductSizeValuesId");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductSizeId",
                table: "ProductSizeValues",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
