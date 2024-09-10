using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_vendamode.Migrations
{
    /// <inheritdoc />
    public partial class InitFooterArticleColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColumnFooterArticles",
                table: "ColumnFooters");

            migrationBuilder.CreateTable(
                name: "FooterArticleColumns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ArticleId = table.Column<Guid>(type: "uuid", nullable: false),
                    Index = table.Column<int>(type: "integer", nullable: false),
                    ColumnFooterId = table.Column<Guid>(type: "uuid", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FooterArticleColumns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FooterArticleColumns_ColumnFooters_ColumnFooterId",
                        column: x => x.ColumnFooterId,
                        principalTable: "ColumnFooters",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FooterArticleColumns_ColumnFooterId",
                table: "FooterArticleColumns",
                column: "ColumnFooterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FooterArticleColumns");

            migrationBuilder.AddColumn<string>(
                name: "ColumnFooterArticles",
                table: "ColumnFooters",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
