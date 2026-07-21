using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstateProject.Data;
using RealEstateProject.Models;

namespace AqarX.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // بيجيب UserId بتاع الـ domain user المرتبط بالحساب الحالي
        private async Task<int?> GetCurrentUserIdAsync()
        {
            var appUser = await _userManager.GetUserAsync(User);
            if (appUser?.AppUserId != null)
            {
                return appUser.AppUserId;
            }

            // fallback: نطابق بالإيميل زي باقي المشروع
            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email))
            {
                return null;
            }

            var domainUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            return domainUser?.UserId;
        }

        // GET: Notifications — صفحة عرض كل الإشعارات
        public async Task<IActionResult> Index()
        {
            var userId = await GetCurrentUserIdAsync();
            if (userId == null)
            {
                return View(new System.Collections.Generic.List<Notification>());
            }

            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return View(notifications);
        }

        // POST: Notifications/MarkAllAsRead
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = await GetCurrentUserIdAsync();
            if (userId != null)
            {
                var unread = await _context.Notifications
                    .Where(n => n.UserId == userId && !n.IsRead)
                    .ToListAsync();

                foreach (var n in unread)
                {
                    n.IsRead = true;
                }

                await _context.SaveChangesAsync();
            }

            // AJAX -> JSON، غير كده -> رجوع لصفحة الإشعارات
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true });
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Notifications/MarkAsRead/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = await GetCurrentUserIdAsync();
            if (userId != null)
            {
                var notification = await _context.Notifications
                    .FirstOrDefaultAsync(n => n.NotificationId == id && n.UserId == userId);

                if (notification != null && !notification.IsRead)
                {
                    notification.IsRead = true;
                    await _context.SaveChangesAsync();
                }
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true });
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
