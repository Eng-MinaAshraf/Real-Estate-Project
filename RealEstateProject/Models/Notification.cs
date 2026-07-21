using System;

namespace RealEstateProject.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }

        // صاحب الإشعار (مرتبط بجدول User الأساسي)
        public int UserId { get; set; }
        public User? User { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        // رابط اختياري بيروح لصفحة متعلّقة بالإشعار
        public string? Url { get; set; }

        // اسم أيقونة Material Symbols يتعرض جنب الإشعار
        public string? Icon { get; set; }

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
