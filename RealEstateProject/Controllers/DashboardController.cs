using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstateProject.Data;
using RealEstateProject.Models.ViewModels;

namespace AqarX.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new DashboardViewModel();

            // نجيب اليوزر الحالي (الـ UserId = OwnerId في السكيمة دي)
            var email = User.Identity?.Name;
            var currentUser = string.IsNullOrEmpty(email)
                ? null
                : await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            // أساس الاستعلام لآخر العقارات مع الصور والمالك
            var recentQuery = _context.Properties
                .Include(p => p.Owner).ThenInclude(o => o.User)
                .Include(p => p.MediaFiles)
                .AsQueryable();

            if (User.IsInRole("Admin"))
            {
                // الأدمن يشوف كل النظام
                vm.TotalProperties = await _context.Properties.CountAsync();
                vm.ActiveSubscriptions = await _context.Subscriptions
                    .CountAsync(s => s.EndDate >= System.DateTime.Now);
                vm.AverageRating = await _context.Ratings
                    .Where(r => r.Score != null)
                    .AverageAsync(r => (double?)r.Score);
            }
            else if (User.IsInRole("Owner") && currentUser != null)
            {
                var ownerId = currentUser.UserId;

                // العقارات بتاعت المالك الحالي بس
                recentQuery = recentQuery.Where(p => p.OwnerId == ownerId);

                vm.TotalProperties = await _context.Properties
                    .CountAsync(p => p.OwnerId == ownerId);
                vm.ActiveSubscriptions = await _context.Subscriptions
                    .CountAsync(s => s.OwnerId == ownerId && s.EndDate >= System.DateTime.Now);
                vm.AverageRating = await _context.Ratings
                    .Where(r => r.ReceiverId == ownerId && r.Score != null)
                    .AverageAsync(r => (double?)r.Score);
            }
            else
            {
                // Tenant أو أي دور تاني: يشوف العقارات المتاحة والمنشورة بس
                recentQuery = recentQuery
                    .Where(p => p.ListingStatus == "Available" && p.PublishStatus == "Published");

                vm.TotalProperties = await recentQuery.CountAsync();

                if (currentUser != null)
                {
                    vm.AverageRating = await _context.Ratings
                        .Where(r => r.ReceiverId == currentUser.UserId && r.Score != null)
                        .AverageAsync(r => (double?)r.Score);
                }
            }

            vm.RecentProperties = await recentQuery
                .OrderByDescending(p => p.PropId)
                .Take(5)
                .ToListAsync();

            return View(vm);
        }
    }
}
