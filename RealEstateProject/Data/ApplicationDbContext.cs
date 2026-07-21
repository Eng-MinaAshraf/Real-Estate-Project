using RealEstateProject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RealEstateProject.Models;

namespace RealEstateProject.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Rating
            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Giver)
                .WithMany(u => u.RatingsGiven)
                .HasForeignKey(r => r.GiverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Receiver)
                .WithMany(u => u.RatingsReceived)
                .HasForeignKey(r => r.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            // User - Admin
            modelBuilder.Entity<User>()
                .HasOne(u => u.Admin)
                .WithOne(a => a.User)
                .HasForeignKey<Admin>(a => a.AdminId);

            // User - Owner
            modelBuilder.Entity<User>()
                .HasOne(u => u.Owner)
                .WithOne(o => o.User)
                .HasForeignKey<Owner>(o => o.OwnerId);

            // User - Tenant
            modelBuilder.Entity<User>()
                .HasOne(u => u.Tenant)
                .WithOne(t => t.User)
                .HasForeignKey<Tenant>(t => t.TenantId);

            // Decimal precision configuration
            modelBuilder.Entity<Alert>()
                .Property(a => a.MinPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Alert>()
                .Property(a => a.MaxPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Booking>()
                .Property(b => b.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Promotion>()
                .Property(p => p.PaidAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Property>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ServicePartner>()
                .Property(sp => sp.CommissionRate)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Subscription>()
                .Property(s => s.Discount)
                .HasPrecision(5, 2);

            modelBuilder.Entity<SubscriptionPlan>()
                .Property(sp => sp.MonthlyPrice)
                .HasPrecision(18, 2);

            // ربط العقار بالصور
            modelBuilder.Entity<Media>()
                .HasOne(m => m.Property)
                .WithMany(p => p.MediaFiles)
                .HasForeignKey(m => m.PropId)
                .OnDelete(DeleteBehavior.Cascade);

            // ربط العقار بالعروض الترويجية
            modelBuilder.Entity<Promotion>()
                .HasOne(p => p.Property)
                .WithMany(pr => pr.Promotions)
                .HasForeignKey(p => p.PropId)
                .OnDelete(DeleteBehavior.Cascade);

            // ربط العقار بالحجوزات
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Property)
                .WithMany(p => p.Bookings)
                .HasForeignKey(b => b.PropId)
                .OnDelete(DeleteBehavior.Cascade);

            // ربط العقار بموافقات الأدمن
            modelBuilder.Entity<PropertyApproval>()
                .HasOne(pa => pa.Property)
                .WithMany(p => p.PropertyApprovals)
                .HasForeignKey(pa => pa.PropId)
                .OnDelete(DeleteBehavior.Cascade);

            // ربط الأدمن بموافقات العقارات
            modelBuilder.Entity<PropertyApproval>()
                .HasOne(pa => pa.Admin)
                .WithMany(a => a.PropertyApprovals)
                .HasForeignKey(pa => pa.AdminId);

            // ربط خطة الاشتراك بالاشتراكات
            modelBuilder.Entity<Subscription>()
                .HasOne(s => s.SubscriptionPlan)
                .WithMany(sp => sp.Subscriptions)
                .HasForeignKey(s => s.PlanId);

            // ربط المالك بالاشتراكات
            modelBuilder.Entity<Subscription>()
                .HasOne(s => s.Owner)
                .WithMany(o => o.Subscriptions)
                .HasForeignKey(s => s.OwnerId);

            // ربط المالك بالعقارات
            modelBuilder.Entity<Property>()
                .HasOne(p => p.Owner)
                .WithMany(o => o.Properties)
                .HasForeignKey(p => p.OwnerId);

            // ربط المستأجر بالتنبيهات
            modelBuilder.Entity<Alert>()
                .HasOne(a => a.Tenant)
                .WithMany(t => t.Alerts)
                .HasForeignKey(a => a.TenantId);

            // ربط المستأجر بالحجوزات
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Tenant)
                .WithMany(t => t.Bookings)
                .HasForeignKey(b => b.TenantId);

            // ربط المستأجر بطلبات الخدمة
            modelBuilder.Entity<ServiceRequest>()
                .HasOne(sr => sr.Tenant)
                .WithMany(t => t.ServiceRequests)
                .HasForeignKey(sr => sr.TenantId);

            // ربط شركاء الخدمة بطلبات الخدمة
            modelBuilder.Entity<ServiceRequest>()
                .HasOne(sr => sr.ServicePartner)
                .WithMany(sp => sp.Service_Requests)
                .HasForeignKey(sr => sr.PartnerId);

            // ربط ApplicationUser (Identity) بجدول User بتاعك
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(au => au.AppUser)
                .WithOne()
                .HasForeignKey<ApplicationUser>(au => au.AppUserId);

            // ربط المستخدم بالإشعارات
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Notification>()
                .HasIndex(n => new { n.UserId, n.IsRead });

            // ===== Seed Data =====

            // Users (2 Owners, 2 Tenants, 1 Admin)
            modelBuilder.Entity<User>().HasData(
                new User { UserId = 1, Fname = "Ahmed", Lname = "Hassan", Email = "ahmed.hassan@example.com", Password = "Pass@123", Phone = "01012345678", Role = "Owner" },
                new User { UserId = 2, Fname = "Sara", Lname = "Mostafa", Email = "sara.mostafa@example.com", Password = "Pass@123", Phone = "01098765432", Role = "Owner" },
                new User { UserId = 3, Fname = "Omar", Lname = "Ali", Email = "omar.ali@example.com", Password = "Pass@123", Phone = "01055554444", Role = "Tenant" },
                new User { UserId = 4, Fname = "Nour", Lname = "Ibrahim", Email = "nour.ibrahim@example.com", Password = "Pass@123", Phone = "01033332222", Role = "Tenant" },
                new User { UserId = 5, Fname = "Khaled", Lname = "Mahmoud", Email = "khaled.mahmoud@example.com", Password = "Pass@123", Phone = "01011112222", Role = "Admin" }
            );

            // Owners (linked to UserId 1 and 2)
            modelBuilder.Entity<Owner>().HasData(
                new Owner { OwnerId = 1 },
                new Owner { OwnerId = 2 }
            );

            // Tenants (linked to UserId 3 and 4)
            modelBuilder.Entity<Tenant>().HasData(
                new Tenant { TenantId = 3, SmokingStatus = "Non-Smoker", Personality = "Quiet", Occupation = "Engineer" },
                new Tenant { TenantId = 4, SmokingStatus = "Smoker", Personality = "Social", Occupation = "Teacher" }
            );

            // Admin (linked to UserId 5)
            modelBuilder.Entity<Admin>().HasData(
                new Admin { AdminId = 5 }
            );

            // Properties (owned by Owner 1 and 2)
            modelBuilder.Entity<Property>().HasData(
                new Property { PropId = 1, OwnerId = 1, PropType = "Apartment", Purpose = "Rent", Price = 8500, Location = "Nasr City, Cairo", Conditions = "Furnished", PublishStatus = "Published", ListingStatus = "Available" },
                new Property { PropId = 2, OwnerId = 1, PropType = "Villa", Purpose = "Sale", Price = 4500000, Location = "New Cairo", Conditions = "Semi-Finished", PublishStatus = "Published", ListingStatus = "Available" },
                new Property { PropId = 3, OwnerId = 2, PropType = "Studio", Purpose = "Rent", Price = 5000, Location = "Maadi, Cairo", Conditions = "Furnished", PublishStatus = "Published", ListingStatus = "Rented" }
            );

            // Subscription Plans
            modelBuilder.Entity<SubscriptionPlan>().HasData(
                new SubscriptionPlan
                {
                    PlanId = 1,
                    PlanName = "Basic",
                    MaxUnits = 3,
                    AllowedUnitTypes = "Apartment",
                    MaxMediaCount = 5,
                    HasFeaturedSearch = false,
                    HasMonthlyReports = false,
                    HasAccountManager = false,
                    MonthlyPrice = 299
                },
                new SubscriptionPlan
                {
                    PlanId = 2,
                    PlanName = "Pro",
                    MaxUnits = 15,
                    AllowedUnitTypes = "Apartment,Villa,Studio",
                    MaxMediaCount = 15,
                    HasFeaturedSearch = true,
                    HasMonthlyReports = true,
                    HasAccountManager = false,
                    MonthlyPrice = 799
                },
                new SubscriptionPlan
                {
                    PlanId = 3,
                    PlanName = "Enterprise",
                    MaxUnits = 999999,
                    AllowedUnitTypes = "All Types",
                    MaxMediaCount = 999999,
                    HasFeaturedSearch = true,
                    HasMonthlyReports = true,
                    HasAccountManager = true,
                    MonthlyPrice = 1999
                }
            );

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<Media> Media { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Alert> Alerts { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<ServicePartner> ServicePartners { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }
        public DbSet<PropertyApproval> PropertyApprovals { get; set; }
        public DbSet<Notification> Notifications { get; set; }
    }
}