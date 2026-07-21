using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateProject.Migrations
{
    /// <inheritdoc />
    public partial class intitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServicePartners",
                columns: table => new
                {
                    PartnerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServiceCategory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommissionRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServicePartners", x => x.PartnerId);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionPlans",
                columns: table => new
                {
                    PlanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaxUnits = table.Column<int>(type: "int", nullable: false),
                    AllowedUnitTypes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaxMediaCount = table.Column<int>(type: "int", nullable: false),
                    HasFeaturedSearch = table.Column<bool>(type: "bit", nullable: true),
                    HasMonthlyReports = table.Column<bool>(type: "bit", nullable: true),
                    HasAccountManager = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionPlans", x => x.PlanId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    AdminId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.AdminId);
                    table.ForeignKey(
                        name: "FK_Admins_Users_AdminId",
                        column: x => x.AdminId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Owners",
                columns: table => new
                {
                    OwnerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Owners", x => x.OwnerId);
                    table.ForeignKey(
                        name: "FK_Owners_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ratings",
                columns: table => new
                {
                    RatingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GiverId = table.Column<int>(type: "int", nullable: true),
                    ReceiverId = table.Column<int>(type: "int", nullable: true),
                    Score = table.Column<int>(type: "int", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ratings", x => x.RatingId);
                    table.ForeignKey(
                        name: "FK_Ratings_Users_GiverId",
                        column: x => x.GiverId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ratings_Users_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    SmokingStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Personality = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Occupation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.TenantId);
                    table.ForeignKey(
                        name: "FK_Tenants_Users_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Properties",
                columns: table => new
                {
                    PropId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerId = table.Column<int>(type: "int", nullable: true),
                    PropType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Conditions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PublishStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ListingStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Properties", x => x.PropId);
                    table.ForeignKey(
                        name: "FK_Properties_Owners_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Owners",
                        principalColumn: "OwnerId");
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    SubscriptionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerId = table.Column<int>(type: "int", nullable: true),
                    PlanId = table.Column<int>(type: "int", nullable: true),
                    SubscriptionPlanPlanId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BillingCycle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PaymentStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.SubscriptionId);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Owners_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Owners",
                        principalColumn: "OwnerId");
                    table.ForeignKey(
                        name: "FK_Subscriptions_SubscriptionPlans_SubscriptionPlanPlanId",
                        column: x => x.SubscriptionPlanPlanId,
                        principalTable: "SubscriptionPlans",
                        principalColumn: "PlanId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Alerts",
                columns: table => new
                {
                    AlertId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    TargetPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TargetLocation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TargetType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alerts", x => x.AlertId);
                    table.ForeignKey(
                        name: "FK_Alerts_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId");
                });

            migrationBuilder.CreateTable(
                name: "ServiceRequests",
                columns: table => new
                {
                    RequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    PartnerId = table.Column<int>(type: "int", nullable: true),
                    ServicePartnerPartnerId = table.Column<int>(type: "int", nullable: true),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceRequests", x => x.RequestId);
                    table.ForeignKey(
                        name: "FK_ServiceRequests_ServicePartners_ServicePartnerPartnerId",
                        column: x => x.ServicePartnerPartnerId,
                        principalTable: "ServicePartners",
                        principalColumn: "PartnerId");
                    table.ForeignKey(
                        name: "FK_ServiceRequests_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId");
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    BookingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    PropId = table.Column<int>(type: "int", nullable: true),
                    PropertyPropId = table.Column<int>(type: "int", nullable: true),
                    BookingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PaymentStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.BookingId);
                    table.ForeignKey(
                        name: "FK_Bookings_Properties_PropertyPropId",
                        column: x => x.PropertyPropId,
                        principalTable: "Properties",
                        principalColumn: "PropId");
                    table.ForeignKey(
                        name: "FK_Bookings_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId");
                });

            migrationBuilder.CreateTable(
                name: "Media",
                columns: table => new
                {
                    MediaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PropId = table.Column<int>(type: "int", nullable: true),
                    PropertyPropId = table.Column<int>(type: "int", nullable: true),
                    MediaType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MediaUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Media", x => x.MediaId);
                    table.ForeignKey(
                        name: "FK_Media_Properties_PropertyPropId",
                        column: x => x.PropertyPropId,
                        principalTable: "Properties",
                        principalColumn: "PropId");
                });

            migrationBuilder.CreateTable(
                name: "Promotions",
                columns: table => new
                {
                    PromoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PropId = table.Column<int>(type: "int", nullable: true),
                    PropertyPropId = table.Column<int>(type: "int", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaidAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promotions", x => x.PromoId);
                    table.ForeignKey(
                        name: "FK_Promotions_Properties_PropertyPropId",
                        column: x => x.PropertyPropId,
                        principalTable: "Properties",
                        principalColumn: "PropId");
                });

            migrationBuilder.CreateTable(
                name: "PropertyApprovals",
                columns: table => new
                {
                    ApprovalId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdminId = table.Column<int>(type: "int", nullable: true),
                    PropId = table.Column<int>(type: "int", nullable: true),
                    PropertyPropId = table.Column<int>(type: "int", nullable: true),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovalStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyApprovals", x => x.ApprovalId);
                    table.ForeignKey(
                        name: "FK_PropertyApprovals_Admins_AdminId",
                        column: x => x.AdminId,
                        principalTable: "Admins",
                        principalColumn: "AdminId");
                    table.ForeignKey(
                        name: "FK_PropertyApprovals_Properties_PropertyPropId",
                        column: x => x.PropertyPropId,
                        principalTable: "Properties",
                        principalColumn: "PropId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_TenantId",
                table: "Alerts",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_PropertyPropId",
                table: "Bookings",
                column: "PropertyPropId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TenantId",
                table: "Bookings",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Media_PropertyPropId",
                table: "Media",
                column: "PropertyPropId");

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_PropertyPropId",
                table: "Promotions",
                column: "PropertyPropId");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_OwnerId",
                table: "Properties",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyApprovals_AdminId",
                table: "PropertyApprovals",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyApprovals_PropertyPropId",
                table: "PropertyApprovals",
                column: "PropertyPropId");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_GiverId",
                table: "Ratings",
                column: "GiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_ReceiverId",
                table: "Ratings",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_ServicePartnerPartnerId",
                table: "ServiceRequests",
                column: "ServicePartnerPartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_TenantId",
                table: "ServiceRequests",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_OwnerId",
                table: "Subscriptions",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_SubscriptionPlanPlanId",
                table: "Subscriptions",
                column: "SubscriptionPlanPlanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alerts");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Media");

            migrationBuilder.DropTable(
                name: "Promotions");

            migrationBuilder.DropTable(
                name: "PropertyApprovals");

            migrationBuilder.DropTable(
                name: "Ratings");

            migrationBuilder.DropTable(
                name: "ServiceRequests");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "Properties");

            migrationBuilder.DropTable(
                name: "ServicePartners");

            migrationBuilder.DropTable(
                name: "Tenants");

            migrationBuilder.DropTable(
                name: "SubscriptionPlans");

            migrationBuilder.DropTable(
                name: "Owners");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
