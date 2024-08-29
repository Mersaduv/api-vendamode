using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_vendamode.Migrations
{
    /// <inheritdoc />
    public partial class InitUpdateLogoImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntityImage<Guid, LogoImages>_LogoImages_EntityId",
                table: "EntityImage<Guid, LogoImages>");

            migrationBuilder.DropForeignKey(
                name: "FK_LogoImages_EntityImage<Guid, LogoImages>_FaviconThumbnailId",
                table: "LogoImages");

            migrationBuilder.DropIndex(
                name: "IX_LogoImages_FaviconThumbnailId",
                table: "LogoImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EntityImage<Guid, LogoImages>",
                table: "EntityImage<Guid, LogoImages>");

            migrationBuilder.DropColumn(
                name: "FaviconThumbnailId",
                table: "LogoImages");

            migrationBuilder.RenameTable(
                name: "EntityImage<Guid, LogoImages>",
                newName: "FaviconThumbnailImages");

            migrationBuilder.RenameIndex(
                name: "IX_EntityImage<Guid, LogoImages>_EntityId",
                table: "FaviconThumbnailImages",
                newName: "IX_FaviconThumbnailImages_EntityId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FaviconThumbnailImages",
                table: "FaviconThumbnailImages",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "OrgThumbnailImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    Placeholder = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrgThumbnailImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrgThumbnailImages_LogoImages_EntityId",
                        column: x => x.EntityId,
                        principalTable: "LogoImages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrgThumbnailImages_EntityId",
                table: "OrgThumbnailImages",
                column: "EntityId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FaviconThumbnailImages_LogoImages_EntityId",
                table: "FaviconThumbnailImages",
                column: "EntityId",
                principalTable: "LogoImages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FaviconThumbnailImages_LogoImages_EntityId",
                table: "FaviconThumbnailImages");

            migrationBuilder.DropTable(
                name: "OrgThumbnailImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FaviconThumbnailImages",
                table: "FaviconThumbnailImages");

            migrationBuilder.RenameTable(
                name: "FaviconThumbnailImages",
                newName: "EntityImage<Guid, LogoImages>");

            migrationBuilder.RenameIndex(
                name: "IX_FaviconThumbnailImages_EntityId",
                table: "EntityImage<Guid, LogoImages>",
                newName: "IX_EntityImage<Guid, LogoImages>_EntityId");

            migrationBuilder.AddColumn<Guid>(
                name: "FaviconThumbnailId",
                table: "LogoImages",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_EntityImage<Guid, LogoImages>",
                table: "EntityImage<Guid, LogoImages>",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_LogoImages_FaviconThumbnailId",
                table: "LogoImages",
                column: "FaviconThumbnailId");

            migrationBuilder.AddForeignKey(
                name: "FK_EntityImage<Guid, LogoImages>_LogoImages_EntityId",
                table: "EntityImage<Guid, LogoImages>",
                column: "EntityId",
                principalTable: "LogoImages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LogoImages_EntityImage<Guid, LogoImages>_FaviconThumbnailId",
                table: "LogoImages",
                column: "FaviconThumbnailId",
                principalTable: "EntityImage<Guid, LogoImages>",
                principalColumn: "Id");
        }
    }
}
