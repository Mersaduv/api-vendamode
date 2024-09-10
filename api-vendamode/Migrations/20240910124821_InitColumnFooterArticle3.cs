using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_vendamode.Migrations
{
    /// <inheritdoc />
    public partial class InitColumnFooterArticle3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArticleIds",
                table: "ColumnFooters");

            migrationBuilder.CreateTable(
                name: "ColumnFooterArticle",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Index = table.Column<int>(type: "integer", nullable: false),
                    ColumnFooterId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColumnFooterArticle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ColumnFooterArticle_ColumnFooters_ColumnFooterId",
                        column: x => x.ColumnFooterId,
                        principalTable: "ColumnFooters",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ColumnFooterArticle_ColumnFooterId",
                table: "ColumnFooterArticle",
                column: "ColumnFooterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ColumnFooterArticle");

            migrationBuilder.AddColumn<List<Guid>>(
                name: "ArticleIds",
                table: "ColumnFooters",
                type: "uuid[]",
                nullable: false);
        }
    }
}
