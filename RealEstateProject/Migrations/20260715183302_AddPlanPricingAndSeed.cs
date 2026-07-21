using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RealEstateProject.Migrations
{
    /// <inheritdoc />
    public partial class AddPlanPricingAndSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MonthlyPrice",
                table: "SubscriptionPlans",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.InsertData(
                table: "SubscriptionPlans",
                columns: new[] { "PlanId", "AllowedUnitTypes", "HasAccountManager", "HasFeaturedSearch", "HasMonthlyReports", "MaxMediaCount", "MaxUnits", "MonthlyPrice", "PlanName" },
                values: new object[,]
                {
                    { 1, "Apartment", false, false, false, 5, 3, 299m, "Basic" },
                    { 2, "Apartment,Villa,Studio", false, true, true, 15, 15, 799m, "Pro" },
                    { 3, "All Types", true, true, true, 999999, 999999, 1999m, "Enterprise" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SubscriptionPlans",
                keyColumn: "PlanId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "SubscriptionPlans",
                keyColumn: "PlanId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "SubscriptionPlans",
                keyColumn: "PlanId",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "MonthlyPrice",
                table: "SubscriptionPlans");
        }
    }
}
