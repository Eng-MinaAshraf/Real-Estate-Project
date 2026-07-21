using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateProject.Migrations
{
    /// <inheritdoc />
    public partial class ReshapeAlertForPropertySearch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TargetType",
                table: "Alerts",
                newName: "Purpose");

            migrationBuilder.RenameColumn(
                name: "TargetPrice",
                table: "Alerts",
                newName: "MinPrice");

            migrationBuilder.RenameColumn(
                name: "TargetLocation",
                table: "Alerts",
                newName: "PropertyType");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Alerts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Alerts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Alerts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "MaxPrice",
                table: "Alerts",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Alerts");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Alerts");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Alerts");

            migrationBuilder.DropColumn(
                name: "MaxPrice",
                table: "Alerts");

            migrationBuilder.RenameColumn(
                name: "Purpose",
                table: "Alerts",
                newName: "TargetType");

            migrationBuilder.RenameColumn(
                name: "PropertyType",
                table: "Alerts",
                newName: "TargetLocation");

            migrationBuilder.RenameColumn(
                name: "MinPrice",
                table: "Alerts",
                newName: "TargetPrice");
        }
    }
}
