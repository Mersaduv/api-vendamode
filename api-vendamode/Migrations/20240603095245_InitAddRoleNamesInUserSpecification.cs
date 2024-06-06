using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_vendamode.Migrations
{
    /// <inheritdoc />
    public partial class InitAddRoleNamesInUserSpecification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "UserSpecifications");

            migrationBuilder.AddColumn<List<string>>(
                name: "Roles",
                table: "UserSpecifications",
                type: "text[]",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Roles",
                table: "UserSpecifications");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "UserSpecifications",
                type: "text",
                nullable: true);
        }
    }
}
