using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_vendamode.Migrations
{
    /// <inheritdoc />
    public partial class InitUpdateUSerSpecifications2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BussinessLicenseNumber",
                table: "UserSpecifications",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CommissionType",
                table: "UserSpecifications",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActiveAddProduct",
                table: "UserSpecifications",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublishProduct",
                table: "UserSpecifications",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSelectedAsSpecialSeller",
                table: "UserSpecifications",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "NoReturns",
                table: "UserSpecifications",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PercentageValue",
                table: "UserSpecifications",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SellerPerformance",
                table: "UserSpecifications",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShippingCommitment",
                table: "UserSpecifications",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StoreAddress",
                table: "UserSpecifications",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StoreName",
                table: "UserSpecifications",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StoreTelephone",
                table: "UserSpecifications",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TimelySupply",
                table: "UserSpecifications",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BussinessLicenseNumber",
                table: "UserSpecifications");

            migrationBuilder.DropColumn(
                name: "CommissionType",
                table: "UserSpecifications");

            migrationBuilder.DropColumn(
                name: "IsActiveAddProduct",
                table: "UserSpecifications");

            migrationBuilder.DropColumn(
                name: "IsPublishProduct",
                table: "UserSpecifications");

            migrationBuilder.DropColumn(
                name: "IsSelectedAsSpecialSeller",
                table: "UserSpecifications");

            migrationBuilder.DropColumn(
                name: "NoReturns",
                table: "UserSpecifications");

            migrationBuilder.DropColumn(
                name: "PercentageValue",
                table: "UserSpecifications");

            migrationBuilder.DropColumn(
                name: "SellerPerformance",
                table: "UserSpecifications");

            migrationBuilder.DropColumn(
                name: "ShippingCommitment",
                table: "UserSpecifications");

            migrationBuilder.DropColumn(
                name: "StoreAddress",
                table: "UserSpecifications");

            migrationBuilder.DropColumn(
                name: "StoreName",
                table: "UserSpecifications");

            migrationBuilder.DropColumn(
                name: "StoreTelephone",
                table: "UserSpecifications");

            migrationBuilder.DropColumn(
                name: "TimelySupply",
                table: "UserSpecifications");
        }
    }
}
