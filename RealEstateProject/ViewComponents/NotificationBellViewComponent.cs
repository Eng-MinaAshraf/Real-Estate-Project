using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstateProject.Data;
using RealEstateProject.Models;

namespace RealEstateProject.ViewComponents
{
    public class NotificationBellViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        private const int PreviewCount = 6;

        public NotificationBellViewComponent(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public class NotificationBellViewModel
        {
            public int UnreadCount { get; set; }
            public List<Notification> Items { get; set; } = new List<Notification>();
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = await GetCurrentUserIdAsync();
            var model = new NotificationBellViewModel();

            if (userId != null)
            {
                model.Items = await _context.Notifications
                    .Where(n => n.UserId == userId)
                    .OrderByDescending(n => n.CreatedAt)
                    .Take(PreviewCount)
                    .ToListAsync();

                model.UnreadCount = await _context.Notifications
                    .CountAsync(n => n.UserId == userId && !n.IsRead);
            }

            return View(model);
        }

        private async Task<int?> GetCurrentUserIdAsync()
        {
            var appUser = await _userManager.GetUserAsync(HttpContext.User);
            if (appUser?.AppUserId != null)
            {
                return appUser.AppUserId;
            }

            var email = HttpContext.User.Identity?.Name;
            if (string.IsNullOrEmpty(email))
            {
                return null;
            }

            var domainUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            return domainUser?.UserId;
        }
    }
}
