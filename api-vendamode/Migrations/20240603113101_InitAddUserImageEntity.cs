using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_vendamode.Migrations
{
    /// <inheritdoc />
    public partial class InitAddUserImageEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserImages_UserSpecifications_UserSpecificationId",
                table: "UserImages");

            migrationBuilder.DropIndex(
                name: "IX_UserImages_UserSpecificationId",
                table: "UserImages");

            migrationBuilder.DropColumn(
                name: "UserSpecificationId",
                table: "UserImages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserSpecificationId",
                table: "UserImages",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserImages_UserSpecificationId",
                table: "UserImages",
                column: "UserSpecificationId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserImages_UserSpecifications_UserSpecificationId",
                table: "UserImages",
                column: "UserSpecificationId",
                principalTable: "UserSpecifications",
                principalColumn: "Id");
        }
    }
}
