using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_vendamode.Migrations
{
    /// <inheritdoc />
    public partial class InitMedia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductImages_Products_EntityId",
                table: "ProductImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductImages",
                table: "ProductImages");

            migrationBuilder.RenameTable(
                name: "ProductImages",
                newName: "EntityImage<Guid, Product>");

            migrationBuilder.RenameIndex(
                name: "IX_ProductImages_EntityId",
                table: "EntityImage<Guid, Product>",
                newName: "IX_EntityImage<Guid, Product>_EntityId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EntityImage<Guid, Product>",
                table: "EntityImage<Guid, Product>",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EntityImage<Guid, Product>_Products_EntityId",
                table: "EntityImage<Guid, Product>",
                column: "EntityId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntityImage<Guid, Product>_Products_EntityId",
                table: "EntityImage<Guid, Product>");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EntityImage<Guid, Product>",
                table: "EntityImage<Guid, Product>");

            migrationBuilder.RenameTable(
                name: "EntityImage<Guid, Product>",
                newName: "ProductImages");

            migrationBuilder.RenameIndex(
                name: "IX_EntityImage<Guid, Product>_EntityId",
                table: "ProductImages",
                newName: "IX_ProductImages_EntityId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductImages",
                table: "ProductImages",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImages_Products_EntityId",
                table: "ProductImages",
                column: "EntityId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
