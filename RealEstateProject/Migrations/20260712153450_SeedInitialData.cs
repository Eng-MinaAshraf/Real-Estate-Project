using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RealEstateProject.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Email", "Fname", "Lname", "Password", "Phone", "Role" },
                values: new object[,]
                {
                    { 1, "ahmed.hassan@example.com", "Ahmed", "Hassan", "Pass@123", "01012345678", "Owner" },
                    { 2, "sara.mostafa@example.com", "Sara", "Mostafa", "Pass@123", "01098765432", "Owner" },
                    { 3, "omar.ali@example.com", "Omar", "Ali", "Pass@123", "01055554444", "Tenant" },
                    { 4, "nour.ibrahim@example.com", "Nour", "Ibrahim", "Pass@123", "01033332222", "Tenant" },
                    { 5, "khaled.mahmoud@example.com", "Khaled", "Mahmoud", "Pass@123", "01011112222", "Admin" }
                });

            migrationBuilder.InsertData(
                table: "Admins",
                column: "AdminId",
                value: 5);

            migrationBuilder.InsertData(
                table: "Owners",
                column: "OwnerId",
                values: new object[]
                {
                    1,
                    2
                });

            migrationBuilder.InsertData(
                table: "Tenants",
                columns: new[] { "TenantId", "Occupation", "Personality", "SmokingStatus" },
                values: new object[,]
                {
                    { 3, "Engineer", "Quiet", "Non-Smoker" },
                    { 4, "Teacher", "Social", "Smoker" }
                });

            migrationBuilder.InsertData(
                table: "Properties",
                columns: new[] { "PropId", "Conditions", "ListingStatus", "Location", "OwnerId", "Price", "PropType", "PublishStatus", "Purpose" },
                values: new object[,]
                {
                    { 1, "Furnished", "Available", "Nasr City, Cairo", 1, 8500m, "Apartment", "Published", "Rent" },
                    { 2, "Semi-Finished", "Available", "New Cairo", 1, 4500000m, "Villa", "Published", "Sale" },
                    { 3, "Furnished", "Rented", "Maadi, Cairo", 2, 5000m, "Studio", "Published", "Rent" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Admins",
                keyColumn: "AdminId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Properties",
                keyColumn: "PropId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Properties",
                keyColumn: "PropId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Properties",
                keyColumn: "PropId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Tenants",
                keyColumn: "TenantId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Tenants",
                keyColumn: "TenantId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Owners",
                keyColumn: "OwnerId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Owners",
                keyColumn: "OwnerId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2);
        }
    }
}
