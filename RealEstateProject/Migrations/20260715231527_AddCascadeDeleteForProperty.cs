using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateProject.Migrations
{
    /// <inheritdoc />
    public partial class AddCascadeDeleteForProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Properties_PropId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Media_Properties_PropId",
                table: "Media");

            migrationBuilder.DropForeignKey(
                name: "FK_Promotions_Properties_PropId",
                table: "Promotions");

            migrationBuilder.DropForeignKey(
                name: "FK_PropertyApprovals_Properties_PropId",
                table: "PropertyApprovals");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Properties_PropId",
                table: "Bookings",
                column: "PropId",
                principalTable: "Properties",
                principalColumn: "PropId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Media_Properties_PropId",
                table: "Media",
                column: "PropId",
                principalTable: "Properties",
                principalColumn: "PropId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Promotions_Properties_PropId",
                table: "Promotions",
                column: "PropId",
                principalTable: "Properties",
                principalColumn: "PropId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyApprovals_Properties_PropId",
                table: "PropertyApprovals",
                column: "PropId",
                principalTable: "Properties",
                principalColumn: "PropId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Properties_PropId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Media_Properties_PropId",
                table: "Media");

            migrationBuilder.DropForeignKey(
                name: "FK_Promotions_Properties_PropId",
                table: "Promotions");

            migrationBuilder.DropForeignKey(
                name: "FK_PropertyApprovals_Properties_PropId",
                table: "PropertyApprovals");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Properties_PropId",
                table: "Bookings",
                column: "PropId",
                principalTable: "Properties",
                principalColumn: "PropId");

            migrationBuilder.AddForeignKey(
                name: "FK_Media_Properties_PropId",
                table: "Media",
                column: "PropId",
                principalTable: "Properties",
                principalColumn: "PropId");

            migrationBuilder.AddForeignKey(
                name: "FK_Promotions_Properties_PropId",
                table: "Promotions",
                column: "PropId",
                principalTable: "Properties",
                principalColumn: "PropId");

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyApprovals_Properties_PropId",
                table: "PropertyApprovals",
                column: "PropId",
                principalTable: "Properties",
                principalColumn: "PropId");
        }
    }
}
