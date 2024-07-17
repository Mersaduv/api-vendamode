using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_vendamode.Migrations
{
    /// <inheritdoc />
    public partial class InitUpdateCategorySize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SizeIds_ProductScales_ProductScaleId",
                table: "SizeIds");

            migrationBuilder.DropForeignKey(
                name: "FK_SizeModels_ProductScales_ProductScaleId",
                table: "SizeModels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategorySizes",
                table: "CategorySizes");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProductScaleId",
                table: "SizeModels",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ProductScaleId",
                table: "SizeIds",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategorySizes",
                table: "CategorySizes",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CategorySizes_CategoryId",
                table: "CategorySizes",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_SizeIds_ProductScales_ProductScaleId",
                table: "SizeIds",
                column: "ProductScaleId",
                principalTable: "ProductScales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SizeModels_ProductScales_ProductScaleId",
                table: "SizeModels",
                column: "ProductScaleId",
                principalTable: "ProductScales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SizeIds_ProductScales_ProductScaleId",
                table: "SizeIds");

            migrationBuilder.DropForeignKey(
                name: "FK_SizeModels_ProductScales_ProductScaleId",
                table: "SizeModels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategorySizes",
                table: "CategorySizes");

            migrationBuilder.DropIndex(
                name: "IX_CategorySizes_CategoryId",
                table: "CategorySizes");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProductScaleId",
                table: "SizeModels",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProductScaleId",
                table: "SizeIds",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategorySizes",
                table: "CategorySizes",
                columns: new[] { "CategoryId", "SizeId" });

            migrationBuilder.AddForeignKey(
                name: "FK_SizeIds_ProductScales_ProductScaleId",
                table: "SizeIds",
                column: "ProductScaleId",
                principalTable: "ProductScales",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SizeModels_ProductScales_ProductScaleId",
                table: "SizeModels",
                column: "ProductScaleId",
                principalTable: "ProductScales",
                principalColumn: "Id");
        }
    }
}
