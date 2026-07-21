using RealEstateProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealEstateProject.Data;

namespace RealEstateProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            // خدمة مطابقة العقارات المنشورة مع تنبيهات المستأجرين
            builder.Services.AddScoped<RealEstateProject.Services.AlertMatchingService>();

            // إضافة خدمات Identity
            builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
            .AddRoles<IdentityRole>() // ⚠️ سطر جديد، لازم عشان نفعّل نظام الـ Roles
            .AddEntityFrameworkStores<ApplicationDbContext>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Landing}/{id?}");

            app.MapRazorPages();

            // ⚠️ الكود الجديد: إنشاء الـ Roles + حساب Admin تلقائيًا عند أول تشغيل
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                SeedRolesAndAdminAsync(services).GetAwaiter().GetResult();
            }

            app.Run();
        }

        private static async Task SeedRolesAndAdminAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var context = services.GetRequiredService<ApplicationDbContext>();

            // 1. إنشاء الـ Roles الثلاثة لو مش موجودة
            string[] roleNames = { "Admin", "Owner", "Tenant" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // 2. إنشاء حساب Admin افتراضي لو مش موجود
            string adminEmail = "admin@aqarx.com";
            string adminPassword = "Admin@123";

            var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
            if (existingAdmin == null)
            {
                // 2.1 إنشاء صف في جدول Users بتاعك الأول
                var adminAppUser = new User
                {
                    Fname = "System",
                    Lname = "Admin",
                    Email = adminEmail,
                    Phone = "00000000000",
                    Role = "Admin",
                    Password = ""
                };
                context.Users.Add(adminAppUser);
                await context.SaveChangesAsync();

                context.Admins.Add(new Admin { AdminId = adminAppUser.UserId });
                await context.SaveChangesAsync();

                // 2.2 إنشاء حساب Identity ومربوطه بالـ User
                var adminIdentityUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    AppUserId = adminAppUser.UserId
                };

                var result = await userManager.CreateAsync(adminIdentityUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminIdentityUser, "Admin");
                }
            }
        }
    }
}