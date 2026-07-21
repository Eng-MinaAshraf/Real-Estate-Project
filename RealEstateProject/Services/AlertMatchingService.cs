using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RealEstateProject.Data;
using RealEstateProject.Models;

namespace RealEstateProject.Services
{
    // بيطابق العقار المنشور حديثًا مع كل الـ Alerts النشطة وينشئ إشعار لكل مستأجر مطابق
    public class AlertMatchingService
    {
        private readonly ApplicationDbContext _context;

        private const string MatchMessage =
            "Good news! A new property matching your saved search has just been published.";

        public AlertMatchingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task NotifyMatchingAlertsAsync(Property property)
        {
            if (property == null)
            {
                return;
            }

            // شرط أساسي: الإشعارات تتبعت بس لما العقار يكون منشور فعلاً
            if (property.PublishStatus != "Published")
            {
                return;
            }

            // نجيب كل الـ Alerts النشطة اللي بتطابق النوع والغرض والموقع
            // (المطابقة نصية حرفية زي باقي المشروع - PropType/Purpose/Location قيم نصية)
            var candidateAlerts = await _context.Alerts
                .Where(a => a.IsActive
                            && a.TenantId != null
                            && a.PropertyType == property.PropType
                            && a.Purpose == property.Purpose
                            && a.Location == property.Location)
                .ToListAsync();

            if (candidateAlerts.Count == 0)
            {
                return;
            }

            var propertyUrl = $"/Properties/Details/{property.PropId}";

            foreach (var alert in candidateAlerts)
            {
                if (!PriceMatches(alert, property.Price))
                {
                    continue;
                }

                // في السكيمة دي: TenantId == UserId، والإشعارات مربوطة بالـ UserId
                var userId = alert.TenantId!.Value;

                // منع التكرار: لو فيه إشعار غير مقروء لنفس اليوزر ونفس العقار بالفعل، نتخطى
                bool alreadyNotified = await _context.Notifications
                    .AnyAsync(n => n.UserId == userId && !n.IsRead && n.Url == propertyUrl);

                if (alreadyNotified)
                {
                    continue;
                }

                _context.Notifications.Add(new Notification
                {
                    UserId = userId,
                    Title = "New property match",
                    Message = MatchMessage,
                    Url = propertyUrl,
                    Icon = "notifications_active",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
        }

        // السعر مطابق لو أقل من أو يساوي MaxPrice، وأكبر من أو يساوي MinPrice لو موجود
        private static bool PriceMatches(Alert alert, decimal? propertyPrice)
        {
            if (propertyPrice == null)
            {
                return false;
            }

            if (alert.MaxPrice != null && propertyPrice > alert.MaxPrice)
            {
                return false;
            }

            // لو MinPrice = null نقارن بالـ MaxPrice بس (حسب المتطلبات)
            if (alert.MinPrice != null && propertyPrice < alert.MinPrice)
            {
                return false;
            }

            return true;
        }
    }
}
