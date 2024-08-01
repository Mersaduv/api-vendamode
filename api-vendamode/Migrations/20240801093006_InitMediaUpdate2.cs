using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_vendamode.Migrations
{
    /// <inheritdoc />
    public partial class InitMediaUpdate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "Descriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Descriptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MediaImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    Placeholder = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MediaImages_Descriptions_EntityId",
                        column: x => x.EntityId,
                        principalTable: "Descriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MediaImages_EntityId",
                table: "MediaImages",
                column: "EntityId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImages_Products_EntityId",
                table: "ProductImages",
                column: "EntityId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductImages_Products_EntityId",
                table: "ProductImages");

            migrationBuilder.DropTable(
                name: "MediaImages");

            migrationBuilder.DropTable(
                name: "Descriptions");

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
    }
}
