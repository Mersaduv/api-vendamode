using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_vendamode.Migrations
{
    /// <inheritdoc />
    public partial class InitAddPropertiesInProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ProductSizeValueId",
                table: "SizeModel",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "SizeModel",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "ProductSizeValueName",
                table: "SizeModel",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "SizeIds",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<List<Guid>>(
                name: "FeatureValueIds",
                table: "Products",
                type: "uuid[]",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductSizeValueName",
                table: "SizeModel");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "SizeIds");

            migrationBuilder.DropColumn(
                name: "FeatureValueIds",
                table: "Products");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProductSizeValueId",
                table: "SizeModel",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "SizeModel",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
