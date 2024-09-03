using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_vendamode.Migrations
{
    /// <inheritdoc />
    public partial class InitUpdateSR : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Copyright",
                table: "Supports",
                newName: "Address");

            migrationBuilder.AddColumn<string>(
                name: "Copyright",
                table: "Redirects",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Copyright",
                table: "Redirects");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Supports",
                newName: "Copyright");
        }
    }
}
