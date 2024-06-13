using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_vendamode.Migrations
{
    /// <inheritdoc />
    public partial class InitSliderImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntityImage<Guid, Slider>_Sliders_EntityId",
                table: "EntityImage<Guid, Slider>");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EntityImage<Guid, Slider>",
                table: "EntityImage<Guid, Slider>");

            migrationBuilder.RenameTable(
                name: "EntityImage<Guid, Slider>",
                newName: "SliderImages");

            migrationBuilder.RenameIndex(
                name: "IX_EntityImage<Guid, Slider>_EntityId",
                table: "SliderImages",
                newName: "IX_SliderImages_EntityId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SliderImages",
                table: "SliderImages",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SliderImages_Sliders_EntityId",
                table: "SliderImages",
                column: "EntityId",
                principalTable: "Sliders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SliderImages_Sliders_EntityId",
                table: "SliderImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SliderImages",
                table: "SliderImages");

            migrationBuilder.RenameTable(
                name: "SliderImages",
                newName: "EntityImage<Guid, Slider>");

            migrationBuilder.RenameIndex(
                name: "IX_SliderImages_EntityId",
                table: "EntityImage<Guid, Slider>",
                newName: "IX_EntityImage<Guid, Slider>_EntityId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EntityImage<Guid, Slider>",
                table: "EntityImage<Guid, Slider>",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EntityImage<Guid, Slider>_Sliders_EntityId",
                table: "EntityImage<Guid, Slider>",
                column: "EntityId",
                principalTable: "Sliders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
